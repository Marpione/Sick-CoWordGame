using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsPanel : Panel
{
    public GameObject FriendDiplayPrefab;


    private List<FriendDiplay> friendDiplays = new List<FriendDiplay>();

    public Net_OnAddFriend lastAddedFriend;

    private void OnEnable()
    {
        Client.OnLoginSuccess += AskForFirends;
        Client.OnAddFriend += AddFriendToList;
        Client.OnRequestFriend += GetFriends;
        //Client.OnRemoveFriend += AskForFirends;
    }

    private void OnDisable()
    {
        Client.OnLoginSuccess -= AskForFirends;
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
        Debug.Log(net_OnAddFriend.Success);
        if (net_OnAddFriend.Success == 1)
        {
            GameObject go = Instantiate(FriendDiplayPrefab, transform);
            FriendDiplay friendDiplay = go.GetComponent<FriendDiplay>();
            friendDiplay.SetUpFriend(net_OnAddFriend.FriendAccount);
            friendDiplays.Add(friendDiplay);
            lastAddedFriend = net_OnAddFriend;
        }
    }

    void UpdateFriend(Account account)
    {
        string updatedFriend = account.Username + "#" + account.Discriminator;
        for (int i = 0; i < friendDiplays.Count; i++)
        {
            string currentFriend = friendDiplays[i].FriendAccount.Username + "#" + friendDiplays[i].FriendAccount.Discriminator;

            if (string.Equals(currentFriend, updatedFriend))
                friendDiplays[i].SetUpFriend(account);
        }
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
                friendDiplay.SetUpFriend(friend);
                friendDiplays.Add(friendDiplay);
            }
        }
            
    }
}
