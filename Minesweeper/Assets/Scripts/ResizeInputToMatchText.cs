using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResizeInputToMatchText : MonoBehaviour
{
    private RectTransform m_Rect;
    private RectTransform m_inputFieldRect;
    private float maxSize;


    private RectTransform rectTransform
    {
        get
        {
            if (m_Rect == null)
                m_Rect = GetComponent<RectTransform>();
            return m_Rect;
        }
    }

    private RectTransform inputFieldTextRectTransform
    {
        get
        {
            if (m_inputFieldRect == null)
                m_inputFieldRect = GetComponent<TextMeshProUGUI>().rectTransform;
            return m_inputFieldRect;
        }
    }

    void Start()
    {
        maxSize = rectTransform.rect.width;
    }

    private void Update()
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Min(maxSize, LayoutUtility.GetPreferredSize(inputFieldTextRectTransform, 0)));
        //inputFieldTextRectTransform.localPosition = Vector3.zero; // stops the text scrolling sideways - it doesn't need to
    }
}