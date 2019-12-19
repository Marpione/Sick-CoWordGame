using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestLogin : MonoBehaviour
{
    private void OnEnable()
    {
        Client.OnCreateAcountSuccess += LoginAsGuest;
    }

    private void OnDisable()
    {
        Client.OnCreateAcountSuccess -= LoginAsGuest;
    }

    public void LoginAsGuest()
    {
        try
        {
            //Check if the account exist
            if (PlayerPrefs.HasKey(PlayerPrefKeys.UserId))
            {
                string userId = PlayerPrefs.GetString(PlayerPrefKeys.UserId);
                Client.Instance.SendLoginRequest(userId);
                return;
            }

            Client.Instance.SendCreateAccount("Guest#" + Utility.GenerateRandom(256));
        }
        catch (System.Exception e)
        {
            Debug.LogError("Can't login as a guest: " + e);
        }
       
    }

    public void LoginAsGuest(Net_OnCreateAccount onCreateAccount)
    {
        try
        {
            string userId = PlayerPrefs.GetString(PlayerPrefKeys.UserId);
            Client.Instance.SendLoginRequest(userId);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Can't create account as a guest: " + e);
        }

    }
}
