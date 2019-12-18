
[System.Serializable]
public class Account
{
    public AccountType AccountType { set; get; }
    public int ActiveconnectionStatus { get; set; }
    public string Username { set; get; }
    public string Discriminator { set; get; }
    public byte Status { set; get; }
}
