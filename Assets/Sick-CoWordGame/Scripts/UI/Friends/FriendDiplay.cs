using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendDiplay : MonoBehaviour
{
    public Image FriendAvatarImage;
    public Text FriendUserNameText;
    public Image FriendOnlinestatusImage;

    Button button;
    Button Button { get { return (button == null) ? button = GetComponent<Button>() : button; } }

    string username;

    public void SetUpFriend(string friendUserName, int onlineStatus)
    {
        username = friendUserName;
        FriendUserNameText.text = friendUserName;
        FriendOnlinestatusImage.color = (onlineStatus != 1) ? FriendOnlinestatusImage.color = Color.red : FriendOnlinestatusImage.color = Color.green;
        Button.onClick.AddListener(DeleteThisFriend);
    }

    void DeleteThisFriend()
    {
        Net_RemoveFriend removeFriend = new Net_RemoveFriend();
        removeFriend.Token = Client.Instance.Token;
        removeFriend.Username = username;
        Client.Instance.SendServer(removeFriend);
        Destroy(gameObject);
    }

}
