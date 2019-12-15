using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsPanel : Panel
{
    public GameObject FriendDiplayPrefab;


    private List<FriendDiplay> friendDiplays = new List<FriendDiplay>();

    private void OnEnable()
    {
        Client.OnLoginSuccess += GetFriends;
        Client.OnAddFriend += GetFriends;
        Client.OnRemoveFriend += GetFriends;
    }

    private void OnDisable()
    {
        Client.OnLoginSuccess -= GetFriends;
        Client.OnAddFriend -= GetFriends;
        Client.OnRemoveFriend -= GetFriends;
    }

    public void GetFriends(NetMessage netMessage)
    {
        Client.Instance.SendFriendRequest();

        try
        {
            Net_OnRequestFriend requestFriendMessage = (Net_OnRequestFriend)netMessage;

            if (requestFriendMessage != null)
            {
                return;
            }


            Net_OnAddFriend addFriendMessage = (Net_OnAddFriend)netMessage;

            if (addFriendMessage != null)
            {
                Account friendAccount = addFriendMessage.FriendAccount;
                GameObject go = Instantiate(FriendDiplayPrefab, transform);
                FriendDiplay friendDiplay = go.GetComponent<FriendDiplay>();

                friendDiplay.SetUpFriend(friendAccount.Username + "#" + friendAccount.Discriminator, friendAccount.ActiveconnectionStatus);
                friendDiplays.Add(friendDiplay);
                return;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            throw;
        }

        

        
    }
}
