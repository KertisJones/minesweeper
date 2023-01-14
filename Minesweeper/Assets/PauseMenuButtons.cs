using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenuButtons : MonoBehaviour {

    private GameManager gm;

    public enum OptionType {resume, options, statistics, credits, quit, returnToPause};
    public OptionType optionType;

    //public GameObject persistentGameObject;

    public PauseMenuMove pauseMenu;
    public PauseMenuMove statisticsMenu;

    // Use this for initialization
    void Start () {
        if (gm == null)
        {
            gm = GameObject.FindObjectOfType<GameManager>();
        }

        //persistentGameObject = GameObject.FindGameObjectWithTag("Persistent Object");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (optionType == OptionType.resume)
        {
            gm.Pause(false);
        }
        else if (optionType == OptionType.options)
        {

        }
        else if (optionType == OptionType.statistics)
        {
            pauseMenu.isActive = false;
            statisticsMenu.isActive = true;
        }
        else if (optionType == OptionType.returnToPause)
        {
            pauseMenu.isActive = true;
            statisticsMenu.isActive = false;
        }
        /*else if (optionType == OptionType.credits)
        {
            Time.timeScale = 1;
            StartCoroutine(ViewCredits());
        }*/
        else if (optionType == OptionType.quit)
        {
            Application.Quit();
        }
    }

    /*public IEnumerator ViewCredits()
    {

        //float fadeTime = 
        GameObject.Find("_GM").GetComponent<Fading>().BeginFade(1);

        //fade the music out
        if (GameObject.Find("$MusicManager").gameObject.GetComponent<CrossfadeOnTrigger>() != null)
        {
            GameObject.Find("$MusicManager").gameObject.GetComponent<CrossfadeOnTrigger>().triggerMusic = true;
            GameObject.Find("$MusicManager").gameObject.GetComponent<CrossfadeOnTrigger>().currentTrack = 1;
            GameObject.Find("$MusicManager").gameObject.GetComponent<CrossfadeOnTrigger>().fadeTime = 1;
        }

        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        persistentGameObject.GetComponent<PersistentScript>().returnScene = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene("Credits");
        //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<LoadNewScene>().sceneToLoad = SceneManager.GetActiveScene().name;
    }*/
}
