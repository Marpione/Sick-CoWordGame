using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsPanel : Panel
{
    public GameObject FriendDiplayPrefab;


    private List<FriendDiplay> friendDiplays = new List<FriendDiplay>();

    private void OnEnable()
    {
        Client.OnLoginSuccess += AskForFirends;
        Client.OnAddFriend += AskForFirends;
        Client.OnAddFriend += AddFriendToList;
        Client.OnRequestFriend += GetFriends;
        //Client.OnRemoveFriend += AskForFirends;
    }

    private void OnDisable()
    {
        Client.OnLoginSuccess -= AskForFirends;
        Client.OnAddFriend -= AskForFirends;
        Client.OnAddFriend -= AddFriendToList;
        Client.OnRequestFriend -= GetFriends;
    }

    void AskForFirends(NetMessage netMessage)
    {
        Debug.Log("Asking friend Request");
        Debug.Log(netMessage.GetType());
        Client.Instance.SendFriendRequest();
    }

    void AddFriendToList(Net_OnAddFriend net_OnAddFriend)
    {
        GameObject go = Instantiate(FriendDiplayPrefab, transform);
        FriendDiplay friendDiplay = go.GetComponent<FriendDiplay>();
        friendDiplay.SetUpFriend(net_OnAddFriend.FriendAccount.Username + "#" + net_OnAddFriend.FriendAccount.Discriminator, net_OnAddFriend.FriendAccount.Status);
        friendDiplays.Add(friendDiplay);
    }

    public void GetFriends(Net_OnRequestFriend netMessage)
    {
        Debug.Log("Requested friends");
        Net_OnRequestFriend requestFriendMessage = (Net_OnRequestFriend)netMessage;

        if (requestFriendMessage != null)
        {
            Debug.Log(requestFriendMessage.FriendRequests.Count);
            foreach (var friend in requestFriendMessage.FriendRequests)
            {
                GameObject go = Instantiate(FriendDiplayPrefab, transform);
                FriendDiplay friendDiplay = go.GetComponent<FriendDiplay>();
                friendDiplay.SetUpFriend(friend.Username + "#" + friend.Discriminator, friend.Status);
                friendDiplays.Add(friendDiplay);
            }
        }
            
    }
}
