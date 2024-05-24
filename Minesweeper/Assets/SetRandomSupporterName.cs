using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetRandomSupporterName : MonoBehaviour
{
    List<string> supporters = new List<string> {
        // Development
        "Jesse Riggins",

        "The Foun", // Czech translation        
        "Carlos Hugo García Maldonado", //H3NZ //"H3NZ PLAYZ", // Spanish (Spain) translation
        "Alexuty", // Screenshot design
        "Poinl", // Music

        //Special Thanks
        // Influencers
        "Icely Puzzles",
        "Random 595",
        "Stickman comic", // Playtesting maniac
        // Moderators
        "JMaxchill",
        "Kusane",
        "Niv",
        "Peridot",
        "Star Cubey",
        
        
        // Buy Me a Coffee Supporters:
        "Haruka",
        // Itch Supporters:
        "Gueszst",
        "Toaran",
        "Realiste",
        "Emily Whetstine",
        "Seraphina Nix",
        "Lewmas Fain",
        "Dfirebug",
        "Dyst!",
        "Circadian Izzy",
        "Ænthroppe",
        "Sam Figg",
        "Lazuli Quetzal",
        "Maura",
        "Bio Hammer",
        "Draconis Eltanin",
        "Camellias",
        "Silver_Hawk",
        "Cantras"
    };
    public TextMeshProUGUI supportText;
    GameModifiers gameMods;
    // Start is called before the first frame update
    void Start()
    {
        gameMods = GameObject.FindGameObjectWithTag("ScoreKeeper").GetComponent<GameModifiers>();

        if(gameMods.minesweeperTextType == GameModifiers.MinesweeperTextType.credits && !GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().isTitleMenu)
            this.gameObject.SetActive(true);
        else
            this.gameObject.SetActive(false);
        
        if (supportText != null)
            supportText.text = supporters[UnityEngine.Random.Range(0, supporters.Count)];
    }
}
