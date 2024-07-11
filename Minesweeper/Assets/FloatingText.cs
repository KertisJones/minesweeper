using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingText : MonoBehaviour
{
    TextMeshProUGUI textBox;
    SpriteRenderer textColorHoldingSprite;
    public Vector3 scaleTarget = new Vector3(1.5f, 1.5f, 1.5f);
    public float duration = 1.5f;
    public Ease ease = Ease.InOutSine;

    private Color textColor;
    // Start is called before the first frame update
    void Start()
    {
        textBox = GetComponent<TextMeshProUGUI>();
        textColorHoldingSprite = GetComponent<SpriteRenderer>();

        textColorHoldingSprite.color = textBox.color;

        transform.DOScale(scaleTarget, duration).SetEase(ease);
        textColorHoldingSprite.DOColor(Color.clear, duration).SetEase(ease);
    }

    // Update is called once per frame
    void Update()
    {
        textBox.color = textColorHoldingSprite.color;
        if (textBox.color == Color.clear)
            Destroy(this.gameObject);
    }

}
