using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddFriendPanel : Panel
{
    public InputField AddFriendField;

    private void OnEnable()
    {
        Client.OnLoginSuccess += ShowPanel;
        Client.OnLoginFail += HidePanel;
    }

    private void OnDisable()
    {
        Client.OnLoginSuccess -= ShowPanel;
        Client.OnLoginFail -= HidePanel;
    }

    public void AddFriend()
    {
        string usernameOrEmail = AddFriendField.text;

        if(!Utility.IsUsernameAndDiscriminator(usernameOrEmail))
        {
            return;
        }
        Debug.Log("Adding friend " + usernameOrEmail);

        Client.Instance.SendAddFriend(usernameOrEmail);
    }

    public void RemoveFriend(string username, string discrimiator)
    {
        Client.Instance.SendAddFriend(username + "#" + discrimiator);
    }
}
