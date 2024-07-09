using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    GameManager gm;
    public PauseMenuMove pauseMenuMove;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider soundVolumeSlider;
    public Toggle screenShakeToggle;
    public Toggle lockDelayDisplayToggle;
    public TMP_Dropdown languageDropdown;
    public LinearRangeSlider autoRepeatRateSlider;
    public LinearRangeSlider delayedAutoShiftSlider;
    public LinearRangeSlider dasCutDelaySlider;
    public LinearRangeSlider softDropFactorSlider;
    public LinearRangeSlider lineClearPreventMinesweepDelaySlider;
    float masterVolume = 0.5f; //Max 0.5
    float musicVolume = 0.25f; //Max 0.5
    float soundVolume = 0.5f; //Max 1
    bool screenShakeEnabled = true;
    bool lockDelayDisplayEnabled = true;
    int languageIndex = 0;

    //Handling
    float autoRepeatRateDefault = 50;
    float delayedAutoShiftDefault = 250;
    float dasCutDelayDefault = 17;
    float softDropFactorDefault = 12;
    float lineClearPreventMinesweepDelayDefault = 50;
    float autoRepeatRate = 50;
    float delayedAutoShift = 250;
    float dasCutDelay = 17;
    float softDropFactor = 12;
    float lineClearPreventMinesweepDelay = 50;

    private void Awake()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", masterVolume);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", musicVolume);
        soundVolume = PlayerPrefs.GetFloat("SoundVolume", soundVolume);
        screenShakeEnabled = (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) != 0);
        lockDelayDisplayEnabled = (PlayerPrefs.GetInt("LockDelayDisplayEnabled", 0) != 0);
        //languageIndex = PlayerPrefs.GetInt("LanguageIndex", 1);
        languageIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        //controlScheme = PlayerPrefs.GetInt("ControlScheme", 0);
        //abTest = PlayerPrefs.GetInt("ABTest", 0);

        autoRepeatRate = PlayerPrefs.GetFloat("AutoRepeatRate", autoRepeatRateDefault);
        delayedAutoShift = PlayerPrefs.GetFloat("DelayedAutoShift", delayedAutoShiftDefault);
        dasCutDelay = PlayerPrefs.GetFloat("DASCutDelay", dasCutDelayDefault);
        softDropFactor = PlayerPrefs.GetFloat("SoftDropFactor", softDropFactorDefault);
        lineClearPreventMinesweepDelay = PlayerPrefs.GetFloat("LineClearPreventMinesweepDelay", lineClearPreventMinesweepDelay);
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
        languageDropdown.value = languageIndex;
        languageDropdown.onValueChanged.AddListener(delegate { LanguageSelectDropdown(); });
        //StartCoroutine(SetLocale(languageIndex));

        autoRepeatRateSlider.SetAdjustedValue(autoRepeatRate);
        autoRepeatRateSlider.slider.onValueChanged.AddListener(delegate { AutoRepeatRateSlider(); });

        delayedAutoShiftSlider.SetAdjustedValue(delayedAutoShift);
        delayedAutoShiftSlider.slider.onValueChanged.AddListener(delegate { DelayedAutoShiftSlider(); });

        dasCutDelaySlider.SetAdjustedValue(dasCutDelay);
        dasCutDelaySlider.slider.onValueChanged.AddListener(delegate { DASCutDelaySlider(); });

        softDropFactorSlider.slider.value = softDropFactor;
        softDropFactorSlider.slider.onValueChanged.AddListener(delegate { SoftDropFactorSlider(); });

        lineClearPreventMinesweepDelaySlider.SetAdjustedValue(lineClearPreventMinesweepDelay);
        lineClearPreventMinesweepDelaySlider.slider.onValueChanged.AddListener(delegate { LineClearPreventMinesweepDelaySlider(); });

        //lineClearPreventMinesweepDelay
    }

    // Update is called once per frame
    /*void Update()
    {        
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = masterVolume;
            if (pauseMenuMove.GetIsActive())
                masterVolumeSlider.interactable = true;
            else
                masterVolumeSlider.interactable = false;
        }
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = musicVolume;
            if (pauseMenuMove.GetIsActive())
                musicVolumeSlider.interactable = true;
            else
                musicVolumeSlider.interactable = false;
        }
        if (soundVolumeSlider != null)
        {
            soundVolumeSlider.value = soundVolume;
            if (pauseMenuMove.GetIsActive())
                soundVolumeSlider.interactable = true;
            else
                soundVolumeSlider.interactable = false;
        }
        if (screenShakeToggle != null)
        {
            screenShakeToggle.isOn = !screenShakeEnabled;
            if (pauseMenuMove.GetIsActive())
                screenShakeToggle.interactable = true;
            else
                screenShakeToggle.interactable = false;
        }
        if (lockDelayDisplayToggle != null)
        {
            lockDelayDisplayToggle.isOn = lockDelayDisplayEnabled;
            if (pauseMenuMove.GetIsActive())
                lockDelayDisplayToggle.interactable = true;
            else
                lockDelayDisplayToggle.interactable = false;
        }
        if (languageDropdown != null)
        {
            languageDropdown.value = languageIndex;
            if (pauseMenuMove.GetIsActive())
                languageDropdown.interactable = true;
            else
                languageDropdown.interactable = false;
        }
    }*/

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
    public void LanguageSelectDropdown()
    {
        languageIndex = languageDropdown.value;
        //PlayerPrefs.SetInt("LanguageIndex", languageIndex);
        StartCoroutine(SetLocale(languageIndex));
    }

    IEnumerator SetLocale(int _localeID)
    {
        //Debug.Log("_localeID " + _localeID);
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
    }

    public void HoverMusicEnter()
    {
        gm.soundManager.EnablePauseFilter();
    }

    public void HoverMusicExit()
    {
        gm.soundManager.DisablePauseFilter();
    }

    public void AutoRepeatRateSlider()
    {
        autoRepeatRate = autoRepeatRateSlider.GetAdjustedValue();        
        PlayerPrefs.SetFloat("AutoRepeatRate", autoRepeatRate);
        gm.tetrominoSpawner.currentTetromino.GetComponent<Group>().UpdateInputValues();
    }
    public void DelayedAutoShiftSlider()
    {
        delayedAutoShift = delayedAutoShiftSlider.GetAdjustedValue();
        PlayerPrefs.SetFloat("DelayedAutoShift", delayedAutoShift);
        gm.tetrominoSpawner.currentTetromino.GetComponent<Group>().UpdateInputValues();
    }
    public void DASCutDelaySlider()
    {
        dasCutDelay = dasCutDelaySlider.GetAdjustedValue();
        PlayerPrefs.SetFloat("DASCutDelay", dasCutDelay);
        gm.tetrominoSpawner.currentTetromino.GetComponent<Group>().UpdateInputValues();
    }

    public void SoftDropFactorSlider()
    {
        softDropFactor = softDropFactorSlider.GetAdjustedValue();
        PlayerPrefs.SetFloat("SoftDropFactor", softDropFactor);
        gm.tetrominoSpawner.currentTetromino.GetComponent<Group>().UpdateInputValues();
    }

    public void LineClearPreventMinesweepDelaySlider()
    {
        lineClearPreventMinesweepDelay = lineClearPreventMinesweepDelaySlider.GetAdjustedValue();
        PlayerPrefs.SetFloat("LineClearPreventMinesweepDelay", lineClearPreventMinesweepDelay);
    }

    public void ResetDefaultsHandling()
    {
        /*float autoRepeatRate = 50;
        float delayedAutoShift = 250;
        float dasCutDelay = 17;
        float softDropFactor = 12;*/

        autoRepeatRate = autoRepeatRateDefault;
        delayedAutoShift = delayedAutoShiftDefault;
        dasCutDelay = dasCutDelayDefault;
        softDropFactor = softDropFactorDefault;
        lineClearPreventMinesweepDelay = lineClearPreventMinesweepDelayDefault;

        autoRepeatRateSlider.SetAdjustedValue(autoRepeatRateDefault);
        delayedAutoShiftSlider.SetAdjustedValue(delayedAutoShiftDefault);
        dasCutDelaySlider.SetAdjustedValue(dasCutDelayDefault);
        softDropFactorSlider.slider.value = softDropFactorDefault;
        lineClearPreventMinesweepDelaySlider.SetAdjustedValue(lineClearPreventMinesweepDelayDefault);
    }
}
