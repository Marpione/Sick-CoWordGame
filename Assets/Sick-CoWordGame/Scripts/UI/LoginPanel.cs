using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : Panel
{
    public InputField UsernameField;
    public InputField PasswordField;

    private void OnEnable()
    {
        Client.OnLoginSuccess += HidePanel;
    }

    private void OnDisable()
    {
        Client.OnLoginSuccess -= HidePanel;
    }

    public void LogIn()
    {
        Client.Instance.SendLoginRequest(UsernameField.text, PasswordField.text);
    }
}
