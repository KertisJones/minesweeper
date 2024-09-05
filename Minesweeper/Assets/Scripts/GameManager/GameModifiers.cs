using GUPS.AntiCheat.Protected;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModifiers : MonoBehaviour
{   
    public ProtectedString gameModeName = "Marathon";
    public ProtectedString gameModeDisplayName = "";

    // Game Rules
    public enum LineClearTriggerType // your custom enumeration
    {
        clearOnLock,
        clearInstantly
    };
    public LineClearTriggerType lineClearTrigger = LineClearTriggerType.clearOnLock;
    public ProtectedInt32 targetLines = 150;
    public ProtectedInt32 previewCount = 5;
    public ProtectedInt32 basicFallDistance = 1;
    public ProtectedBool detailedTimer = false;
    public ProtectedFloat timeLimit = Mathf.Infinity;
    public ProtectedBool endlessIsEnabled = true;    

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
    public ProtectedInt32 auraNormalWeight = 10;
    public ProtectedInt32 auraBurningWeight = 0;
    public ProtectedInt32 auraFrozenWeight = 0;
    public ProtectedInt32 auraWetWeight = 0;
    public ProtectedInt32 auraElectricWeight = 0;
    public ProtectedInt32 auraPlantWeight = 0;
    public ProtectedInt32 auraSandWeight = 0;
    public ProtectedInt32 auraGlassWeight = 0;
    public ProtectedInt32 auraInfectedWeight = 0;

    public Tile.AuraType floorAndWallAura = Tile.AuraType.normal;



    // Distractions
    public enum MinesweeperTextType // your custom enumeration
    {
        numbers,
        credits,
        dots
    };
    public MinesweeperTextType minesweeperTextType = MinesweeperTextType.numbers;
    public ProtectedBool showTitle = false;
    

    
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
