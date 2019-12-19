using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendDiplay : MonoBehaviour
{
    public Image FriendAvatarImage;
    public Text FriendUserNameText;
    public Image FriendOnlinestatusImage;

    private Account friendAccount;
    public Account FriendAccount { get { return friendAccount; } }

    Button button;
    Button Button { get { return (button == null) ? button = GetComponent<Button>() : button; } }


    public void SetUpFriend(Account fa)
    {
        friendAccount = fa;
        FriendUserNameText.text = friendAccount.userId;
        FriendOnlinestatusImage.color = (friendAccount.Status != 1) ? FriendOnlinestatusImage.color = Color.red : FriendOnlinestatusImage.color = Color.green;
        Button.onClick.AddListener(DeleteThisFriend);
    }

    void DeleteThisFriend()
    {
        Net_RemoveFriend removeFriend = new Net_RemoveFriend();
        removeFriend.Token = Client.Instance.Token;
        removeFriend.userId = friendAccount.userId;
        Client.Instance.SendServer(removeFriend);
        Destroy(gameObject);
    }

}
