using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour, ISaveable
{
    public float bestScore;
    public int runs;
    public static float masterVolume  = 0.2f;
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
            SaveJsonData(this.GetComponent<ScoreKeeper>());
        }
            
        AudioListener.volume = masterVolume;
        
        /*if (GameObject.FindGameObjectWithTag("Volume") != null)
            AudioListener.volume = GameObject.FindGameObjectWithTag("Volume").GetComponent<Slider>().value;*/
    }

    public void SaveJsonData(ScoreKeeper a_ScoreKeepeer) 
    {
        SaveData sd = new SaveData();
        a_ScoreKeepeer.PopulateSaveData(sd);

        if (FileManager.WriteToFile("SaveData.dat", sd.ToJson()))
        {
            Debug.Log("Save successful");
        }
    }

    public void LoadJsonData (ScoreKeeper a_ScoreKeeper) {
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
        SaveData.GameStatsData gameStatsData = new SaveData.GameStatsData();
        gameStatsData.m_score = gm.GetScore();
        gameStatsData.dateTime = System.DateTime.Now; // TODO
        a_SaveData.m_GameStatsData.Add(gameStatsData);
    }
    public void LoadFromSaveData (SaveData a_SaveData) {
        float bestSavedScore = 0;
        foreach (SaveData.GameStatsData gameStat in a_SaveData.m_GameStatsData)
        {
            if (bestSavedScore < gameStat.m_score)
                bestSavedScore = gameStat.m_score;
        }
        bestScore = bestSavedScore;
    }
}
