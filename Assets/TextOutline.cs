using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TextOutline : MonoBehaviour
{
    public float outlineWidth = 0.2f;
    public Color color = Color.black;
    void Awake()
    {
        TextMeshProUGUI textmeshPro = GetComponent<TextMeshProUGUI>();
        textmeshPro.outlineWidth = outlineWidth;
        textmeshPro.outlineColor = color;
    }

}
