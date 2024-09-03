using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWebURL : MonoBehaviour
{
    public string url = "";

    public void OpenLink () 
    {
        Tooltip.HideTooltip_Static();
        Application.OpenURL(url);
    }
}
