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
    public AudioClip enlargeSound;
    public AudioClip shrinkSound;
    //public float loopingAnimationDelay = 0f;

    // Option to disable programmatically
    public bool jiggleIsEnabled = true;
    public bool reorderToLastSibling = false;
    private Tween enlargeTween;
    private Tween shrinkTween;
    //private Tween jumpInPlaceTween;
    private Tween shrinkToZeroTween;
    public bool startAtScaleZero = false;
    void Start()
    {
        SetNewStartingValues();
    }

    /*IEnumerator PlayLoopingAnimations()
    {
        yield return new WaitForSeconds(loopingAnimationDelay);
        StartCoroutine(JumpInPlaceLoop());
    }*/

    void OnDestroy()
    {
        if(!this.gameObject.scene.isLoaded) 
            return;
        transform.DOKill();
        enlargeTween = null;
        shrinkTween = null;
        //jumpInPlaceTween = null;
        shrinkToZeroTween = null;
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

        if (GetComponent<AudioSource>() != null && enlargeSound != null)
        {
            GetComponent<AudioSource>().clip = enlargeSound;
            GetComponent<AudioSource>().volume = 0.6f * PlayerPrefs.GetFloat("SoundVolume", 0.5f);
            if (enlargeTween == null)
                GetComponent<AudioSource>().Play();
            else if (!enlargeTween.IsActive())
                GetComponent<AudioSource>().Play();
            else if (!enlargeTween.IsPlaying())
                GetComponent<AudioSource>().Play();
        }

        //animate on point hover
        enlargeTween = this.transform.DOScale(startScale * scaleMultiplierEnlarge, scaleTransitionTime).SetUpdate(true);
        this.transform.DOMoveZ(transform.position.z + scalePositionOffset, scaleTransitionTime).SetUpdate(true);   
        if (GetComponent<IdleJiggle>() != null)
            GetComponent<IdleJiggle>().ShakeRotation(GetComponent<IdleJiggle>().jumpInPlaceDuration, 0.15f);     
        
    }

    public void JumpInPlace()
    {
        if (GetComponent<IdleJiggle>() != null)
            GetComponent<IdleJiggle>().JumpInPlace();
    }

    /*public void Enlarge(bool overrideEnabled = false)
    {
        
    }*/

    public void Shrink () 
    {
        if (transform == null)// || (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 0))
            return;
        if (!jiggleIsEnabled)
            return;
        if (reorderToLastSibling)
            this.transform.SetAsLastSibling();
        
        if (GetComponent<AudioSource>() != null && shrinkSound != null)
        {
            GetComponent<AudioSource>().clip = shrinkSound;
            GetComponent<AudioSource>().volume = 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f);
            if (shrinkTween == null)
                GetComponent<AudioSource>().Play();
            else if (!shrinkTween.IsActive())
                GetComponent<AudioSource>().Play();
            else if (!shrinkTween.IsPlaying())
                GetComponent<AudioSource>().Play();
        }
        
        //animate on point click
        shrinkTween = this.transform.DOScale(startScale * scaleMultiplierShrink, scaleTransitionTime).SetUpdate(true);
        this.transform.DOMoveZ(transform.position.z - scalePositionOffset, scaleTransitionTime).SetUpdate(true);
    }



    public void ShrinkToZero(bool autoReset = false)
    {        
        if (autoReset)
            this.transform.DOScale(Vector3.zero, scaleTransitionTime).SetUpdate(true).OnKill(ResetScale);
        else
            shrinkToZeroTween = this.transform.DOScale(Vector3.zero, scaleTransitionTime).SetUpdate(true);            
    }

    public void Reset()
    {
        //if (transform == null)
            //return;
        //animate on pointer exit
        
        ResetPosition();
        ResetScale();
        
    }

    void ResetPosition()
    {
        this.transform.DOMoveZ(startZPos, scaleTransitionTime).SetUpdate(true);
    }
    void ResetScale()
    {
        if (shrinkToZeroTween != null)
            if (shrinkToZeroTween.IsActive())
                if (shrinkToZeroTween.IsPlaying())
                    return;
        this.transform.DOScale(startScale, scaleTransitionTime).SetUpdate(true);
    }

    public void SetNewStartingValues()
    {
        //cache the scale of the object
        startScale = this.transform.localScale;
        startZPos = this.transform.position.z;

        if (startAtScaleZero)
            this.transform.localScale = Vector3.zero;
    }


    /*public Sequence DOJumpY(float jumpPower, float duration)
    {
        float startPosY = this.transform.position.y;

        Sequence s = DOTween.Sequence();

        s.Append(this.transform.DOMoveY(startPosY + jumpPower, duration / 2).SetEase(Ease.OutQuad));
        s.Append(this.transform.DOMoveY(startPosY, duration / 2).SetEase(Ease.OutQuad));

        return s;
    }*/

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
