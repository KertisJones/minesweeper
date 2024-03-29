﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    GameManager gm;
    public PauseMenuMove pauseMenuMove;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider soundVolumeSlider;
    public Toggle screenShakeToggle;
    public Toggle lockDelayDisplayToggle;
    float masterVolume = 0.4f; //Max 0.8
    float musicVolume = 0.25f; //Max 0.5
    float soundVolume = 0.5f; //Max 1
    bool screenShakeEnabled = true;
    bool lockDelayDisplayEnabled = true;

    private void Awake()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", masterVolume);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", musicVolume);
        soundVolume = PlayerPrefs.GetFloat("SoundVolume", soundVolume);
        screenShakeEnabled = (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) != 0);
        lockDelayDisplayEnabled = (PlayerPrefs.GetInt("LockDelayDisplayEnabled", 0) != 0);
        //controlScheme = PlayerPrefs.GetInt("ControlScheme", 0);
        //abTest = PlayerPrefs.GetInt("ABTest", 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        //if (ScoreKeeper.masterVolume != null)
            //masterVolume = ScoreKeeper.masterVolume;
        masterVolumeSlider.value = masterVolume;
        masterVolumeSlider.onValueChanged.AddListener(delegate { MasterVolumeSlider(); });
        musicVolumeSlider.value = musicVolume;
        musicVolumeSlider.onValueChanged.AddListener(delegate { MusicVolumeSlider(); });
        soundVolumeSlider.value = soundVolume;
        soundVolumeSlider.onValueChanged.AddListener(delegate { SoundVolumeSlider(); });
        screenShakeToggle.isOn = !screenShakeEnabled;
        screenShakeToggle.onValueChanged.AddListener(delegate  { ScreenShakeToggle(); });
        lockDelayDisplayToggle.isOn = lockDelayDisplayEnabled;
        lockDelayDisplayToggle.onValueChanged.AddListener(delegate  { LockDelayDisplayToggle(); });
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
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = musicVolume;
            if (pauseMenuMove.isActive)
                musicVolumeSlider.interactable = true;
            else
                musicVolumeSlider.interactable = false;
        }
        if (soundVolumeSlider != null)
        {
            soundVolumeSlider.value = soundVolume;
            if (pauseMenuMove.isActive)
                soundVolumeSlider.interactable = true;
            else
                soundVolumeSlider.interactable = false;
        }
        if (screenShakeToggle != null)
        {
            screenShakeToggle.isOn = !screenShakeEnabled;
            if (pauseMenuMove.isActive)
                screenShakeToggle.interactable = true;
            else
                screenShakeToggle.interactable = false;
        }
        if (lockDelayDisplayToggle != null)
        {
            lockDelayDisplayToggle.isOn = lockDelayDisplayEnabled;
            if (pauseMenuMove.isActive)
                lockDelayDisplayToggle.interactable = true;
            else
                lockDelayDisplayToggle.interactable = false;
        }
    }

    public void MasterVolumeSlider() // Sets the Master Volume Slider from PlayerPrefs
    {
        masterVolume = masterVolumeSlider.value;
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        //ScoreKeeper.masterVolume = masterVolume;
    }
    public void MusicVolumeSlider() // Sets the Music Volume Slider from PlayerPrefs
    {
        musicVolume = musicVolumeSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }
    public void SoundVolumeSlider() // Sets the SFX Volume Slider from PlayerPrefs
    {
        soundVolume = soundVolumeSlider.value;
        PlayerPrefs.SetFloat("SoundVolume", soundVolume);
    }
    public void ScreenShakeToggle() // Sets the Screen Shake from PlayerPrefs
    {
        screenShakeEnabled = !screenShakeToggle.isOn;
        PlayerPrefs.SetInt("ScreenShakeEnabled", (screenShakeEnabled ? 1 : 0));
    }
    public void LockDelayDisplayToggle() // Sets the Lock Delay Display from PlayerPrefs
    {
        lockDelayDisplayEnabled = lockDelayDisplayToggle.isOn;
        PlayerPrefs.SetInt("LockDelayDisplayEnabled", (lockDelayDisplayEnabled ? 1 : 0));
    }

    public void HoverMusicEnter()
    {
        
        gm.soundManager.EnablePauseFilter();
    }

    public void HoverMusicExit()
    {
        gm.soundManager.DisablePauseFilter();
    }
}
