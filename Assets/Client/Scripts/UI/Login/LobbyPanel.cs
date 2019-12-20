using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPanel : Panel
{
    private void OnEnable()
    {
        Client.OnConnectServer += AutoLogin;
        Client.OnLoginSuccess += HidePanel;
    }

    private void OnDisable()
    {
        Client.OnConnectServer -= AutoLogin;
        Client.OnLoginSuccess -= HidePanel;
    }

    private void AutoLogin()
    {
        //If we loged in this device
        Debug.Log("Attempt to login automatically");
        if (PlayerPrefs.HasKey(PlayerPrefKeys.UserId))
        {

            string userId = PlayerPrefs.GetString(PlayerPrefKeys.UserId);
            Debug.Log("Checking if user is a guest " + userId);

            if (Utility.IsGuest(userId))
                return;
            Debug.Log("Logging automatically with id " + userId);
            Client.Instance.SendLoginRequest(userId);
        }
    }
}
