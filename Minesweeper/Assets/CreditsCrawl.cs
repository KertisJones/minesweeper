using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;


public class CreditsCrawl : MonoBehaviour
{
    public float target = 0;
    public float creditsDuration = 10;

    public string nextScene = "";

    public TextMeshProUGUI continueText;
    public SpriteRenderer colorSprite;
    private Color textColor;
    void OnEnable()
    {
        InputManager.Instance.anyKey.started += _ => NextScene();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (colorSprite != null)
        {
            textColor = colorSprite.color;
            colorSprite.color = Color.clear;

            colorSprite.DOColor(textColor, 20f).SetUpdate(true).SetEase(Ease.InOutSine);
        }

        this.transform.DOLocalMoveY(target, creditsDuration).SetEase(Ease.InOutSine).OnComplete(NextScene);
    }

    void Update()
    {
        if (continueText != null)
            continueText.color = colorSprite.color;
    }

    void NextScene()
    {
        GetComponent<LoadNewScene>().OpenNewScene(nextScene);
    }
}
