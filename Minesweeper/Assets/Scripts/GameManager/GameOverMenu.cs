using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GameOverMenu : MonoBehaviour
{
    GameManager gm;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI gameOverTextSubtitle;
    public SpriteRenderer backgroundBlurImage;

    public Material normalMaterial;
    public Material burningMaterial;

    //public GameObject restartButton;
    //public GameObject endlessButton;
    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }
    public void SetDeathNormal()
    {
        gameOverText.text = GameManager.GetTranslation("UIText", "Menu GameOver");
        gameOverTextSubtitle.text = "";
        backgroundBlurImage.material = normalMaterial;

        //restartButton.SetActive(true);
        //endlessButton.SetActive(false);
    }
    public void SetDeathBurn()
    {
        gameOverText.text = GameManager.GetTranslation("UIText", "Menu GameOver Burn");
        gameOverTextSubtitle.text = GameManager.GetTranslation("UIText", "Menu GameOver BurnSubtitle");
        backgroundBlurImage.material = burningMaterial;

        //restartButton.SetActive(true);
        //endlessButton.SetActive(false);
    }
    /*public void SetGamemodeComplete()
    {
        // gameModeNameComplete
        string gameMode = gm.gameMods.gameModeName;
        if (gm.gameMods.gameModeDisplayName != "")
            gameMode = gm.gameMods.gameModeDisplayName;

        string localizedText = GameManager.GetTranslation("UIText", "GameMode " + gameMode); // Returns translation of game mode name, ex. "Marathon"

        gameOverText.text = localizedText + " " + GameManager.GetTranslation("UIText", "GameMode Complete") + "!";

        gameOverTextSubtitle.text = "";
        backgroundBlurImage.material = normalMaterial;

        endlessButton.SetActive(true);
        restartButton.SetActive(false);
    }*/
}
