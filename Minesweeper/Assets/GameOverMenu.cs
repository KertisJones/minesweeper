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
    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }
    public void SetDeathNormal()
    {
        gameOverText.text = GameManager.GetTranslation("UIText", "Menu GameOver");
        gameOverTextSubtitle.text = "";
        backgroundBlurImage.material = normalMaterial;
    }
    public void SetDeathBurn()
    {
        gameOverText.text = GameManager.GetTranslation("UIText", "Menu GameOver Burn");
        gameOverTextSubtitle.text = GameManager.GetTranslation("UIText", "Menu GameOver BurnSubtitle");
        backgroundBlurImage.material = burningMaterial;
    }
}
