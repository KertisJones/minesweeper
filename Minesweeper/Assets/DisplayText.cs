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
        tetrisweepsCleard,
        scoreMultiplier,
        quit
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
            if (gm.GetScore() > 0)
                this.GetComponent<TextMeshProUGUI>().text = gm.GetScore().ToString("#,#");
            else
                this.GetComponent<TextMeshProUGUI>().text = gm.GetScore().ToString();
        }
        else if (displayType == TextType.scoreMultiplier)
        {
            string suffix = "";
            if (gm.scoreMultiplierTimer > 25)
                suffix = "!!!!!";
            else if (gm.scoreMultiplierTimer > 20)
                suffix = "!!!!";
            else if (gm.scoreMultiplierTimer > 15)
                suffix = "!!!";
            else if (gm.scoreMultiplierTimer > 10)
                suffix = "!!";
            else if (gm.scoreMultiplierTimer > 5)
                suffix = "!";
            else if (gm.scoreMultiplierTimer <= 1)
                suffix = "...";
            else if (gm.scoreMultiplierTimer <= 2)
                suffix = "..";
            else if (gm.scoreMultiplierTimer <= 3)
                suffix = "."; 
            
            
            if (gm.scoreMultiplier > 1)
                this.GetComponent<TextMeshProUGUI>().text = "x" + gm.scoreMultiplier + suffix;
            else
                this.GetComponent<TextMeshProUGUI>().text = "";
        }
        else if (displayType == TextType.minesMissing)
        {
            int currentMines = gm.currentMines;
            int currentFlags = gm.currentFlags;
            int unknownMines = currentMines - currentFlags;
            string suffix = " *";
            if (unknownMines < 0)
                suffix = "? *";
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
                this.GetComponent<TextMeshProUGUI>().text = sk.bestScore.ToString("#,#");
            else
                this.GetComponent<TextMeshProUGUI>().text = sk.bestScore.ToString();
            
            if (sk.bestScore <= gm.GetScore() && sk.runs > 1 && sk.bestScore > 0)
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
        else if (displayType == TextType.quit)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer && gm.hasQuit)
                this.GetComponent<TextMeshProUGUI>().text = "Can't Quit in Browser";
            else
                this.GetComponent<TextMeshProUGUI>().text = "Quit";
        }
    }
}
