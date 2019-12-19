using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookLogin : MonoBehaviour
{
    private void OnEnable()
    {
        Client.OnLoginFail += CreateAccountWithFacebook;
        Client.OnCreateAcountSuccess += LoginWithFacebook;
    }

    private void OnDisable()
    {
        Client.OnLoginFail -= CreateAccountWithFacebook;
        Client.OnCreateAcountSuccess -= LoginWithFacebook;
    }

    public void LoginWithFacebook()
    {
        Debug.Log(FacebookEntegration.FacebookIsLoggedIn());


        string currentUserId = PlayerPrefs.GetString(PlayerPrefKeys.UserId);

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
                //Check if user played as a guest
                if (!string.Equals(currentUserId, AccessToken.CurrentAccessToken.UserId))
                {
                    //Tell server to Change this account to a facebook connected account
                    Client.Instance.SendLoginRequest(currentUserId, AccessToken.CurrentAccessToken.UserId);
                    PlayerPrefs.SetString(PlayerPrefKeys.UserId, AccessToken.CurrentAccessToken.UserId);
                    return;
                }
            }
        });

        //Check if the user exits
        //Login with the acces token userId
        Client.Instance.SendLoginRequest(currentUserId);
    }

    public void LoginWithFacebook(Net_OnCreateAccount createAccount)
    {
        FacebookEntegration.FacebookLogin((result) =>
        {
            if (result.Error == null)
            {
                //Tell server to Change this account to a facebook connected account
                Client.Instance.SendLoginRequest(PlayerPrefs.GetString(PlayerPrefKeys.UserId));
                PlayerPrefs.SetString(PlayerPrefKeys.UserId, AccessToken.CurrentAccessToken.UserId);
            }
        });
    }

    void CreateAccountWithFacebook(Net_OnLoginRequest onLoginRequest)
    {
        Client.Instance.SendCreateAccount(AccessToken.CurrentAccessToken.UserId);
    }
}
