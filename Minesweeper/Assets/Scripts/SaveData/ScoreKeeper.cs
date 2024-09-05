using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using GUPS.AntiCheat.Protected;

public class ScoreKeeper : MonoBehaviour, ISaveable
{
    public ProtectedFloat bestScore;
    public ProtectedFloat bestScoreToday = 0;
    public ProtectedFloat bestScoreEndless;
    public ProtectedFloat bestScoreTodayEndless = 0;
    public ProtectedFloat bestTime;
    public ProtectedFloat bestTimeToday;
    public ProtectedInt32 runs;
    //public static float masterVolume  = 0.2f;
    GameManager gm;
    AudioSource musicSource;

    public enum VersionType
    {
        standard,
        beta,
        demoOnline,
        demoSteam
    }
    public static VersionType versionType = VersionType.demoOnline;
    //CameraShake cameraShake;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("ScoreKeeper");
        DOTween.SetTweensCapacity(2000, 100);

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        //masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        ResetScoreKeeper();

        DontDestroyOnLoad(this.gameObject);
    }

    public void ResetScoreKeeper() 
    {
        bestScore = 0;
        bestScoreToday = 0;
        bestScoreEndless = 0;
        bestScoreTodayEndless = 0;
        bestTime = Mathf.Infinity;
        bestTimeToday = Mathf.Infinity;
        runs = 0;
        LoadJsonData(this.GetComponent<ScoreKeeper>());
    }

    // Update is called once per frame
    void Update()
    {
        if (gm == null)
            if (GameObject.FindGameObjectWithTag("GameController") != null)
                gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (musicSource == null)
            musicSource = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();

        if (!gm.isTitleMenu)
        {
            if ((gm.GetScore() > bestScore) && !(gm.isEndless && !gm.marathonOverMenu.GetIsActive())) // Endless mode has not yet begun
            {
                bestScore = gm.GetScore();
            }
            if ((gm.GetScore() > bestScoreToday) && !(gm.isEndless && !gm.marathonOverMenu.GetIsActive())) // Endless mode has not yet begun
            {
                bestScoreToday = gm.GetScore();
            }
            if (gm.GetScore() > bestScoreEndless)
            {
                bestScoreEndless = gm.GetScore();
            }
            if (gm.GetScore() > bestScoreTodayEndless)
            {
                bestScoreTodayEndless = gm.GetScore();
            }
        }

        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        if (musicSource != null)
            musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.25f);
    }

    public void SaveCurrentGame() 
    {
        if (GetComponent<GameModifiers>().gameModeName != "Custom")
            SaveJsonData(this.GetComponent<ScoreKeeper>());
        else
            Debug.Log("Can't save game of type " + GetComponent<GameModifiers>().gameModeName);
        
        // Time updates
        if ((gm.GetTime() < bestTime) && (gm.isEndless && gm.marathonOverMenu.GetIsActive())) // Endless mode has not yet begun
        {
            bestTime = gm.GetTime();            
        }
        if ((gm.GetTime() < bestTimeToday) && (gm.isEndless && gm.marathonOverMenu.GetIsActive())) // Endless mode has not yet begun
        {
            bestTimeToday = gm.GetTime();            
        }
    }

    public void SaveJsonData(ScoreKeeper a_ScoreKeeper) 
    {
        SaveData sd = new SaveData();
        // Get current save data, if it exists
        if (FileManager.LoadFromFile(GetComponent<GameModifiers>().gameModeName + "SaveData.dat", out var json)) //"SaveData.dat"
        {
            sd.LoadFromJson(json);
            Debug.Log("Previous Save Found");
        }

        a_ScoreKeeper.PopulateSaveData(sd);

        if (FileManager.WriteToFile(GetComponent<GameModifiers>().gameModeName + "SaveData.dat", sd.ToJson()))
        {
            Debug.Log("Save successful");
        }
    }

    public void LoadJsonData (ScoreKeeper a_ScoreKeeper) 
    {
        if (FileManager.LoadFromFile(GetComponent<GameModifiers>().gameModeName + "SaveData.dat", out var json)) //"SaveData.dat"
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);

            a_ScoreKeeper.LoadFromSaveData(sd);
            //Debug.Log("Load complete");
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

        if (gm.GetTime() < a_SaveData.m_gameTimeBest && (gm.isEndless && gm.marathonOverMenu.GetIsActive()))
            a_SaveData.m_gameTimeBest = gm.GetTime();
        else if (a_SaveData.m_gameTimeBest == 0 && (gm.isEndless && gm.marathonOverMenu.GetIsActive()))
            a_SaveData.m_gameTimeBest = gm.GetTime();

        a_SaveData.m_gamesPlayedTotal++;
        a_SaveData.m_gameTimeTotal += gm.GetTime();
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


        if (gm.GetScore() >= bestScore 
        || gm.linesCleared >= a_SaveData.m_linesClearedBest 
        || gm.tetrisweepsCleared >= a_SaveData.m_tetrisweepsClearedBest 
        || gm.tSpinsweepsCleared >= a_SaveData.m_tSpinsweepsClearedBest
        || gm.GetTime() <= a_SaveData.m_gameTimeBest)
            a_SaveData.m_GameStatsData.Add(PopulateSaveDataHighScores());
    }

    public SaveData.GameStatsData PopulateSaveDataHighScores()
    {
        SaveData.GameStatsData gameStatsData = new SaveData.GameStatsData();
        gameStatsData.dateTime = System.DateTime.Now;
        gameStatsData.m_score = gm.GetScore();
        gameStatsData.m_isEndless = gm.isEndless && !gm.marathonOverMenu.GetIsActive();
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
        float bestSavedScoreEndless = 0;
        foreach (SaveData.GameStatsData gameStat in a_SaveData.m_GameStatsData)
        {
            if (bestSavedScore < gameStat.m_score && !gameStat.m_isEndless)
                bestSavedScore = gameStat.m_score;
            if (bestSavedScoreEndless < gameStat.m_score)
                bestSavedScoreEndless = gameStat.m_score;
        }
        bestScore = bestSavedScore;
        bestScoreEndless = bestSavedScoreEndless;
        bestTime = a_SaveData.m_gameTimeBest;
    }
}
