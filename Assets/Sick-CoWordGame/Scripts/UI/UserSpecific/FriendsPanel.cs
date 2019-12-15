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

    public void GetFriends()
    {
        GameObject go = Instantiate(FriendDiplayPrefab, transform);
        FriendDiplay friendDiplay = go.GetComponent<FriendDiplay>();

        friendDiplay.SetUpFriend("Test Friend", 1);
        friendDiplays.Add(friendDiplay);
    }
}
