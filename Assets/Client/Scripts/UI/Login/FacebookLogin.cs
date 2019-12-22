using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using Sirenix.OdinInspector;
public class FacebookLogin : MonoBehaviour
{
    public List<FacebookFriendInfoHolder> FacebookFriends;
    public Text DebugText;

    private void OnEnable()
    {
        Client.OnLoginSuccess += GetFacebookFriends;
        Client.OnLoginFail += CreateANewAccountForDiffferentFacebookUser;
        Client.OnCreateAcountSuccess += LoginWithFacebook;
        
    }

    private void OnDisable()
    {
        Client.OnLoginSuccess -= GetFacebookFriends;
        Client.OnLoginFail -= CreateANewAccountForDiffferentFacebookUser;
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
                    if(Utility.IsGuest(currentUserId))
                    {
                        //Tell server to Change this account to a facebook connected account
                        Client.Instance.SendLoginRequest(currentUserId, AccessToken.CurrentAccessToken.UserId);
                        PlayerPrefs.SetString(PlayerPrefKeys.UserId, AccessToken.CurrentAccessToken.UserId);
                        return;
                    }
                    else
                    {
                        //Switch facebook account
                        Client.Instance.SendLoginRequest(AccessToken.CurrentAccessToken.UserId);
                    }

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
        FacebookEntegration.FacebookLogin((result) =>
        {

            if (result.Error != null)
            {
                Debug.LogError("Facebook login error result: " + result.Error);
                return;
            }
            else if (result.Cancelled)
            {
                Debug.LogError("Facebook login error result: " + result.Cancelled);
                return;
            }
            else
            {
                Client.Instance.SendLoginRequest(PlayerPrefs.GetString(PlayerPrefKeys.UserId));
            }
        });
    }

    void CreateAccountWithFacebook()
    {
        Client.Instance.SendCreateAccount(AccessToken.CurrentAccessToken.UserId);
        PlayerPrefs.SetString(PlayerPrefKeys.UserId, AccessToken.CurrentAccessToken.UserId);
    }

    void CreateANewAccountForDiffferentFacebookUser(Net_OnLoginRequest olr)
    {
        if (!FacebookEntegration.FacebookIsLoggedIn())
            return;
        //This is the case when we play as a facebook user then login with a different facebook account
        //Check if the login respone has user id
        if (string.IsNullOrEmpty(olr.UserId))
        {
            //Check if there is a user id in this mechine
            if(PlayerPrefs.HasKey(PlayerPrefKeys.UserId))
            {
                string userId = PlayerPrefs.GetString(PlayerPrefKeys.UserId);
                //Check if the user id
                if (!Utility.IsGuest(userId))
                {
                    //Check if we want to login with a different facebook account from the saved one
                    if(!string.Equals(userId, AccessToken.CurrentAccessToken.UserId))
                        CreateAccountWithFacebook();
                }
            }
        }
    }

    private void GetFacebookFriends(Net_OnLoginRequest onLoginRequest)
    {
        if(FacebookEntegration.FacebookIsLoggedIn())
            FacebookEntegration.GetFacebookFriedns();
    }
}
