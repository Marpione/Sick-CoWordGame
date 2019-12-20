using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookLogin : MonoBehaviour
{
    private void OnEnable()
    {
        Client.OnCreateAcountSuccess += LoginWithFacebook;
    }

    private void OnDisable()
    {
        Client.OnCreateAcountSuccess -= LoginWithFacebook;
    }

    public void LoginWithFacebook()
    {
        Debug.Log(FacebookEntegration.FacebookIsLoggedIn());


        string currentUserId = string.Empty;
        if(PlayerPrefs.HasKey(PlayerPrefKeys.UserId))
            currentUserId = PlayerPrefs.GetString(PlayerPrefKeys.UserId);

        FacebookEntegration.FacebookLogin((result) =>
        {

            if(result.Error != null)
            {
                Debug.LogError("Facebook login error result: " + result.Error);
                return;
            }
            else if(result.Cancelled)
            {
                Debug.LogError("Facebook login error result: " + result.Cancelled);
                return;
            }
            else
            {
                Debug.Log(currentUserId);
                //If true there has been no login from this phone Create the user
                if(string.IsNullOrEmpty(currentUserId))
                {
                    CreateAccountWithFacebook();
                    return;
                }

                //Check if user played as a guest
                if (!string.Equals(currentUserId, AccessToken.CurrentAccessToken.UserId))
                {
                    //Tell server to Change this account to a facebook connected account
                    Client.Instance.SendLoginRequest(currentUserId, AccessToken.CurrentAccessToken.UserId);
                    PlayerPrefs.SetString(PlayerPrefKeys.UserId, AccessToken.CurrentAccessToken.UserId);
                    return;
                }
                else
                {
                    Client.Instance.SendLoginRequest(AccessToken.CurrentAccessToken.UserId);
                }
            }
        });
    }

    public void LoginWithFacebook(Net_OnCreateAccount createAccount)
    {
        if (Utility.IsGuest(createAccount.UserId))
            return;

        Client.Instance.SendLoginRequest(PlayerPrefs.GetString(PlayerPrefKeys.UserId));
    }

    void CreateAccountWithFacebook()
    {
        Client.Instance.SendCreateAccount(AccessToken.CurrentAccessToken.UserId);
        PlayerPrefs.SetString(PlayerPrefKeys.UserId, AccessToken.CurrentAccessToken.UserId);
    }
}
