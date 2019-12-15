﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPanel : Panel
{
    private void OnEnable()
    {
        Client.OnLoginSuccess += HidePanel;
        Client.OnCreateAcountSuccess += HidePanel;
    }

    private void OnDisable()
    {
        Client.OnLoginSuccess -= HidePanel;
        Client.OnCreateAcountSuccess -= HidePanel;
    }

}
