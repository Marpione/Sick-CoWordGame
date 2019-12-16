
[System.Serializable]
public class Net_OnAddFriend : NetMessage
{
    public Net_OnAddFriend()
    {
        OP = NetOP.OnAddFriend;
    }
    public byte Success { get; set; }
    public Account FriendAccount { set; get; }
}
