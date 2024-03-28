using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
/*#if UNITY_EDITOR
using UnityEditor;
#endif*/

public class ProgressBar : MonoBehaviour
{
/*#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Linear Progress Bar")]
    public static void AddLinearProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Linear Progress Bar"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }
#endif*/

    public int minimum;
    public int maximum;
    public float current;
    [HideInInspector]
    public float currentTween;
    private float tweenAmount = 0.01f;
    public Image mask;
    public Image fill;
    [SerializeField]
    private Color color;
    public TMPro.TMP_Text hoverText;
    public SpriteRenderer hoverTextSprite;
    private Color textColor;
    // Start is called before the first frame update
    void Start()
    {
        currentTween = current;
        fill.color = color;
        textColor = hoverTextSprite.color;
        hoverTextSprite.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {        
        hoverText.color = hoverTextSprite.color;
        
        TweenProgress();
        GetCurrentFill();
    }

    void GetCurrentFill()
    {
        float currentOffset = currentTween - minimum;
        float maximumOffset = maximum - minimum;
        float fillAmount = currentOffset / maximumOffset;
        if (currentOffset > maximumOffset)
            currentOffset = maximumOffset;
        mask.fillAmount = fillAmount;
        hoverText.text = (current - minimum) + "/" + maximumOffset;
    }

    void TweenProgress()
    {
        if (current == currentTween)
            return;
        else if (Mathf.Abs(current - currentTween) < tweenAmount)
        {
            currentTween = current;
        }
        else if (current > currentTween)
        {
            currentTween += tweenAmount;
        }
        else if (current < currentTween)
        {
            currentTween -= tweenAmount;
        }
    }

    public void ChangeColor(Color newColor)
    {
        fill.DOColor(newColor, 0.5f);
    }

    public void ShowText()
    {
        hoverTextSprite.DOColor(textColor, 0.15f);        
    }

    public void HideText()
    {
        hoverTextSprite.DOColor(Color.clear, 0.15f);    
    }
}
