using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetRandomSupporterName : MonoBehaviour
{
    List<string> supporters = new List<string> {
        "Kertis Jones",
        "Haruka",
        "Star Cubey",
        "Peridot",
        "Niv",
        "JMax Chill",
        "Kusane",
        "Icely Puzzles",
        "Random 595",
        "Stickman Comic"
    };
    
    bool showText = true;
    public TextMeshProUGUI supportText;
    // Start is called before the first frame update
    void Start()
    {
        if (!showText)
            this.gameObject.SetActive(false);
        
        if (supportText != null)
            supportText.text = supporters[UnityEngine.Random.Range(0, supporters.Count)];

    }
}
