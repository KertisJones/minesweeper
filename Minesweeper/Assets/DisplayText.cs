using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayText : MonoBehaviour
{
    GameManager gm;

    public enum TextType // your custom enumeration
    {
        score, 
        minesMissing, 
        minesTotal,
        flagsTotal,
        time
    };
    public TextType displayType;  // t$$anonymous$$s public var should appear as a drop down

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (displayType == TextType.score)
            this.GetComponent<TextMeshProUGUI>().text = "Score: " + gm.score;
        else if (displayType == TextType.minesMissing)
        {
            int currentMines = gm.currentMines;
            int currentFlags = gm.currentFlags;
            int unknownMines = currentMines - currentFlags;
            string suffix = "";
            if (unknownMines < 0)
                suffix = "?";
            this.GetComponent<TextMeshProUGUI>().text = "Mines: " + unknownMines + suffix;
        }
        else if (displayType == TextType.minesTotal)
        {
            this.GetComponent<TextMeshProUGUI>().text = "Mines Total: " + gm.currentMines;
        }
        else if (displayType == TextType.flagsTotal)
        {
            this.GetComponent<TextMeshProUGUI>().text = "Flags Total: " + gm.currentFlags;
        }
        else if (displayType == TextType.time)
        {
            this.GetComponent<TextMeshProUGUI>().text = "Time: " + Time.time;
        }
    }
}
