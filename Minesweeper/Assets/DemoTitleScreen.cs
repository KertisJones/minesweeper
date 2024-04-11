using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class DemoTitleScreen : MonoBehaviour
{
    public GameObject playFrameStandard;
    public GameObject playFrameDemo;
    public GameObject[] buttonsUnlockedByDemoVisit;
    public GameObject lockedByDemoVisitText;
    public TMP_Text callToActionText;
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
                callToActionText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", "Menu SteamCallToActionUnclicked"); // "Unlock bonus game modes!"
            }
            else
            {
                UnlockDemo();
            }
        }
    }

    void UnlockDemo()
    {
        lockedByDemoVisitText.SetActive(false);
        foreach (GameObject button in buttonsUnlockedByDemoVisit)
        {
            button.GetComponent<Button>().interactable = true;
            button.GetComponent<ButtonJiggle>().jiggleIsEnabled = true;
        }
        callToActionText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", "Menu SteamCallToActionClicked"); // "Tetrisweep like never before!"
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
}
