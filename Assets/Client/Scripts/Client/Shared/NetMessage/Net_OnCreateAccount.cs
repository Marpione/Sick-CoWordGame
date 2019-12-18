using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AccountType { guest, facebook }

[System.Serializable]
public class Net_OnCreateAccount : NetMessage
{
    public Net_OnCreateAccount()
    {
        OP = NetOP.OnCreateAccount;
    }
    public AccountType AccountType { set; get; }
    public byte Success { set; get; }
    public string Information { set; get; }
}
