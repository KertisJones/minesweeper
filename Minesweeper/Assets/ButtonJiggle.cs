using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonJiggle : MonoBehaviour
{
    private Vector3 startScale;
    private float startZPos;

    public float scaleMultiplierEnlarge = 1.1f;
    public float scaleMultiplierShrink = 0.9f;
    public float scaleTransitionTime = 0.15f;
    public float scalePositionOffset = 0f;
    public float jumpInPlaceHeight = 0f;

    // Option to disable programmatically
    public bool jiggleIsEnabled = true;
    public bool reorderToLastSibling = false;
    private Tween jumpInPlaceTween;
    void Start()
    {
        //cache the scale of the object
        startScale = this.transform.localScale;
        startZPos = this.transform.position.z;
    }

    /*public void OnPointerClick(PointerEventData eventData)
    {
        //set selected object
        EventSystem.current.SetSelectedGameObject(this.gameObject);

        //compare count click and do tween if double clicked
        if (eventData.clickCount == 2)
            this.transform.DOShakeRotation(1f);
    }*/

    public void Enlarge () 
    {
        if (transform == null)// || (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 0))
            return;
        if (!jiggleIsEnabled)
            return;
        if (reorderToLastSibling)
            this.transform.SetAsLastSibling();

        //animate on point hover
        this.transform.DOScale(startScale * scaleMultiplierEnlarge, scaleTransitionTime).SetUpdate(true);
        this.transform.DOMoveZ(transform.position.z + scalePositionOffset, scaleTransitionTime).SetUpdate(true);
        
    }

    public void Shrink () 
    {
        if (transform == null)// || (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 0))
            return;
        if (!jiggleIsEnabled)
            return;
        if (reorderToLastSibling)
            this.transform.SetAsLastSibling();
        
        //animate on point click
        this.transform.DOScale(startScale * scaleMultiplierShrink, scaleTransitionTime).SetUpdate(true);
        this.transform.DOMoveZ(transform.position.z - scalePositionOffset, scaleTransitionTime).SetUpdate(true);
    }

    public void JumpInPlace()
    {
        if (jumpInPlaceTween != null)
            if (jumpInPlaceTween.IsPlaying())
                return;
        
        if (jumpInPlaceHeight != 0)
        {
            jumpInPlaceTween = this.transform.DOJump(this.transform.position, jumpInPlaceHeight, 1, 0.5f);//.OnKill(JumpInPlaceReset);
        }        
    }

    public void Reset ()
    {
        if (transform == null)
            return;
        //animate on pointer exit
        this.transform.DOScale(startScale, scaleTransitionTime).SetUpdate(true);
        this.transform.DOMoveZ(startZPos, scaleTransitionTime).SetUpdate(true);
    }

    /*public void OnPointerEnter(PointerEventData eventData)
    {
        //animate on point hover
        this.transform.DOScale(startScale * 1.1f, 0.15f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //animate on pointer exit
        this.transform.DOScale(startScale, 0.15f);
    }*/
}
