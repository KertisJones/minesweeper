using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour, ISaveable
{
    public float bestScore;
    public float bestScoreToday = 0;
    public int runs;
    //public static float masterVolume  = 0.2f;
    GameManager gm;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("ScoreKeeper");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        //masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        LoadJsonData(this.GetComponent<ScoreKeeper>());

        DontDestroyOnLoad(this.gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        if (gm == null)
            gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (gm.GetScore() > bestScore)
        {
            bestScore = gm.GetScore();            
        }
        if (gm.GetScore() > bestScoreToday)
        {
            bestScoreToday = gm.GetScore();            
        }
            
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        
        /*if (GameObject.FindGameObjectWithTag("Volume") != null)
            AudioListener.volume = GameObject.FindGameObjectWithTag("Volume").GetComponent<Slider>().value;*/
    }

    public void SaveCurrentGame() 
    {
        SaveJsonData(this.GetComponent<ScoreKeeper>());
    }

    public void SaveJsonData(ScoreKeeper a_ScoreKeeper) 
    {
        SaveData sd = new SaveData();
        // Get current save data, if it exists
        if (FileManager.LoadFromFile("SaveData.dat", out var json))
        {
            sd.LoadFromJson(json);
            Debug.Log("Previous Save Found");
        }

        a_ScoreKeeper.PopulateSaveData(sd);

        if (FileManager.WriteToFile("SaveData.dat", sd.ToJson()))
        {
            Debug.Log("Save successful");
        }
    }

    public void LoadJsonData (ScoreKeeper a_ScoreKeeper) 
    {
        if (FileManager.LoadFromFile("SaveData.dat", out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);

            a_ScoreKeeper.LoadFromSaveData(sd);
            Debug.Log("Load complete");
        }
    }

    public void PopulateSaveData(SaveData a_SaveData)
    {
        /*
          
        */
        a_SaveData.m_HiScore = bestScore;
        if (gm.linesCleared > a_SaveData.m_linesClearedBest)
            a_SaveData.m_linesClearedBest = gm.linesCleared;
        if (gm.tetrisweepsCleared > a_SaveData.m_tetrisweepsClearedBest)
            a_SaveData.m_tetrisweepsClearedBest = gm.tetrisweepsCleared;
        if (gm.tSpinsweepsCleared > a_SaveData.m_tSpinsweepsClearedBest)
            a_SaveData.m_tSpinsweepsClearedBest = gm.tSpinsweepsCleared;

        a_SaveData.m_gameTimeTotal++;
        a_SaveData.m_linesClearedTotal += gm.linesCleared;

        a_SaveData.m_piecesPlacedTotal += gm.piecesPlaced;
        a_SaveData.m_tetrisweepsClearedTotal += gm.tetrisweepsCleared;
        a_SaveData.m_tSpinsweepsClearedTotal += gm.tSpinsweepsCleared;
        a_SaveData.m_linesweepsClearedTotal += gm.linesweepsCleared;
        a_SaveData.m_highestScoreMultiplierTotal += gm.highestScoreMultiplier;
        a_SaveData.m_minesSweepedTotal += gm.minesSweeped;
        a_SaveData.m_perfectClearsTotal += gm.perfectClears;
        a_SaveData.m_singlesFilledTotal += gm.singlesFilled;
        a_SaveData.m_doublesFilledTotal += gm.doublesFilled;
        a_SaveData.m_triplesFilledTotal += gm.triplesFilled;
        a_SaveData.m_tetrisesFilledTotal += gm.tetrisesFilled;
        a_SaveData.m_tSpinMiniNoLinesTotal += gm.tSpinMiniNoLines;
        a_SaveData.m_tSpinMiniSingleTotal += gm.tSpinMiniSingle;
        a_SaveData.m_tSpinMiniDoubleTotal += gm.tSpinMiniDouble;
        a_SaveData.m_tSpinNoLinesTotal += gm.tSpinNoLines;
        a_SaveData.m_tSpinSingleTotal += gm.tSpinSingle;
        a_SaveData.m_tSpinDoubleTotal += gm.tSpinDouble;
        a_SaveData.m_tSpinTripleTotal += gm.tSpinTriple;  


        if (gm.GetScore() >= bestScore || gm.linesCleared >= a_SaveData.m_linesClearedBest || gm.tetrisweepsCleared >= a_SaveData.m_tetrisweepsClearedBest || gm.tSpinsweepsCleared >= a_SaveData.m_tSpinsweepsClearedBest)
            a_SaveData.m_GameStatsData.Add(PopulateSaveDataHighScores());
    }

    public SaveData.GameStatsData PopulateSaveDataHighScores()
    {
        SaveData.GameStatsData gameStatsData = new SaveData.GameStatsData();
        gameStatsData.gameModeType = gm.gameModeType;
        gameStatsData.dateTime = System.DateTime.Now;
        gameStatsData.m_score = gm.GetScore();
        gameStatsData.m_gameTime = gm.GetTime();
        gameStatsData.m_level = gm.level;
        gameStatsData.m_linesCleared = gm.linesCleared;
        gameStatsData.m_tetrisweepsCleared = gm.tetrisweepsCleared;
        gameStatsData.m_tSpinsweepsCleared = gm.tSpinsweepsCleared;

        gameStatsData.m_piecesPlaced = gm.piecesPlaced;
        gameStatsData.m_holds = gm.holds;
        gameStatsData.m_linesweepsCleared = gm.linesweepsCleared;
        gameStatsData.m_highestScoreMultiplier = gm.highestScoreMultiplier;
        gameStatsData.m_minesSweeped = gm.minesSweeped;
        gameStatsData.m_perfectClears = gm.perfectClears;
        gameStatsData.m_singlesFilled = gm.singlesFilled;
        gameStatsData.m_doublesFilled = gm.doublesFilled;
        gameStatsData.m_triplesFilled = gm.triplesFilled;
        gameStatsData.m_tetrisesFilled = gm.tetrisesFilled;
        gameStatsData.m_tSpinMiniNoLines = gm.tSpinMiniNoLines;
        gameStatsData.m_tSpinMiniSingle = gm.tSpinMiniSingle;
        gameStatsData.m_tSpinMiniDouble = gm.tSpinMiniDouble;
        gameStatsData.m_tSpinNoLines = gm.tSpinNoLines;
        gameStatsData.m_tSpinSingle = gm.tSpinSingle;
        gameStatsData.m_tSpinDouble = gm.tSpinDouble;
        gameStatsData.m_tSpinTriple = gm.tSpinTriple;
        return gameStatsData;
        
    }

    public void LoadFromSaveData (SaveData a_SaveData) 
    {
        float bestSavedScore = 0;
        foreach (SaveData.GameStatsData gameStat in a_SaveData.m_GameStatsData)
        {
            if (bestSavedScore < gameStat.m_score)
                bestSavedScore = gameStat.m_score;
        }
        bestScore = bestSavedScore;
    }
}
