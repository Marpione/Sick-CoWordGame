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
    public static void InitFacebook () {

        if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init (InitCallback, OnHideUnity);
        } else {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp ();
        }

    }

    /// <summary>
    /// Cannot start login before this called.
    /// </summary>
    private static void InitCallback () {

        if (FB.IsInitialized) {
            // Signal an app activation App Event
            FB.ActivateApp ();
            // Continue with Facebook SDK
            // ...
        } else {
            if (debugMode)
                Debug.Log ("Failed to Initialize the Facebook SDK");
        }
    }

    private static void OnHideUnity (bool isGameShown) {
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
    public static void FacebookLogin (FacebookDelegate<ILoginResult> fbLoginCallback) {

        // InitFacebook ();
        var perms = new List<string> () { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions (perms, fbLoginCallback);
    }

    public static void FacebookLogout () {
        FB.LogOut ();
    }

    #endregion

    #region FacebookSharePart

    public static void FacebookShare () {

        FB.ShareLink (new System.Uri ("https://www.eksisozluk.com"), "Check it out!",
            "Good site!",
            new System.Uri ("https://resocoder.com/wp-content/uploads/2017/01/logoRound512.png"), callback: ShareCallback);

    }

    private static void ShareCallback (IShareResult result) {

        if (result.Cancelled || !String.IsNullOrEmpty (result.Error)) {
            if (debugMode)
                Debug.Log ("ShareLink Error: " + result.Error);
        } else if (!String.IsNullOrEmpty (result.PostId)) {
            // Print post identifier of the shared content
            if (debugMode)
                Debug.Log (result.PostId);
        } else {
            // Share succeeded without postID
            if (debugMode)
                Debug.Log ("ShareLink success!");
        }
    }
    #endregion

    #region GetFriendNamesAndPhotos

    public static async Task<List<FacebookFriendInfoHolder>> GetFriendDatasPlayingThisGame () {
        try {
            List<FacebookFriendInfoHolder> facebookFriendInfoHolder = await GetFriendsPlayingThisGame ();

            List<Task> taskArray = new List<Task> ();
            foreach (var item in facebookFriendInfoHolder) {
                Task task = GetFriendPhoto (item);
                taskArray.Add (task);
                task.Start ();
            }
            //Task.WaitAll (taskArray.ToArray ());
            return facebookFriendInfoHolder;
        } catch (Exception e) {
            Debug.LogError ("Error on getting friendDataPlayingThisGame: " + e.Message);
            return new List<FacebookFriendInfoHolder> ();
        }
    }

    public static async Task<List<FacebookFriendInfoHolder>> GetFriendsPlayingThisGame () {
        return await Task<List<FacebookFriendInfoHolder>>.Run (() => {
            var t = new TaskCompletionSource<List<FacebookFriendInfoHolder>> ();

            List<FacebookFriendInfoHolder> facebookFriendInfoHolder = new List<FacebookFriendInfoHolder> ();
            FacebookFriendInfoHolder friendInfoHolder;
            string query = "/me/friends";

            FB.API (query, HttpMethod.GET, result => {

                var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize (result.RawResult);
                var friendsList = (List<object>)dictionary ["data"];

                foreach (var dict in friendsList) {
                    friendInfoHolder = new FacebookFriendInfoHolder ();
                    friendInfoHolder.name = ((Dictionary<string, object>)dict) ["name"].ToString ();
                    friendInfoHolder.id = (((Dictionary<string, object>)dict) ["id"]).ToString ();
                    facebookFriendInfoHolder.Add (friendInfoHolder);
                    //Debug.Log ("friendId"+(((Dictionary<string, object>)dict) ["id"]).ToString ());
                }
                t.TrySetResult (facebookFriendInfoHolder);
            });
            return t.Task;

        });

    }

    public static async Task<Sprite> GetFriendPhoto (FacebookFriendInfoHolder facebookFriendInfo) {
        //return await Task<Sprite>.Run (() => {
        //    var t = new TaskCompletionSource<Sprite> ();

        //    string query = "/" + facebookFriendInfo.id + "/" + "picture";

        //    FB.API (query, HttpMethod.GET, result => {
        //        facebookFriendInfo.sprite = Sprite.Create (result.Texture, new Rect (0f, 0f, 50, 50), new Vector2 (0f, 0f));
        //        t.TrySetResult (Sprite.Create (result.Texture, new Rect (0f, 0f, 50, 50), new Vector2 (0f, 0f)));

        //    });
        //    return t.Task;
        //});
        Sprite friendSprite = null;
        string query = "/" + facebookFriendInfo.id + "/" + "picture";

        FB.API (query, HttpMethod.GET, result => {
            friendSprite = Sprite.Create (result.Texture, new Rect (0f, 0f, 50, 50), new Vector2 (0f, 0f));
        });

        return friendSprite;
    }

    //public static IEnumerator GetFriendPhotoForLeaderBoardCoroutine (string friendId, TaskCompletionSource<bool> t, Action<Sprite, LeaderBoardElement> callback, LeaderBoardElement giftPopupElementRef)
    //{
    //    Sprite friendSprite = null;
    //    string query = "/" + friendId + "/" + "picture";

    //    FB.API (query, HttpMethod.GET, result => {
    //        friendSprite = Sprite.Create (result.Texture, new Rect (0f, 0f, 50, 50), new Vector2 (0f, 0f));
    //        callback (friendSprite, giftPopupElementRef);
    //    });

    //    yield break;
    //}

    //public static IEnumerator GetFriendPhotoForGiftCoroutine (string friendId, TaskCompletionSource<bool> t, Action<Sprite, GiftPopUpElement> callback, GiftPopUpElement giftPopupElementRef)
    //{
    //    Sprite friendSprite = null;
    //    string query = "/" + friendId + "/" + "picture";

    //    FB.API (query, HttpMethod.GET, result => {
    //        friendSprite = Sprite.Create (result.Texture, new Rect (0f, 0f, 50, 50), new Vector2 (0f, 0f));
    //        callback(friendSprite, giftPopupElementRef);
    //        t.TrySetResult (true);
    //    });
        
    //    yield break;
    //}


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
    public Sprite sprite;
    internal bool giftUsable;
}
