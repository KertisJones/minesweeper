using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetRandomSupporterName : MonoBehaviour
{
    List<string> supporters = new List<string> {
        "Jesse Riggins",
        "Poinl",
        "Haruka",
        "Star Cubey",
        "Peridot",
        "Niv",
        "JMaxchill",
        "Kusane",
        "Icely Puzzles",
        "Random 595",
        "Stickman comic"
    };
    public TextMeshProUGUI supportText;
    GameModifiers gameMods;
    // Start is called before the first frame update
    void Start()
    {
        gameMods = GameObject.FindGameObjectWithTag("ScoreKeeper").GetComponent<GameModifiers>();

        if(gameMods.showCredits && !GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().isTitleMenu)
            this.gameObject.SetActive(true);
        else
            this.gameObject.SetActive(false);
        
        if (supportText != null)
            supportText.text = supporters[UnityEngine.Random.Range(0, supporters.Count)];
    }
}
