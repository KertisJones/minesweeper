using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonJiggle : MonoBehaviour
{
    [SerializeField]
    private Vector3 startScale;
    private Vector3 targetScaleEnlarge;
    private Vector3 targetScaleShrink;
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
    private Tween resetTween;
    public bool startAtScaleZero = false;

    public bool isScaled = false;

    void OnEnable()
    {
        GameManager.OnKillTweenEvent += Reset;
    }
    void OnDisable()
    {
        GameManager.OnKillTweenEvent -= Reset;
    }

    void Awake()
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

        if (GetComponent<IdleJiggle>() != null)
            GetComponent<IdleJiggle>().ShakeRotation(GetComponent<IdleJiggle>().jumpInPlaceDuration, 0.15f);

        if (resetTween != null)
            if (resetTween.IsActive())
                if (resetTween.IsPlaying())
                    return;

        isScaled = true;

        //animate on point hover
        enlargeTween = this.transform.DOBlendableScaleBy(targetScaleEnlarge - transform.localScale, scaleTransitionTime).SetUpdate(true);
        this.transform.DOMoveZ(transform.position.z + scalePositionOffset, scaleTransitionTime).SetUpdate(true);   
                
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

        isScaled = true;

        //animate on point click
        shrinkTween = this.transform.DOBlendableScaleBy(targetScaleShrink - transform.localScale, scaleTransitionTime).SetUpdate(true);
        this.transform.DOMoveZ(transform.position.z - scalePositionOffset, scaleTransitionTime).SetUpdate(true);
    }



    public void ShrinkToZero(bool autoReset = false, float overrideTransitionTime = -1)
    {
        //Debug.Log(name + " Shrink to Zero!");
        if (shrinkToZeroTween != null)
            if (shrinkToZeroTween.IsActive())
                if (shrinkToZeroTween.IsPlaying())
                {
                    //Debug.Log("Shrink to Zero...DENIED!");
                    return;
                }            
        //Debug.Log("Shrink to Zero. AutoReset=" + autoReset);
        isScaled = true;
        if (overrideTransitionTime < 0)
            overrideTransitionTime = scaleTransitionTime;

        if (autoReset)
        {
            shrinkToZeroTween = this.transform.DOBlendableScaleBy(transform.localScale * -1, overrideTransitionTime).SetUpdate(true).OnComplete(ResetScale);
        }
        else
        {
            shrinkToZeroTween = this.transform.DOBlendableScaleBy(transform.localScale * -1, overrideTransitionTime).SetUpdate(true);
        }
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
        /*if (shrinkToZeroTween != null)
            if (shrinkToZeroTween.IsActive())
                if (shrinkToZeroTween.IsPlaying())
                    return;*/
        
        if (enlargeTween != null)
            enlargeTween.Kill();
        if (shrinkTween != null)
            shrinkTween.Kill();
        if (resetTween != null) 
            resetTween.Kill();
        if (shrinkToZeroTween != null)
        {
            if (shrinkToZeroTween.IsActive())
                if (shrinkToZeroTween.IsPlaying())
                {
                    //Debug.Log("ResetScale... onComplete=" + shrinkToZeroTween.onComplete);
                    if (shrinkToZeroTween.onComplete != null)
                        shrinkToZeroTween.Kill();
                    else
                        return;
                }            
        }
        //Debug.Log("ResetScale");


        isScaled = false;

        //Debug.Log(name + " ResetScale ButtonJiggle! " + startScale + ", " + transform.localScale);
        //transform.localScale = new Vector3(Mathf.Max(0, transform.localScale.x), Mathf.Max(0, transform.localScale.y), Mathf.Max(0, transform.localScale.z));
        resetTween = this.transform.DOBlendableScaleBy(startScale - transform.localScale, scaleTransitionTime).SetUpdate(true);
    }

    public void SetNewStartingValues()
    {
        //cache the scale of the object
        startScale = this.transform.localScale;
        targetScaleEnlarge = startScale * scaleMultiplierEnlarge;
        targetScaleShrink = startScale * scaleMultiplierShrink;
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
