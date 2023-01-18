using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
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

        DontDestroyOnLoad(this.gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        if (gm == null)
            gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (gm.GetScore() > bestScore)
            bestScore = gm.GetScore();
        AudioListener.volume = masterVolume;
        /*if (GameObject.FindGameObjectWithTag("Volume") != null)
            AudioListener.volume = GameObject.FindGameObjectWithTag("Volume").GetComponent<Slider>().value;*/
    }
}
