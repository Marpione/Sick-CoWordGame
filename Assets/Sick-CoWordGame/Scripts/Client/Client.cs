using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Sirenix.OdinInspector;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class Client : Singleton<Client>
{
    private const int MAX_USER = 100;
    private const int port = 13000;
    private const int web_Port = 13001;
    private const string serverIP = "127.0.0.1";
    public int BYTE_SIZE = 1024;


    private byte relibleChannel;
    private int connectionID;
    private int hostId;
    private byte error;

    private bool isStarted = false;

    private Account self;
    private string token;

    #region Events
    public delegate void OnLoginSuccessAction(Net_OnLoginRequest netMessage);
    public static event OnLoginSuccessAction OnLoginSuccess;

    public delegate void OnLoginFailAction(Net_OnLoginRequest netMessage);
    public static event OnLoginFailAction OnLoginFail;

    public delegate void OnCreateAccountSuccessAction(Net_OnCreateAccount netMessage);
    public static event OnCreateAccountSuccessAction OnCreateAcountSuccess;

    public delegate void OnCreateAccountFailAction(Net_OnCreateAccount netMessage);
    public static event OnCreateAccountFailAction OnCreateAcountFail;

    public delegate void OnAddFriendAction(Net_OnAddFriend net_OnAddFriend);
    public static event OnAddFriendAction OnAddFriend;

    public delegate void OnRemoveFriendAction(NetMessage netMessage);
    public static event OnRemoveFriendAction OnRemoveFriend;

    public delegate void OnRequestFriendAction(Net_OnRequestFriend netMessage);
    public static event OnRequestFriendAction OnRequestFriend;

    #endregion

    #region MonoBehaviour
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Initilize();
    }

    private void Update()
    {
        UpdateMessageBumb();
    }
    #endregion
    public void Initilize()
    {
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();
        cc.AddChannel(QosType.Reliable);

        HostTopology hostTopology = new HostTopology(cc, MAX_USER);

        //Client Only Code
        hostId = NetworkTransport.AddHost(hostTopology, 0);

        //StandaloneClient
        connectionID = NetworkTransport.Connect(hostId, serverIP, port, 0, out error);

        //WebClient
        //NetworkTransport.Connect(hostId, serverIP, web_Port, 0, out error);

        isStarted = true;
        Debug.Log(string.Format("Attemting to connect on {0}...", serverIP));
    }

    public void ShutDown()
    {
        isStarted = false;
        NetworkTransport.Shutdown();
    }

    public void UpdateMessageBumb()
    {
        if (!isStarted)
            return;

        int recHostId; //Where is it from
        int channelId; //Which Lane is he sending that messageFrom

        byte[] recBuffer = new byte[BYTE_SIZE];
        int dataSize;

        NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionID, out channelId, recBuffer, BYTE_SIZE, out dataSize, out error);

        switch (type)
        {
            case NetworkEventType.DataEvent:
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMessage netMessage = (NetMessage)formatter.Deserialize(ms);
                OnData(connectionID, channelId, recHostId, netMessage);
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("We have connected to server");
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("We have disconnected to server");
                break;
            case NetworkEventType.Nothing:
                break;

            default:
            case NetworkEventType.BroadcastEvent:
                Debug.Log("Unexpected network Event Type");
                break;
        }
    }
    #region OnData
    private void OnData(int connectionId, int channelId, int recHostId, NetMessage netMessage)
    {
        Debug.Log("Recived a message of type " + netMessage.OP);
        Debug.Log("Recived message type "+ netMessage.GetType());
        switch (netMessage.OP)
        {
            case NetOP.None:
                Debug.LogError("Unexpected Message from client");
                break;
            case NetOP.OnCreateAccount:
                OnCreateAccount((Net_OnCreateAccount) netMessage);
                break;

            case NetOP.OnLoginRequest:
                OnLoginRequest((Net_OnLoginRequest) netMessage);
                break;

            case NetOP.OnAddFriend:
                Debug.Log("should invoke Add friend");
                if (OnAddFriend != null)
                {
                    OnAddFriend.Invoke((Net_OnAddFriend)netMessage);
                    Debug.Log("Invoked Add friend");
                }
                break;

            //case NetOP.onre:
            //    if (OnRemoveFriend != null)
            //        OnRemoveFriend.Invoke(netMessage);
            //    break;

            case NetOP.OnRequestFriend:
                if (OnRequestFriend != null)
                    OnRequestFriend.Invoke((Net_OnRequestFriend)netMessage);
                break;
        }
    }

    private void OnCreateAccount(Net_OnCreateAccount oca)
    {
        Debug.Log(oca.Success);
        if (oca.Success != 0)
        {
            Debug.Log("Create account Sucess " + oca.Information);
            if (OnCreateAcountSuccess != null)
                OnCreateAcountSuccess.Invoke(oca);
        }
        else
        {
            Debug.Log("Create Failed " + oca.Information);
            if (OnCreateAcountFail != null)
                OnCreateAcountFail.Invoke(oca);
        }
    }

    private void OnLoginRequest(Net_OnLoginRequest olr)
    {
        if (olr.Success != 1)
        {
            Debug.Log("Login Fail " + olr.Information);
            if (OnLoginFail != null)
                OnLoginFail.Invoke(olr);
          
        }
        else 
        {
            Debug.Log(olr.Information);
            if (OnLoginSuccess != null)
            {
                OnLoginSuccess.Invoke(olr);
                Debug.Log("Invoke Login success");
            }

            self = new Account();
            self.ActiveconnectionStatus = olr.ConnectionId;
            self.Username = olr.Username;
            self.Discriminator = olr.Discriminator;

            token = olr.Token;
        }
    }

    #endregion


    #region Send
    public void SendServer(NetMessage netMessage)
    {
        //This is where we hold our data
        byte[] buffer = new byte[BYTE_SIZE];

        //This is where you would crush your data into bytes
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, netMessage);



        NetworkTransport.Send(hostId, connectionID, relibleChannel, buffer, BYTE_SIZE, out error);
    }

    [Button(ButtonSizes.Medium)]
    public void SendCreateAccount(string username, string password, string email)
    {
        if (!Utility.IsUsername(username))
            return;

        if (!Utility.IsEmail(email))
            return;

        if (string.IsNullOrEmpty(password))
            return;

        Net_CreateAccount createAccount = new Net_CreateAccount();

        createAccount.Username = username;
        createAccount.Password = Utility.Sha256FromString(password);
        createAccount.Email = email;

        SendServer(createAccount);
    }
    [Button(ButtonSizes.Medium)]
    public void SendLoginRequest(string usernameOrEmail, string password)
    {
        if (!Utility.IsUsernameAndDiscriminator(usernameOrEmail) && !Utility.IsEmail(usernameOrEmail))
            return;

        if (string.IsNullOrEmpty(password))
            return;

        Net_LoginRequest loginRequest = new Net_LoginRequest();

        loginRequest.UsernameOrEmail = usernameOrEmail;
        loginRequest.Password = Utility.Sha256FromString(password);

        SendServer(loginRequest);
        Debug.Log("Send a login Request"); 
    }

    public void SendAddFriend(string usernameOrEmail)
    {
        Net_AddFriend af = new Net_AddFriend();

        af.Token = token;
        af.UsernameOrEmail = usernameOrEmail;

        SendServer(af);
    }

    public void SendRemoveFriend(string usernameOrEmail)
    {
        Net_RemoveFriend rf = new Net_RemoveFriend();

        rf.Token = token;
        rf.UsernameOrEmail = usernameOrEmail;

        SendServer(rf);
    }

    public void SendFriendRequest()
    {
        Net_RequestFriend rf = new Net_RequestFriend();
        rf.Token = token;
        SendServer(rf);
    }


    #endregion
}
