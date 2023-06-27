using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    private static Tooltip Instance;
    private InputManager inputManager;
    private Camera cam;
    public TMPro.TMP_Text tooltipText;
    public RectTransform backgroundRectTransform;
    
    bool isActive = false;
    bool isVisible = false;
    float timeDelay = 0;
    float timePrev = 0;
    public Vector2 positionOffset = new Vector2();
    string tooltipString = "";
    
    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
        
        inputManager = InputManager.Instance;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        HideTooltip();
        //backgroundRectTransform = transform.Fin .GetComponent<RectTransform>();
        //tooltipText = transform. Find ("text") .GetComponent<TMPro.TMP_Text>();

        //ShowTooltip_Static("This is a tooltip!");
    }

    private void ShowTooltip()
    {
        isVisible = true;
        //gameObject.SetActive(true);
        tooltipText.gameObject.SetActive(true);
        backgroundRectTransform.gameObject.SetActive(true);
        UpdatePosition();

        tooltipText.text = tooltipString;
        tooltipText.ForceMeshUpdate();

        //float width = tooltipText.preferredWidth / tooltipText.textInfo.lineCount;
        //.textBounds.size.x;//.GetPreferredWidth()//GetPreferredValues(tooltipText.text, 10, 0).x;//.textBounds.extents.x;//.size.x;//.rendered.width.characterWidthAdjustment;//.renderedWidth;//.flexibleWidth.preferredWidth;
        float width = tooltipText.renderedWidth + tooltipText.margin.x + tooltipText.margin.z;
        
        //if (width > tooltipText.rectTransform.rect.width)
            //width = tooltipText.rectTransform.rect.width;
        //Debug.Log(width + "=" + tooltipText.preferredWidth + " / " + tooltipText.textInfo.lineCount);
        Vector2 backgroundSize = new Vector2(width * tooltipText.rectTransform.localScale.x, tooltipText.preferredHeight *  tooltipText.rectTransform.localScale.y);
        backgroundRectTransform.sizeDelta = backgroundSize;
    }
    
    private void Update() {
        if (!isActive)
            return;
        
        UpdatePosition();

        if (timeDelay > 0.5f)
        {
            ShowTooltip();
        }

        timeDelay += Time.deltaTime;    
        
        /*if (timePrev > 1)
        {
            timePrev = 0;
            string abc = "abcdefghijklmnopqrstuvwxyz";
            string showText = "";
            for (int j = 0; j < UnityEngine.Random.RandomRange(1, 25); j++)
            {
                showText += abc[Random.RandomRange(0, abc.Length)];
            }

            for (int i = 0; i < UnityEngine.Random.RandomRange(0, 24); i++)
            {
                showText += " ";
                for (int j = 0; j < UnityEngine.Random.RandomRange(1, 25); j++)
                {
                    showText += abc[Random.RandomRange(0, abc.Length)];
                }
            }
            ShowTooltip_Static(showText);
        }
        timePrev += Time.deltaTime;   */   
    }

    private void UpdatePosition() 
    { 
        Vector2 localPoint = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), inputManager.GetMousePosition(), cam, out localPoint);
        RectTransform parentCanvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        localPoint *= parentCanvasRect.localScale;
        localPoint += new Vector2(parentCanvasRect.position.x, parentCanvasRect.position.y);                
        localPoint += positionOffset;
        transform.position = new Vector3(localPoint.x, localPoint.y, transform.position.z);
    }

    private void HideTooltip()
    {
        //gameObject.SetActive(false);
        tooltipText.gameObject.SetActive(false);
        backgroundRectTransform.gameObject.SetActive(false);
        isActive = false;
        isVisible = false;
        timeDelay = 0;
    }

    public static void ShowTooltip_Static(string tooltipNewString)
    {
        Instance.isActive = true;     
        Instance.isVisible = false;
        Instance.timeDelay = 0;   
        Instance.tooltipString = tooltipNewString;
    }

    public static void HideTooltip_Static()
    {
        Instance.HideTooltip();
    }

}
