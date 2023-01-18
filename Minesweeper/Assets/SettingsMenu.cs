using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public PauseMenuMove pauseMenuMove;
    public Slider masterVolumeSlider;
    float masterVolume = 0.2f;

    private void Awake()
    {
        //masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        //musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        //soundVolume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        //controlScheme = PlayerPrefs.GetInt("ControlScheme", 0);
        //abTest = PlayerPrefs.GetInt("ABTest", 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (ScoreKeeper.masterVolume != null)
            masterVolume = ScoreKeeper.masterVolume;
        masterVolumeSlider.value = masterVolume;
        masterVolumeSlider.onValueChanged.AddListener(delegate { MasterVolumeSlider(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = masterVolume;
            if (pauseMenuMove.isActive)
                masterVolumeSlider.interactable = true;
            else
                masterVolumeSlider.interactable = false;
        }
            
    }

    public void MasterVolumeSlider() // Sets the Master Volume Slider from PlayerPrefs
    {
        masterVolume = masterVolumeSlider.value;
        //PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        ScoreKeeper.masterVolume = masterVolume;
    }
}
