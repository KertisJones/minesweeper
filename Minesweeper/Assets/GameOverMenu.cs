using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GameOverMenu : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI gameOverTextSubtitle;
    public SpriteRenderer backgroundBlurImage;

    public Material normalMaterial;
    public Material burningMaterial;

    public void SetDeathNormal()
    {
        gameOverText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", "Menu GameOver");
        gameOverTextSubtitle.text = "";
        backgroundBlurImage.material = normalMaterial;
    }
    public void SetDeathBurn()
    {
        gameOverText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", "Menu GameOver Burn");
        gameOverTextSubtitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", "Menu GameOver BurnSubtitle");
        backgroundBlurImage.material = burningMaterial;
    }
}
