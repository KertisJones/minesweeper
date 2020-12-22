using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour
{
    public int coordX = 0;
    public int coordY = 0;

    public bool isMine = false;
    public bool isFlagged = false;
    public bool isQuestioned = false;
    public bool isRevealed = false;
    public int nearbyMines = 0;

    TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string myText = "";
        if (isRevealed)
        {
            myText = nearbyMines.ToString();
            if (nearbyMines == 0)
                myText = "";
            if (isMine)
                myText = "M";
        }
        else
        {
            if (isFlagged)
                myText = "!";
            else if (isQuestioned)
                myText = "?";
        }
        


        text.SetText(myText);
    }
}
