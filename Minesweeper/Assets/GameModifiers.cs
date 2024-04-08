using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModifiers : MonoBehaviour
{   
    public string gameModeName = "Marathon";
    public string gameModeDisplayName = "";

    // Game Rules
    public enum LineClearTriggerType // your custom enumeration
    {
        clearOnLock,
        clearInstantly
    };
    public LineClearTriggerType lineClearTrigger = LineClearTriggerType.clearOnLock;
    public int targetLines = 150;
    public bool detailedTimer = false;
    public float timeLimit = Mathf.Infinity;

    // Game Board Setup
    public enum WallType // your custom enumeration
    {
        unlock,
        disabled,
        playable
    };
    public WallType wallType = WallType.unlock;
    
    // Distractions
    public enum MinesweeperTextType // your custom enumeration
    {
        numbers,
        credits,
        dots
    };
    public MinesweeperTextType minesweeperTextType = MinesweeperTextType.numbers;
    public bool showTitle = false;
    
    public void SetGameToThisMode()
    {
        ScoreKeeper scoreKeeper = GameObject.FindGameObjectWithTag("ScoreKeeper").GetComponent<ScoreKeeper>();
        GameModifiers gameMods = scoreKeeper.GetComponent<GameModifiers>();

        gameMods.gameModeName = gameModeName;
        gameMods.gameModeDisplayName = gameModeDisplayName;
        gameMods.lineClearTrigger = lineClearTrigger;
        gameMods.targetLines = targetLines;
        gameMods.detailedTimer = detailedTimer;
        gameMods.timeLimit = timeLimit;

        //Game Board Setup
        gameMods.wallType = wallType;

        // Distractions
        gameMods.minesweeperTextType = minesweeperTextType;
        gameMods.showTitle = showTitle;

        scoreKeeper.ResetScoreKeeper();
    }
}
