using GUPS.AntiCheat.Protected;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public struct GameStatsData
    {
        public System.DateTime dateTime;
        public ProtectedFloat m_score;
        public ProtectedBool m_isEndless;
        public ProtectedFloat m_gameTime;
        public ProtectedInt32 m_level;
        public ProtectedInt32 m_linesCleared;
        public ProtectedInt32 m_tetrisweepsCleared;
        public ProtectedInt32 m_tSpinsweepsCleared;
        public ProtectedInt32 m_piecesPlaced;
        public ProtectedInt32 m_holds;
        public ProtectedInt32 m_linesweepsCleared;
        public ProtectedFloat m_highestScoreMultiplier;
        public ProtectedInt32 m_minesSweeped;
        public ProtectedInt32 m_perfectClears;
        public ProtectedInt32 m_singlesFilled;
        public ProtectedInt32 m_doublesFilled;
        public ProtectedInt32 m_triplesFilled;
        public ProtectedInt32 m_tetrisesFilled;
        public ProtectedInt32 m_tSpinMiniNoLines;
        public ProtectedInt32 m_tSpinMiniSingle;
        public ProtectedInt32 m_tSpinMiniDouble;
        public ProtectedInt32 m_tSpinNoLines;
        public ProtectedInt32 m_tSpinSingle;
        public ProtectedInt32 m_tSpinDouble;
        public ProtectedInt32 m_tSpinTriple;
    }
    public ProtectedFloat m_HiScore;
    public ProtectedFloat m_HiScoreEndless;
    public ProtectedInt32 m_linesClearedBest;
    public ProtectedInt32 m_tetrisweepsClearedBest;
    public ProtectedInt32 m_tSpinsweepsClearedBest;
    public ProtectedFloat m_gameTimeBest;

    public ProtectedInt32 m_gamesPlayedTotal;
    public ProtectedFloat m_gameTimeTotal;    
    public ProtectedInt32 m_linesClearedTotal;
    public ProtectedInt32 m_piecesPlacedTotal;
    public ProtectedInt32 m_tetrisweepsClearedTotal;
    public ProtectedInt32 m_tSpinsweepsClearedTotal;
    public ProtectedInt32 m_linesweepsClearedTotal;
    public ProtectedFloat m_highestScoreMultiplierTotal;
    public ProtectedInt32 m_minesSweepedTotal;
    public ProtectedInt32 m_perfectClearsTotal;
    public ProtectedInt32 m_singlesFilledTotal;
    public ProtectedInt32 m_doublesFilledTotal;
    public ProtectedInt32 m_triplesFilledTotal;
    public ProtectedInt32 m_tetrisesFilledTotal;
    public ProtectedInt32 m_tSpinMiniNoLinesTotal;
    public ProtectedInt32 m_tSpinMiniSingleTotal;
    public ProtectedInt32 m_tSpinMiniDoubleTotal;
    public ProtectedInt32 m_tSpinNoLinesTotal;
    public ProtectedInt32 m_tSpinSingleTotal;
    public ProtectedInt32 m_tSpinDoubleTotal;
    public ProtectedInt32 m_tSpinTripleTotal;    

    public List<GameStatsData> m_GameStatsData = new List<GameStatsData>();
    
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json)
    {
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}

public interface ISaveable
{
    void PopulateSaveData(SaveData a_SaveData);
    void LoadFromSaveData(SaveData a_SaveData);
}
