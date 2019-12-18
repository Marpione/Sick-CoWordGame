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

       
        FacebookEntegration.FacebookLogin((result) =>
        {
            Debug.Log("Logggg res err: " + result.Error);

            if (result.Error == null)
            {
                //Check if the user exits
                Client.Instance.SendLoginRequestWithFacebook(AccessToken.CurrentAccessToken.UserId);
            }
        });
    }

    public void LoginWithFacebook(Net_OnCreateAccount createAccount)
    {
        if(createAccount.AccountType == AccountType.facebook)
        {
            FacebookEntegration.FacebookLogin((result) =>
            {
                Debug.Log("Logggg res err: " + result.Error);

                if (result.Error == null)
                {
                    //Check if the user exits
                    Client.Instance.SendLoginRequestWithFacebook(AccessToken.CurrentAccessToken.UserId);
                }
            });
        }
    }

    void CreateAccountWithFacebook(Net_OnLoginRequest onLoginRequest)
    {
        Debug.Log(FacebookEntegration.FacebookIsLoggedIn());

        if (FacebookEntegration.FacebookIsLoggedIn())
        {
            var accessToken = AccessToken.CurrentAccessToken;
            Client.Instance.SendCreateAccount("facebook_user"+ Utility.GenerateRandom(256), Utility.GenerateRandom(256), "facebook_email", accessToken.UserId);
        }
    }
}
