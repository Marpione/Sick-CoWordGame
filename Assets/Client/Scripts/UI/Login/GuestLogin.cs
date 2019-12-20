using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestLogin : MonoBehaviour
{
    private void OnEnable()
    {
        //Client.OnLoginFail += CreateAccount;
        Client.OnCreateAcountSuccess += LoginAsGuest;
    }

    private void OnDisable()
    {
        //Client.OnLoginFail -= CreateAccount;
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
            string id = "Guest#" + Utility.GenerateRandom(25);
            Client.Instance.SendCreateAccount(id);
            PlayerPrefs.SetString(PlayerPrefKeys.UserId, id);
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

    void CreateAccount(Net_OnLoginRequest loginRequest)
    {
        if (!Utility.IsGuest(loginRequest.UserId))
            return;

        Client.Instance.SendCreateAccount(loginRequest.UserId);
    }
}
