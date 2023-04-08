using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public struct GameStatsData
    {
        public GameManager.GameModeType gameModeType;
        public System.DateTime dateTime;
        public float m_score;
        public float m_gameTime;
        public int m_level;
        public int m_linesCleared;
        public int m_tetrisweepsCleared;
        public int m_tSpinsweepsCleared;
        public int m_piecesPlaced;
        public int m_holds;
        public int m_linesweepsCleared;
        public float m_highestScoreMultiplier;
        public int m_minesSweeped;
        public int m_perfectClears;
        public int m_singlesFilled;
        public int m_doublesFilled;
        public int m_triplesFilled;
        public int m_tetrisesFilled;
        public int m_tSpinMiniNoLines;
        public int m_tSpinMiniSingle;
        public int m_tSpinMiniDouble;
        public int m_tSpinNoLines;
        public int m_tSpinSingle;
        public int m_tSpinDouble;
        public int m_tSpinTriple;
    }
    
    public float m_HiScore;
    public int m_linesClearedBest;
    public int m_tetrisweepsClearedBest;
    public int m_tSpinsweepsClearedBest;

    public float m_gameTimeTotal;
    public int m_linesClearedTotal;
    public int m_piecesPlacedTotal;
    public int m_tetrisweepsClearedTotal;
    public int m_tSpinsweepsClearedTotal;
    public int m_linesweepsClearedTotal;
    public float m_highestScoreMultiplierTotal;
    public int m_minesSweepedTotal;
    public int m_perfectClearsTotal;
    public int m_singlesFilledTotal;
    public int m_doublesFilledTotal;
    public int m_triplesFilledTotal;
    public int m_tetrisesFilledTotal;
    public int m_tSpinMiniNoLinesTotal;
    public int m_tSpinMiniSingleTotal;
    public int m_tSpinMiniDoubleTotal;
    public int m_tSpinNoLinesTotal;
    public int m_tSpinSingleTotal;
    public int m_tSpinDoubleTotal;
    public int m_tSpinTripleTotal;    

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
