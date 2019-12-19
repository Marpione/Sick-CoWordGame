using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestLogin : MonoBehaviour
{
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void LoginAsGuest()
    {
        //Check if the account exist
        if(PlayerPrefs.HasKey(PlayerPrefKeys.UserId))
        {
            string userId = PlayerPrefs.GetString(PlayerPrefKeys.UserId);
            Client.Instance.SendLoginRequest(userId);
            return;
        }

        Client.Instance.SendCreateAccount("Guest#" + Utility.GenerateRandom(256));
    }

    public void LoginAsGuest(Net_OnCreateAccount onCreateAccount)
    {
        string userId = PlayerPrefs.GetString(PlayerPrefKeys.UserId);
        Client.Instance.SendLoginRequest(userId);
    }
}
