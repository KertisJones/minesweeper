using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TextOutline : MonoBehaviour
{
    public float outlineWidth = 0.2f;
    public Color color = Color.black;

    public bool startEnabled = true;

    TextMeshProUGUI textmeshPro;
    void Awake()
    {
        textmeshPro = GetComponent<TextMeshProUGUI>();
        if (startEnabled)
            EnableOutline();
    }

    public void EnableOutline()
    {
        textmeshPro.outlineWidth = outlineWidth;
        textmeshPro.outlineColor = color;
    }

    public void DisableOutline()
    {
        textmeshPro.outlineWidth = 0;
    }

}
