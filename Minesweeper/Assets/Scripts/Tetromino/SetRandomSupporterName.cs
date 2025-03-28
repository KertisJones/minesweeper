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
        "Kertis Jones", // I get two

        "The Foun", // Czech translation
        "Paincake", // Helped Foun translate
        "Emily DieHenne", // German Translation
        "Carlos Hugo García Maldonado", //H3NZ //"H3NZ PLAYZ", // Spanish (Spain) translation
        "Eleanor silly", // French // omerien
        "Draconis Eltanin", // Italian
        "Alexuty", // Screenshot design
        //"Poinl", // Music..... bye bye Poinl.
        "Star Cubey", // Toki Pona, mod
        "ThomasZQY", // Chinese

        //Special Thanks
        // Influencers
        "Icely Puzzles",
        "Random 595",
        "Stickman comic", // Playtesting maniac
        // Moderators
        "jemmr",
        "JMaxchill",
        "Kusane",
        "Niv",
        "Peridot",
        //"Star Cubey",
        
        
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
        //"Draconis Eltanin",
        "Camellias",
        "Silver_Hawk",
        "Cantras",
        "Alien Sauce_"
    };
    public TextMeshProUGUI supportText;
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponentInParent<Tile>() == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
            
        gm = GetComponentInParent<Tile>().gm;
        if (gm == null)
        {
            //this.gameObject.SetActive(false);
            return;
        }

        if (gm.textType == GameModifiers.MinesweeperTextType.credits)// && !GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().isTitleMenu)
            this.gameObject.SetActive(true);
        else
            this.gameObject.SetActive(false);
        
        if (supportText != null)
            supportText.text = supporters[UnityEngine.Random.Range(0, supporters.Count)];        
    }
}
