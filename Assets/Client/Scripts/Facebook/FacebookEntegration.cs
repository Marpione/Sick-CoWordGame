using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System;
using UnityEngine.UI;
using Facebook.Unity.Example;
using Facebook.MiniJSON;
using System.Threading.Tasks;

public static class FacebookEntegration {

    public static bool debugMode = false;

    #region initPart

    /// <summary>
    /// Must only called once.
    /// </summary>
    public static void InitFacebook() {

        if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        } else {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }

    }

    /// <summary>
    /// Cannot start login before this called.
    /// </summary>
    private static void InitCallback() {

        if (FB.IsInitialized) {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        } else {
            if (debugMode)
                Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private static void OnHideUnity(bool isGameShown) {
        if (!isGameShown) {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        } else {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
    #endregion

    #region FacebookLoginPart
    public static void FacebookLogin(FacebookDelegate<ILoginResult> fbLoginCallback) {

        // InitFacebook ();
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, fbLoginCallback);
    }

    public static void FacebookLogout() {
        FB.LogOut();
    }

    #endregion

    #region FacebookSharePart

    public static void FacebookShare() {

        FB.ShareLink(new System.Uri("https://www.eksisozluk.com"), "Check it out!",
            "Good site!",
            new System.Uri("https://resocoder.com/wp-content/uploads/2017/01/logoRound512.png"), callback: ShareCallback);

    }

    private static void ShareCallback(IShareResult result) {

        if (result.Cancelled || !String.IsNullOrEmpty(result.Error)) {
            if (debugMode)
                Debug.Log("ShareLink Error: " + result.Error);
        } else if (!String.IsNullOrEmpty(result.PostId)) {
            // Print post identifier of the shared content
            if (debugMode)
                Debug.Log(result.PostId);
        } else {
            // Share succeeded without postID
            if (debugMode)
                Debug.Log("ShareLink success!");
        }
    }
    #endregion

    #region GetFriendNamesAndPhotos

    public static List<Account> facebookFriends;

    public delegate void FriendListInitilizedAction(List<Account> facebookFriends);
    public static event FriendListInitilizedAction OnFriendsListInitilized;

    public static void GetFacebookFriedns()
    {
        string query = "/me/friends";
        FB.API(query, HttpMethod.GET, result =>
        {
            var dictinary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            var friendList = (List<object>)dictinary["data"];
            foreach (var dict in friendList)
            {
                Account facebookFriend = new Account();
                facebookFriend.userId = (((Dictionary<string, object>)dict)["id"]).ToString();
                facebookFriend.name = (((Dictionary<string, object>)dict)["name"]).ToString();
            }

            if (OnFriendsListInitilized != null)
                OnFriendsListInitilized.Invoke(facebookFriends);
        });
    }
   


    #endregion

        #region Inviting

        /// <summary>
        /// Facebook Invitation to other friends
        /// </summary>
        /// <returns></returns>
    //    public static async Task<List<string>> FacebookInvite ()  {
    //    var t = new TaskCompletionSource<List<string>>();
    //    // FB.AppRequest .FacebookInvite ();// ("Come play this great game!", null, null, (System.Collections.Generic.IEnumerable<string>)"https://play.google.com/store/apps/details?id=com.gl.angrybullattack.bull.racing&hl=tr", null, null, delegate (IAppRequestResult result))
    //    FB.AppRequest ("Come play this great Pachinko Game!", null, null, null, null, null, null, delegate (IAppRequestResult result) {
    //        if (debugMode)
    //            Debug.Log ("Invited People" + result.RawResult);
    //        t.TrySetResult((System.Collections.Generic.List<string>) result.To);
    //        _ = NetworkSystem.Instance.InviteUsersWithFacebook((System.Collections.Generic.List<string>)result.To);
    //    });
    //    return await t.Task;
    //}

    #endregion

    #region ProfilePhoto

    public static Sprite GetMyProfilePhoto () {

        Sprite profilePhoto = null;

        FB.API ("me/picture?width=100&height=100", HttpMethod.GET, result => {
            if (debugMode)
                Debug.Log ("Picture Part");
            if (result.Texture != null) {
               
                profilePhoto = Sprite.Create (result.Texture, new Rect (0f, 0f, 100, 100), new Vector2 (0f, 0f));
                
                if (debugMode)
                    Debug.Log ("Picture is not null");
            } else {
                if (debugMode)
                    Debug.Log ("Picture is null");
            }
        });

        return profilePhoto;
    }

    #endregion

    #region FacebookLoginIsActive
    public static bool FacebookIsLoggedIn () 
    {
        return FB.IsLoggedIn;
    }
    #endregion
}
[System.Serializable]
public class FacebookFriendInfoHolder {
    public string name;
    public string id;
}
