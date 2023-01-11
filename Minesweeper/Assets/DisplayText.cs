using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayText : MonoBehaviour
{
    GameManager gm;
    ScoreKeeper sk;

    float timer = 0.0f;
    int colorIndex = 0;
    private Color startColor = Color.red;


    public enum TextType // your custom enumeration
    {
        score, 
        minesMissing, 
        minesTotal,
        flagsTotal,
        time, 
        bestScore, 
        linesCleared,
        tetrisweepsCleard
    };
    public TextType displayType;  // t$$anonymous$$s public var should appear as a drop down

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        sk = GameObject.FindGameObjectWithTag("ScoreKeeper").GetComponent<ScoreKeeper>();
    }

    // Update is called once per frame
    void Update()
    {
        if (displayType == TextType.score)
        {
            if (gm.score > 0)
                this.GetComponent<TextMeshProUGUI>().text = "Score: " + gm.score.ToString("#,#");
            else
                this.GetComponent<TextMeshProUGUI>().text = "Score: " + gm.score;
        }
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
            float time = gm.GetTime();
            int seconds = ((int)time % 60);
            int minutes = ((int) time / 60);

            this.GetComponent<TextMeshProUGUI>().text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else if (displayType == TextType.bestScore)
        {
            if (sk.bestScore > 0)
                this.GetComponent<TextMeshProUGUI>().text = "Hi: " + sk.bestScore.ToString("#,#");
            else
                this.GetComponent<TextMeshProUGUI>().text = "Hi: " + sk.bestScore;
            
            if (sk.bestScore <= gm.score && sk.runs > 1 && sk.bestScore > 0)
                this.GetComponent<TMPro.Examples.VertexJitter>().enabled = true;
        }
        else if (displayType == TextType.linesCleared)
        {
            this.GetComponent<TextMeshProUGUI>().text = "Lines: " + gm.linesCleared;
        }
        else if (displayType == TextType.tetrisweepsCleard)
        {
            if (gm.tetrisweepsCleared == 0)
                this.GetComponent<TextMeshProUGUI>().text = "";
            else
            {
                this.GetComponent<TextMeshProUGUI>().text = "Tetrisweeps: " + gm.tetrisweepsCleared;
                this.GetComponent<TMPro.Examples.VertexJitter>().enabled = true;
                this.GetComponent<VertexColorCyclerGradient>().enabled = true;
            }
        }
    }
}
