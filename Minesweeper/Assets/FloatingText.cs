using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingText : MonoBehaviour
{
    GameManager gm;
    TextMeshProUGUI textBox;
    SpriteRenderer textColorHoldingSprite;
    public Vector3 scaleTarget = new Vector3(1.5f, 1.5f, 1.5f);
    public float duration = 1.5f;
    public Ease ease = Ease.InOutSine;

    private Vector3 startingScale;
    private Color startingColor;
    private Tween scaleTween;
    private Tween colorTween;

    private Color textColor;
    // Start is called before the first frame update
    void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        textBox = GetComponent<TextMeshProUGUI>();
        textColorHoldingSprite = GetComponent<SpriteRenderer>();

        startingScale = this.transform.localScale;
        startingColor = textBox.color;
        textColorHoldingSprite.color = textBox.color;

        scaleTween = transform.DOBlendableScaleBy(scaleTarget - this.transform.localScale, duration).SetEase(ease).SetUpdate(true);
        colorTween = textColorHoldingSprite.DOColor(Color.clear, duration).SetEase(ease).SetUpdate(true);
    }

    // Update is called once per frame
    void Update()
    {
        textBox.color = textColorHoldingSprite.color;
        if (textBox.color == Color.clear)
        {            
            Destroy(this.gameObject);
        }        
    }

    public void RefreshFade()
    {        
        if (scaleTween != null)
            scaleTween.Kill();

        this.DOKill();
        textColorHoldingSprite.DOKill();

        //Debug.Log(this.transform.localScale + " ->" + startingScale);
        this.transform.localScale = startingScale;
        //Debug.Log(this.transform.localScale + "? " + startingScale);
        textColorHoldingSprite.color = startingColor;
        textBox.color = startingColor;

        scaleTween = transform.DOBlendableScaleBy(scaleTarget - startingScale, duration).SetEase(ease).SetUpdate(true);
        colorTween = textColorHoldingSprite.DOColor(Color.clear, duration).SetEase(ease).SetUpdate(true);
    }

    public void UpdateTextScore(float scoreValue, string translationKey, int comboCount, string translationKeyPrefix1, string translationKeyPrefix2, string translationKeySuffix)
    {
        string prefix1 = "";
        if (translationKeyPrefix1 != "")
            prefix1 = GameManager.GetTranslation("UIText", translationKeyPrefix1) + " ";

        string prefix2 = "";
        if (translationKeyPrefix2 != "")
            prefix2 = GameManager.GetTranslation("UIText", translationKeyPrefix2) + " ";

        string suffix = "";
        if (translationKeySuffix != "")
            suffix = " " + GameManager.GetTranslation("UIText", translationKeySuffix);

        string text = "+" + scoreValue.ToString("#,#") + " " + prefix1 + prefix2 + GameManager.GetTranslation("UIText", translationKey) + suffix;

        if (comboCount > 1)
            text += " x" + comboCount;

        textBox.text = text;

        if (scaleTween != null)
            RefreshFade();
    }

    private void OnDestroy()
    {
        this.DOKill();
        textColorHoldingSprite.DOKill();
        FloatingTextQueue queue = gameObject.GetComponentInParent<FloatingTextQueue>();
        if (queue != null)
            queue.RemoveFloater(this.gameObject);
    }
}
