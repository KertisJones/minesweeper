using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.PlayerLoop;

public class DemoTitleScreen : MonoBehaviour
{
    public GameObject playFrameStandard;
    public GameObject playFrameDemo;
    public GameObject[] buttonsUnlockedByDemoVisit;
    public GameObject lockedByDemoVisitText;
    public TMP_Text callToActionText;

    // Positioning
    public GameObject bouncyLogo;
    public Transform topLeftBounds;

    float steamCallToActionSwitchTime = 10f;
    float lastCallToActionSwitch = 0f;
    // Start is called before the first frame update
    void Start()
    {
        if (ScoreKeeper.versionType == ScoreKeeper.VersionType.standard || ScoreKeeper.versionType == ScoreKeeper.VersionType.beta)
        {
            playFrameStandard.SetActive(true);
            playFrameDemo.SetActive(false);
        }
        else if (ScoreKeeper.versionType == ScoreKeeper.VersionType.demoOnline || ScoreKeeper.versionType == ScoreKeeper.VersionType.demoSteam)
        {
            playFrameStandard.SetActive(false);
            playFrameDemo.SetActive(true);

            // Disable variant modes until player clicks on the steam page
            if (PlayerPrefs.GetInt("DemoVisit", 0) == 0 && ScoreKeeper.versionType == ScoreKeeper.VersionType.demoOnline)
            {
                lockedByDemoVisitText.SetActive(true);
                foreach (GameObject button in buttonsUnlockedByDemoVisit)
                {
                    button.GetComponent<Button>().interactable = false;
                    button.GetComponent<ButtonJiggle>().jiggleIsEnabled = false;
                }
                
            }
            else
            {
                UnlockDemo();
            }
        }
    }

    private void Update()
    {
        if (Time.time - lastCallToActionSwitch < steamCallToActionSwitchTime)
            callToActionText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", "Menu SteamCallToActionUnclicked"); // "Unlock bonus game modes!"
        else if (Time.time - lastCallToActionSwitch < steamCallToActionSwitchTime * 2)
            callToActionText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", "Menu SteamCallToActionClicked"); // "Tetrisweep like never before!"
        else
            lastCallToActionSwitch = Time.time;
    }

    void UnlockDemo()
    {
        lockedByDemoVisitText.SetActive(false);
        foreach (GameObject button in buttonsUnlockedByDemoVisit)
        {
            button.GetComponent<Button>().interactable = true;
            button.GetComponent<ButtonJiggle>().jiggleIsEnabled = true;
        }
        
    }

    public void SetDemoVisitTrue()
    {
        PlayerPrefs.SetInt("DemoVisit", 1);
        UnlockDemo();
    }
    public void SetDemoVisitFalse()
    {
        PlayerPrefs.SetInt("DemoVisit", 0);
    }

    public void ResetStartingPositionsInChildren(float sizeModifier)
    {
        IdleJiggle[] childJigglesIdle = GetComponentsInChildren<IdleJiggle>();
        foreach (var idleJiggle in childJigglesIdle)
        {
            idleJiggle.jumpInPlaceHeight *= sizeModifier;
        }

        /*ButtonJiggle[] childJigglesButton = GetComponentsInChildren<ButtonJiggle>();
        foreach (var buttonJiggle in childJigglesButton)
        {
            buttonJiggle.SetNewStartingValues();
        }*/
    }
}
