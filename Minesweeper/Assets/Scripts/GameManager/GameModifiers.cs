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
    public int previewCount = 5;
    public int basicFallDistance = 1;
    public bool detailedTimer = false;
    public float timeLimit = Mathf.Infinity;
    public bool endlessIsEnabled = true;    

    // Game Board Setup
    public Vector2 boardSize = new Vector2 (10, 20);
    public enum WallType // your custom enumeration
    {
        unlock,
        disabled,
        playable
    };
    public WallType wallType = WallType.unlock;

    // Auras
    public int auraNormalWeight = 10;
    public int auraBurningWeight = 0;
    public int auraFrozenWeight = 0;
    public int auraWetWeight = 0;
    public int auraElectricWeight = 0;
    public int auraPlantWeight = 0;
    public int auraSandWeight = 0;
    public int auraGlassWeight = 0;
    public int auraInfectedWeight = 0;

    public Tile.AuraType floorAndWallAura = Tile.AuraType.normal;



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
        gameMods.previewCount = previewCount;
        gameMods.basicFallDistance = basicFallDistance;
        gameMods.detailedTimer = detailedTimer;
        gameMods.timeLimit = timeLimit;
        gameMods.endlessIsEnabled = endlessIsEnabled;

        //Game Board Setup
        gameMods.boardSize = boardSize;
        gameMods.wallType = wallType;

        // Auras
        gameMods.auraNormalWeight = auraNormalWeight;
        gameMods.auraBurningWeight = auraBurningWeight;
        gameMods.auraFrozenWeight = auraFrozenWeight;
        gameMods.auraWetWeight = auraWetWeight;
        gameMods.auraElectricWeight = auraElectricWeight;
        gameMods.auraPlantWeight = auraPlantWeight;
        gameMods.auraSandWeight = auraSandWeight;
        gameMods.auraGlassWeight = auraGlassWeight;
        gameMods.auraInfectedWeight = auraInfectedWeight;

        gameMods.floorAndWallAura = floorAndWallAura;

        // Distractions
        gameMods.minesweeperTextType = minesweeperTextType;
        gameMods.showTitle = showTitle;

        scoreKeeper.ResetScoreKeeper();
    }
}
