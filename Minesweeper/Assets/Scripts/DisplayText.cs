using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayText : MonoBehaviour
{
    GameManager gm;
    ScoreKeeper sk;
    /*float timer = 0.0f;
    int colorIndex = 0;
    private Color startColor = Color.red;*/
    private Color startColor = Color.white;
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
        tSpinSweeps,
        TESTCurrentMinoLockDelay
        ,
        TESTCurrentMinoRotation
    };
    public TextType displayType;  // t$$anonymous$$s public var should appear as a drop down

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        sk = GameObject.FindGameObjectWithTag("ScoreKeeper").GetComponent<ScoreKeeper>();
        startColor = this.GetComponent<TextMeshProUGUI>().color;
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
            string suffix = "";


            if (gm.scoreMultiplierTimer >= 25)
                suffix = "!!!!!";
            else if (gm.scoreMultiplierTimer >= 20)
                suffix = "!!!!";
            else if (gm.scoreMultiplierTimer >= 15)
                suffix = "!!!";
            else if (gm.scoreMultiplierTimer >= 10)
                suffix = "!!";
            else if (gm.scoreMultiplierTimer >= 5)
                suffix = "!";
            else if (gm.scoreMultiplierTimer <= 0)
                suffix = "";
            else if (gm.scoreMultiplierTimer <= 1)
                suffix = "...";
            else if (gm.scoreMultiplierTimer <= 2)
                suffix = "..";
            else if (gm.scoreMultiplierTimer <= 3)
                suffix = ".";
            
            
            string scoreStr = (100 + gm.scoreMultiplier).ToString();
            string scoreStrFront = scoreStr.Substring(0, scoreStr.Length - 2);
            string scoreStrBack = "";
            string scoreStrTenths = scoreStr.Substring(scoreStr.Length - 2, 1);
            string scoreStrHundredths = scoreStr.Substring(scoreStr.Length - 1);
            if (scoreStrTenths != "0" && scoreStrHundredths == "0")
                scoreStrBack = "." + scoreStrTenths;
            else if (scoreStrHundredths != "0")
                scoreStrBack = "." + scoreStrTenths + scoreStrHundredths;
            if (gm.GetScoreMultiplier() > 0)
                this.GetComponent<TextMeshProUGUI>().text = "x" + scoreStrFront + scoreStrBack + suffix; //(Math.Truncate(gm.scoreMultiplier * 100) / 100));
            else
                this.GetComponent<TextMeshProUGUI>().text = "";

            if (gm.scoreMultiplier >= gm.scoreMultiplierLimit)
                this.GetComponent<VertexColorCyclerGradient>().enabled = true;
            else
            {
                this.GetComponent<VertexColorCyclerGradient>().enabled = false;
                if (gm.scoreMultiplierTimer <= 0)
                    this.GetComponent<TextMeshProUGUI>().color = Color.red;
                else
                    this.GetComponent<TextMeshProUGUI>().color = startColor;
            }
                
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
                this.GetComponent<TextMeshProUGUI>().text = "";
            else
            {
                this.GetComponent<TextMeshProUGUI>().text = "T-Spinsweeps: " + gm.tSpinsweepsCleared;
                this.GetComponent<TMPro.Examples.VertexJitter>().enabled = true;
                this.GetComponent<VertexColorCyclerGradient>().enabled = true;
            }            
        }
        else if (displayType == TextType.TESTCurrentMinoLockDelay)
        {
            Group activeTetromino = gm.GetActiveTetromino();
            if (activeTetromino == null)
            {
                this.GetComponent<TextMeshProUGUI>().text = "Null";
            }
            else
            {
                int resets = 15 - activeTetromino.lockResets;
                //if (resets < 0)
                    //resets = 0;

                if (activeTetromino.lockDelayTimer > 0 && activeTetromino.isLocking)
                    this.GetComponent<TextMeshProUGUI>().text = "Lock: " + activeTetromino.lockDelayTimer.ToString("#,#.#") + ", Resets: " + resets;
                else
                    this.GetComponent<TextMeshProUGUI>().text = "Lock: 0.5" + ", Resets: " + resets;
            }
        }
        else if (displayType == TextType.TESTCurrentMinoRotation)
        {
            Group activeTetromino = gm.GetActiveTetromino();
            if (activeTetromino == null)
            {
                this.GetComponent<TextMeshProUGUI>().text = "Null";
            }
            else
            {                
                // 0 = spawn state, 1 = counter-clockwise rotation from spawn, 2 = 2 successive rotations from spawn, 3 = clockwise rotation from spawn
                switch (activeTetromino.currentRotation) {
                    case 0:
                        this.GetComponent<TextMeshProUGUI>().text = "0: spawn state";
                        break;
                    case 1:
                        this.GetComponent<TextMeshProUGUI>().text = "1: counter-clockwise";
                        break;
                    case 2:
                        this.GetComponent<TextMeshProUGUI>().text = "2: Upside-down";
                        break;
                    case 3:
                        this.GetComponent<TextMeshProUGUI>().text = "3: clockwise";
                        break;
                    default :
                        this.GetComponent<TextMeshProUGUI>().text = "?: no rotation found?";
                        break;
                }
            }
        }
    }
}
