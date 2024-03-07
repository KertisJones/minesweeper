using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModifiers : MonoBehaviour
{   
    public string gameModeName = "Marathon";
    public string gameModeDisplayName = "";
    public enum LineClearTriggerType // your custom enumeration
    {
        clearOnLock,
        clearInstantly
    };
    public LineClearTriggerType lineClearTrigger = LineClearTriggerType.clearOnLock;
    public int targetLines = 150;
    public bool detailedTimer = false;
    public float timeLimit = Mathf.Infinity;
    
    // Distractions
    public bool showCredits = false;
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
        // Distractions
        gameMods.showCredits = showCredits;
        gameMods.showTitle = showTitle;

        scoreKeeper.ResetScoreKeeper();
    }
}
