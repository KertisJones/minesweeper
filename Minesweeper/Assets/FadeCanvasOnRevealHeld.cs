using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class FadeCanvasOnRevealHeld : MonoBehaviour
{
    private InputManager inputManager;
    CanvasGroup canvasGroup;
    Tween hideTween;
    Tween showTween;

    public float fadeTime = 0.25f;

    void Awake()
    {
        inputManager = InputManager.Instance;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        inputManager.revealTilePress.started += _ => HideCanvas();
        inputManager.revealTilePress.canceled += _ => ShowCanvas();
        inputManager.flagTilePress.started += _ => HideCanvas();
        inputManager.flagTilePress.canceled += _ => ShowCanvas();
    }
    void OnDisable()
    {
        inputManager.revealTilePress.started -= _ => HideCanvas();
        inputManager.revealTilePress.canceled -= _ => ShowCanvas();
        inputManager.flagTilePress.started -= _ => HideCanvas();
        inputManager.flagTilePress.canceled -= _ => ShowCanvas();
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    public void HideCanvas()
    {
        if (showTween != null)
            showTween.Kill();

        if (hideTween != null)// && !IsPointerOverUIObject())
            if (hideTween.IsPlaying())
                return;
        
        if (IsPointerOverUIObject())
            return;

        hideTween = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, fadeTime);
        hideTween.SetUpdate(true);
    }

    public void ShowCanvas()
    {
        if (hideTween != null)
            hideTween.Kill();

        if (showTween != null)
            if (showTween.IsPlaying())
                return;
        
        showTween = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, fadeTime);
        showTween.SetUpdate(true);
    }

    public bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        List<RaycastResult> finalResults = new List<RaycastResult>();

        foreach (var raycastResult in results)
        {
            if (raycastResult.gameObject.transform.IsChildOf(this.gameObject.transform))
                finalResults.Add(raycastResult);
        }
        return finalResults.Count > 0;
    }
}
