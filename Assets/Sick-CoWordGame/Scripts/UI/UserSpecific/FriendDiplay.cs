using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendDiplay : MonoBehaviour
{
    public Image FriendAvatarImage;
    public Text FriendUserNameText;
    public Image FriendOnlinestatusImage;

    public void SetUpFriend(string friendUserName, int onlineStatus)
    {
        FriendUserNameText.text = friendUserName;
        FriendOnlinestatusImage.color = (onlineStatus != 1) ? FriendOnlinestatusImage.color = Color.red : FriendOnlinestatusImage.color = Color.green;
    }
}
