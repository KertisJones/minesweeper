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
    public float jumpInPlaceDuration = 0.3f;
    public float jumpInPlaceLoopDuration = -1;
    public ButtonJiggle jumpInPlaceSequenceNextObject;
    //public float loopingAnimationDelay = 0f;

    // Option to disable programmatically
    public bool jiggleIsEnabled = true;
    public bool reorderToLastSibling = false;
    private Tween enlargeTween;
    private Tween jumpInPlaceTween;
    private Tween shrinkToZeroTween;
    public bool startAtScaleZero = false;
    void Start()
    {
        //cache the scale of the object
        startScale = this.transform.localScale;
        startZPos = this.transform.position.z;

        if (startAtScaleZero)
            this.transform.localScale = Vector3.zero;
        
        StartCoroutine(JumpInPlaceLoop());
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
        jumpInPlaceTween = null;
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

        if (GetComponent<AudioSource>() != null)
        {
            GetComponent<AudioSource>().volume = 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f);
            if (enlargeTween == null)
                GetComponent<AudioSource>().Play();
            else if (!enlargeTween.IsPlaying())
                GetComponent<AudioSource>().Play();
        }

        //animate on point hover
        enlargeTween = this.transform.DOScale(startScale * scaleMultiplierEnlarge, scaleTransitionTime).SetUpdate(true);
        this.transform.DOMoveZ(transform.position.z + scalePositionOffset, scaleTransitionTime).SetUpdate(true);   
        if (GetComponent<IdleJiggle>() != null)
            GetComponent<IdleJiggle>().ShakeRotation(jumpInPlaceDuration, 0.15f);     
        
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
        JumpInPlace(false);
    }

    private void JumpInPlace(bool autoJump = false)
    {
        if (jumpInPlaceTween != null)
            if (jumpInPlaceTween.IsPlaying())
                return;
        
        if (jumpInPlaceHeight != 0)
        {
            if (autoJump)
            {                       
                if (this.transform.localScale.x != startScale.x)
                    jumpInPlaceSequencerSend();
                else
                {
                    jumpInPlaceTween = this.transform.DOJump(this.transform.position, 1.5f * jumpInPlaceHeight, 1, jumpInPlaceDuration).OnComplete(jumpInPlaceSequencerSend);
                    
                    //if (jumpInPlaceSequenceNextObject.GetComponent<IdleJiggle>() != null)
                    if (GetComponent<IdleJiggle>() != null)
                    {
                        GetComponent<IdleJiggle>().ShakeScale(jumpInPlaceDuration, 0.15f);
                        GetComponent<IdleJiggle>().ShakeRotation(jumpInPlaceDuration, 0.15f); 
                    }
                        
                }                
            }
            else
            {
                jumpInPlaceTween = this.transform.DOJump(this.transform.position, jumpInPlaceHeight, 1, jumpInPlaceDuration).OnKill(ResetPosition);
                if (GetComponent<IdleJiggle>() != null)
                    GetComponent<IdleJiggle>().ShakeRotation(jumpInPlaceDuration, 0.15f);           
            }            
        }
    }

    IEnumerator JumpInPlaceLoop()
    {
        if (jumpInPlaceLoopDuration >= 0)
        {
            yield return new WaitForSeconds(jumpInPlaceLoopDuration);
            jumpInPlaceSequencerReceive();
            StartCoroutine(JumpInPlaceLoop());
        }
    }

    private void jumpInPlaceSequencerReceive()
    {
        if (jumpInPlaceTween != null)
            if (jumpInPlaceTween.IsPlaying())
                jumpInPlaceSequencerSend();
        JumpInPlace(true);
    }
    private void jumpInPlaceSequencerSend()
    {
        if (jumpInPlaceSequenceNextObject != null)
            jumpInPlaceSequenceNextObject.jumpInPlaceSequencerReceive();
        //ResetPosition();
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
        {
            if (shrinkToZeroTween.IsPlaying())
            {
                Debug.Log("shrinkToZeroTween.IsPlaying");
                return;
            }
        }
        this.transform.DOScale(startScale, scaleTransitionTime).SetUpdate(true);
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
