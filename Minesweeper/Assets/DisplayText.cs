using System;
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
        bestScoreTitle,
        linesCleared,
        tetrisweepsCleard,
        scoreMultiplier,
        quit,
        level,
        tSpinSweeps
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
            
            if (sk.bestScoreToday <= gm.GetScore() && sk.runs > 1 && sk.bestScoreToday > 0)
                this.GetComponent<TMPro.Examples.VertexJitter>().enabled = true;
            if (sk.bestScore <= gm.GetScore() && sk.bestScore > 0)
                this.GetComponent<TMPro.Examples.VertexJitter>().enabled = true;
        }
        else if (displayType == TextType.scoreMultiplier)
        {
            /*string suffix = "";
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
                suffix = "."; */
            
            
            if (gm.GetScoreMultiplier() > 0)
                this.GetComponent<TextMeshProUGUI>().text = "x" + (1 + gm.GetScoreMultiplier()); //(Math.Truncate(gm.scoreMultiplier * 100) / 100));
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
            // Display Best Score Today while your current score is under Best Score Today, unless it's the first run of the game.
            // Otherwise, display your total high score

            if (sk.bestScoreToday > 0 && sk.runs > 1 && sk.bestScoreToday > gm.GetScore()) // Best Score Today
                this.GetComponent<TextMeshProUGUI>().text = sk.bestScoreToday.ToString("#,#");
            else if (sk.bestScore > 0) // Best Score Total
                this.GetComponent<TextMeshProUGUI>().text = sk.bestScore.ToString("#,#"); 
            else // Hi Score = 0 
                this.GetComponent<TextMeshProUGUI>().text = sk.bestScore.ToString();
            
            if (sk.bestScore <= gm.GetScore() && sk.bestScore > 0)
                this.GetComponent<TMPro.Examples.VertexJitter>().enabled = true;
        }
        else if (displayType == TextType.bestScoreTitle)
        {
            if (sk.bestScoreToday > 0 && sk.runs > 1 && sk.bestScoreToday > gm.GetScore()) // Best Score Today
                this.GetComponent<TextMeshProUGUI>().text = "Best Today:";
            else if (sk.bestScore > 0) // Best Score Total
                this.GetComponent<TextMeshProUGUI>().text = "High Score:"; 
            
            if (sk.bestScoreToday == sk.bestScore)
                this.GetComponent<TextMeshProUGUI>().text = "High Score:"; 
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
        else if (displayType == TextType.level)
        {
            this.GetComponent<TextMeshProUGUI>().text = "Level: " + gm.level;
        }
        else if (displayType == TextType.tSpinSweeps)
        {
            if (gm.tSpinsweepsCleared == 0)
                this.GetComponent<TextMeshProUGUI>().text = "T-Spinsweeps";
            else
            {
                this.GetComponent<TextMeshProUGUI>().text = "T-Spinsweeps: " + gm.tSpinsweepsCleared;
                this.GetComponent<TMPro.Examples.VertexJitter>().enabled = true;
                this.GetComponent<VertexColorCyclerGradient>().enabled = true;
            }            
        }
    }
}
