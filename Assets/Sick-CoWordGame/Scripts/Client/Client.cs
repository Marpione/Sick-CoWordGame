using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Sirenix.OdinInspector;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class Client : MonoBehaviour
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
        Debug.Log("Recived a message of type" + netMessage.OP);
        switch (netMessage.OP)
        {
            case NetOP.None:
                Debug.LogError("Unexpected Message from client");
                break;
        }
    }
    #endregion


    #region Send
    [Button(ButtonSizes.Medium)]
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
    #endregion

    [Button(ButtonSizes.Medium)]
    public void TestCreateAccount()
    {
        Net_CreateAccount ca = new Net_CreateAccount();
        ca.Username = "Swag";
        ca.Password = "1234";
        ca.Email = "clientMail@mail.com";
        SendServer(ca);
    }
}
