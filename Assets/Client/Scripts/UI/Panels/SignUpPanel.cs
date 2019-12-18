using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPanel : Panel
{
    public InputField UsernameField;
    public InputField EmailField;
    public InputField PasswordField;
    public Text InfoText;

    private void OnEnable()
    {
        Client.OnCreateAcountSuccess += HidePanel;
        Client.OnCreateAcountSuccess += AutoLogin;
    }

    private void OnDisable()
    {
        Client.OnCreateAcountSuccess -= HidePanel;
        Client.OnCreateAcountSuccess -= AutoLogin;
    }

    public void SignUp()
    {
        Client.Instance.SendCreateAccount(UsernameField.text, PasswordField.text, EmailField.text);
    }

    void AutoLogin(NetMessage netMessage)
    {
        Client.Instance.SendLoginRequest(EmailField.text, PasswordField.text);
    }
}
