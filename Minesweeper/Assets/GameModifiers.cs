using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModifiers : MonoBehaviour
{   
    public string gameModeName = "Marathon";
    public enum LineClearTriggerType // your custom enumeration
    {
        clearOnLock,
        clearInstantly
    };
    public LineClearTriggerType lineClearTrigger = LineClearTriggerType.clearOnLock;
    public int targetLines = 150;
    public bool detailedTimer = false;
    
    public void SetGameToThisMode()
    {
        GameModifiers gameMods = GameObject.FindGameObjectWithTag("ScoreKeeper").GetComponent<GameModifiers>();

        gameMods.gameModeName = gameModeName;
        gameMods.lineClearTrigger = lineClearTrigger;
        gameMods.targetLines = targetLines;
        gameMods.detailedTimer = detailedTimer;
    }
}