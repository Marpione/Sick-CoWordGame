using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(CanvasGroup))]
public class Panel : MonoBehaviour
{
    CanvasGroup canvasGroup;
    CanvasGroup CanvasGroup { get { return (canvasGroup == null) ? canvasGroup = GetComponent<CanvasGroup>() : canvasGroup; } }

    bool status;

    [ButtonGroup("PanelGroup")]
    public void ShowPanel()
    {
        CanvasGroup.alpha = 1;
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
        status = true;
    }

    [ButtonGroup("PanelGroup")]
    public void HidePanel()
    {
        CanvasGroup.alpha = 0;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
        status = false;
    }

    [ButtonGroup("PanelGroup")]
    public void TogglePanel()
    {
        if (status)
            HidePanel();
        else ShowPanel();
    }

}
