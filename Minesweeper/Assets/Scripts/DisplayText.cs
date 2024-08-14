using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEditor;
using DG.Tweening;

public class DisplayText : MonoBehaviour
{
    GameManager gm;
    ScoreKeeper sk;
    GameModifiers gameMods;
    /*float timer = 0.0f;
    int colorIndex = 0;
    private Color startColor = Color.red;*/
    private Color startColor = Color.white;
    private float startFontSize = 12;
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
        TESTCurrentMinoLockDelay,
        TESTCurrentMinoRotation,
        gameModeName,
        gameModeNameComplete,
        versionNumber,
        revealCombo
    };
    public TextType displayType;  // t$$anonymous$$s public var should appear as a drop down

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        sk = gm.scoreKeeper;
        gameMods = gm.gameMods;

        startColor = this.GetComponent<TextMeshProUGUI>().color;
        startFontSize = this.GetComponent<TextMeshProUGUI>().fontSize;

        if (displayType == TextType.versionNumber)
        {
            if (ScoreKeeper.versionType == ScoreKeeper.VersionType.standard)
                this.GetComponent<TextMeshProUGUI>().text = "v" + Application.version;
            else if (ScoreKeeper.versionType == ScoreKeeper.VersionType.beta)
                this.GetComponent<TextMeshProUGUI>().text = "v" + Application.version + " Beta";
            else if (ScoreKeeper.versionType == ScoreKeeper.VersionType.demoOnline)
                this.GetComponent<TextMeshProUGUI>().text = "v" + Application.version + " Demo-B";
            else if (ScoreKeeper.versionType == ScoreKeeper.VersionType.demoSteam)
                this.GetComponent<TextMeshProUGUI>().text = "v" + Application.version + " Demo-A";
        }
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

            string localizedText = GameManager.GetTranslation("UIText", "GUI Mines"); // "Mines"
            this.GetComponent<TextMeshProUGUI>().text = localizedText+ ": " + unknownMines + suffix;
        }
        else if (displayType == TextType.minesTotal)
        {
            /*this.GetComponent<TextMeshProUGUI>().text = "Mines Total: " + gm.currentMines;*/
        }
        else if (displayType == TextType.flagsTotal)
        {
            /*this.GetComponent<TextMeshProUGUI>().text = "Flags Total: " + gm.currentFlags;*/
        }
        else if (displayType == TextType.time)
        {
            string localizedText = ""; // GameManager.GetTranslation("UIText", "GUI Time") + ": "; // "Time"
            if (gm.gameMods.timeLimit < Mathf.Infinity)
                this.GetComponent<TextMeshProUGUI>().text = localizedText + GetTimeString(gm.gameMods.timeLimit - gm.GetTime());
            else
                this.GetComponent<TextMeshProUGUI>().text = localizedText + GetTimeString(gm.GetTime());
        }
        else if (displayType == TextType.bestScore)
        {
            if (!gm.gameMods.detailedTimer) // Normal score mode
            {            
                // Display Best Score Today while your current score is under Best Score Today, unless it's the first run of the game.
                // Otherwise, display your total high score

                /*if (sk.bestScoreToday > 0 && sk.runs > 1 && sk.bestScoreToday > gm.GetScore()) // Best Score Today
                    this.GetComponent<TextMeshProUGUI>().text = sk.bestScoreToday.ToString("#,#");*/
                if (sk.bestScore > 0) // Best Score Total
                    this.GetComponent<TextMeshProUGUI>().text = sk.bestScore.ToString("#,#"); 
                else // Hi Score = 0 
                    this.GetComponent<TextMeshProUGUI>().text = sk.bestScore.ToString();
                
                if (sk.bestScore <= gm.GetScore() && sk.bestScore > 0)
                    this.GetComponent<TMPro.Examples.VertexJitter>().enabled = true;
            }
            else // 40L sprint mode
            {
                string bestTimeStr = GetTimeString(sk.bestTime);
                //string bestTimeTodayStr = GetTimeString(sk.bestTimeToday);
                
                //this.GetComponent<TextMeshProUGUI>().text = bestTimeStr + ", " + bestTimeTodayStr;

                this.GetComponent<TextMeshProUGUI>().text = bestTimeStr;

                /*if (sk.bestTimeToday < Mathf.Infinity && sk.runs > 1 && sk.bestTime < gm.GetTime()) // Best Score Today
                    this.GetComponent<TextMeshProUGUI>().text = bestTimeTodayStr;*/
                
            }
        }
        else if (displayType == TextType.bestScoreTitle)
        {
            string localizedText = GameManager.GetTranslation("UIText", "GUI HighScore"); // "High Score"
            if (!gm.gameMods.detailedTimer || gm.gameMods.timeLimit < Mathf.Infinity) // Normal score mode
            {
                /*if (sk.bestScoreToday > 0 && sk.runs > 1 && sk.bestScoreToday > gm.GetScore()) // Best Score Today
                    localizedText = GameManager.GetTranslation("UIText", "GUI HighScoreBestToday"); // "Best Today"
                
                if (sk.bestScoreToday == sk.bestScore)*/
                localizedText = GameManager.GetTranslation("UIText", "GUI HighScore"); // "High Score"
            }
            else // 40L sprint mode
            {
                localizedText = GameManager.GetTranslation("UIText", "GUI HighScoreBestTime"); // "Best Time"

                /*if (sk.bestTimeToday < Mathf.Infinity && sk.runs > 1 && sk.bestTime < gm.GetTime()) // Best Score Today
                    localizedText = GameManager.GetTranslation("UIText", "GUI HighScoreBestToday"); // "Best Today"*/
            }            
            this.GetComponent<TextMeshProUGUI>().text = localizedText + ""; 
        }
        else if (displayType == TextType.linesCleared)
        {
            //string localizedText = GameManager.GetTranslation("UIText", "GUI Lines"); // "Lines"
            this.GetComponent<TextMeshProUGUI>().text = gm.linesCleared.ToString(); // localizedText + ": " + gm.linesCleared;
        }
        else if (displayType == TextType.tetrisweepsCleard)
        {
            string localizedText = GameManager.GetTranslation("UIText", "GUI Tetrisweeeps"); // "Tetrisweeps"
            if (gm.tetrisweepsCleared == 0)
                this.GetComponent<TextMeshProUGUI>().text = "";
            else
            {
                this.GetComponent<TextMeshProUGUI>().text = localizedText + ": " + gm.tetrisweepsCleared;
                this.GetComponent<TMPro.Examples.VertexJitter>().enabled = true;
                this.GetComponent<VertexColorCyclerGradient>().enabled = true;
            }
        }
        else if (displayType == TextType.quit)
        {
            string localizedText = GameManager.GetTranslation("UIText", "Menu Quit"); // "Quit"
            if (Application.platform == RuntimePlatform.WebGLPlayer && gm.hasQuit)
            {
                localizedText = GameManager.GetTranslation("UIText", "Menu QuitCancel"); // "Can't Quit in Browser"
                this.GetComponent<TextMeshProUGUI>().text = localizedText;
                this.GetComponent<TextMeshProUGUI>().fontSize = 8;
            }                
            else
            {                
                this.GetComponent<TextMeshProUGUI>().text = localizedText;
                this.GetComponent<TextMeshProUGUI>().fontSize = startFontSize;                
            }
                
        }
        else if (displayType == TextType.level)
        {
            //string localizedText = GameManager.GetTranslation("UIText", "GUI Level"); // "Level"
            this.GetComponent<TextMeshProUGUI>().text = gm.level.ToString(); // localizedText + ": " + gm.level;
        }
        else if (displayType == TextType.tSpinSweeps)
        {
            if (gm.tSpinsweepsCleared == 0)
                this.GetComponent<TextMeshProUGUI>().text = "";
            else
            {
                string localizedText = GameManager.GetTranslation("UIText", "GUI Tspinsweeps"); // "T-Spinsweeps"
                this.GetComponent<TextMeshProUGUI>().text = localizedText + ": " + gm.tSpinsweepsCleared;
                this.GetComponent<TMPro.Examples.VertexJitter>().enabled = true;
                this.GetComponent<VertexColorCyclerGradient>().enabled = true;
            }            
        }
        else if (displayType == TextType.TESTCurrentMinoLockDelay)
        {
            if (PlayerPrefs.GetInt("LockDelayDisplayEnabled", 0) == 0)
            {
                this.GetComponent<TextMeshProUGUI>().text = "";
                return;
            }
                
            Group activeTetromino = gm.GetActiveTetromino();
            if (activeTetromino == null)
            {
                this.GetComponent<TextMeshProUGUI>().text = "";
            }
            else
            {
                int resets = 15 - activeTetromino.lockResets;
                //if (resets < 0)
                    //resets = 0;

                if (activeTetromino.lockDelayTimer > 0 && activeTetromino.isLocking)
                    this.GetComponent<TextMeshProUGUI>().text = "Lock: " + activeTetromino.lockDelayTimer.ToString("#,#.#") + ", Resets: " + resets; // TODO I'm gonna remove this maybe?
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
        else if (displayType == TextType.gameModeName)
        {
            string gameMode = gameMods.gameModeName;
            if (gameMods.gameModeDisplayName != "")
                gameMode = gameMods.gameModeDisplayName;
            
            string localizedText = GameManager.GetTranslation("UIText", "GameMode " + gameMode); // Returns translation of game mode name, ex. "Marathon"

            if (gm.isEndless && !gm.marathonOverMenu.GetIsActive())
                localizedText += " (" + GameManager.GetTranslation("UIText", "GameMode Endless") + ")"; // " (Endless)"
            this.GetComponent<TextMeshProUGUI>().text = localizedText;
        }
        else if (displayType == TextType.gameModeNameComplete)
        {
            string gameMode = gameMods.gameModeName;
            if (gameMods.gameModeDisplayName != "")
                gameMode = gameMods.gameModeDisplayName;
            
            string localizedText = GameManager.GetTranslation("UIText", "GameMode " + gameMode); // Returns translation of game mode name, ex. "Marathon"
            
            this.GetComponent<TextMeshProUGUI>().text = localizedText + " " + GameManager.GetTranslation("UIText", "GameMode Complete") + "!";
        }
        else if (displayType == TextType.revealCombo)
        {
            this.GetComponent<TextMeshProUGUI>().text = gm.revealCombo.ToString("#,#.#");

            /*this.GetComponent<TextMeshProUGUI>().color = startColor;
            if (gm.revealComboDrainTween != null)
                if (gm.revealComboDrainTween.IsActive())
                    if (gm.revealComboDrainTween.IsPlaying())
                        this.GetComponent<TextMeshProUGUI>().color = Color.red;*/                           
        }
    }

    public void MouseOverTextEnter()
    {
        if (displayType == TextType.gameModeName)
        {
            string gameMode = gameMods.gameModeName;
            if (gameMods.gameModeDisplayName != "")
                gameMode = gameMods.gameModeDisplayName;
            
            //string localizedText = GameManager.GetTranslation("UIText", "Tooltip " + gameMode); // Returns translation of game mode description, ex. "Marathon"
            Tooltip.ShowTooltip_Static(gameMode);
        }
    }

    /*public void MouseOverTextExit()
    {
        if (displayType == TextType.gameModeName)
            Tooltip.HideTooltip_Static();
    }*/

    public string GetTimeString(float time)
    {
        if (time == Mathf.Infinity)
            return "--";

        int milliseconds = (int)(time * 1000f) % 1000;
        int seconds = ((int)time % 60);
        int minutes = ((int) time / 60);
        
        if (gameMods.detailedTimer)
        {
            return string.Format("{0:0}:{1:00}.{2:000}", minutes, seconds, milliseconds);
        }
        else
        {
            return string.Format("{0:0}:{1:00}", minutes, seconds);
        }            
    }
}
