using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;

public class IdleJiggle : MonoBehaviour
{
    private Transform myTransform;
    public Vector3 idleMoveDistance = Vector3.zero;
    public float idleMoveDuration = 0f;
    public Ease idleMoveEase = Ease.InOutSine;
    public float leanDistance = 1f;
    public float leanDuration = 1f;
    public float jumpInPlaceHeight = 0f;
    public float jumpInPlaceDuration = 0.3f;
    public float jumpInPlaceLoopDuration = -1;
    public IdleJiggle jumpInPlaceSequenceNextObject;
    private Tween shakePositionTween;
    private Tween shakeRotationTween;
    private Tween shakeScaleTween;
    private Tween leanTweenX;
    private Tween leanTweenY;
    private Tween jumpInYTween;
    private Vector3 startPosition;
    private Vector3 startScale;    
    public bool jiggleIsEnabled = true;
    public bool jiggleRotateIsEnabled = true;
    public bool jiggleScaleIsEnabled = true;
    public bool jiggleMoveIsEnabled = true;
    public bool jiggleOnActionIsEnabled = true;
    public bool jiggleLeanIsEnabled = false;

    // Input Handling
    private bool buttonLeftHeld = false;
    private bool buttonLeftHeldSecondary = false;
    private bool buttonRightHeld = false;
    private bool buttonRightHeldSecondary = false;
    
    void OnEnable()
    {
        if (jiggleOnActionIsEnabled)
        {
            //GameManager.OnHardDropEvent += HardDrop;
            GameManager.OnLineClearEvent += _ => LineClear(_);
            GameManager.OnGameOverEvent += GameOver;
            GameManager.OnHardDropEvent += PunchDown;
            InputManager.Instance.leftPress.started += _ => PressLeft();
            InputManager.Instance.leftPress.canceled += _ => ReleaseLeft();
            InputManager.Instance.rightPress.started += _ => PressRight();
            InputManager.Instance.rightPress.canceled += _ => ReleaseRight();
        }        
    }
    void OnDisable()
    {
        DisableEverything();
    }

    /*void OnDestroy()
    {
        DisableEverything();
    }*/

    void DisableEverything()
    {
        if (jiggleOnActionIsEnabled)
        {
            //GameManager.OnHardDropEvent -= HardDrop;
            GameManager.OnLineClearEvent -= _ => LineClear(_);
            GameManager.OnGameOverEvent -= GameOver;
            GameManager.OnHardDropEvent -= PunchDown;
            InputManager.Instance.leftPress.started -= _ => PressLeft();
            InputManager.Instance.leftPress.canceled -= _ => ReleaseLeft();
            InputManager.Instance.rightPress.started -= _ => PressRight();
            InputManager.Instance.rightPress.canceled -= _ => ReleaseRight();
        }
        if(!this.gameObject.scene.isLoaded) 
            return;
        transform.DOKill();
        shakePositionTween = null;
        shakeRotationTween = null;
        shakeScaleTween = null;      
        leanTweenX = null;  
        leanTweenY = null; 
        jumpInYTween = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        myTransform = this.transform;
        if (myTransform == null)// || (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 0))
            return;
        if (!jiggleIsEnabled)
            return;

        startPosition = this.transform.position;
        startScale = this.transform.localScale;
        
        if (idleMoveDistance != Vector3.zero && idleMoveDuration > 0)
        {
            if (idleMoveDistance.x != 0)
                this.transform.DOMoveX(transform.position.x + idleMoveDistance.x, idleMoveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(idleMoveEase);
            if (idleMoveDistance.y != 0)
                this.transform.DOMoveY(transform.position.y + idleMoveDistance.y, idleMoveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(idleMoveEase);
            if (idleMoveDistance.z != 0)
                this.transform.DOMoveZ(transform.position.z + idleMoveDistance.z, idleMoveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(idleMoveEase);
        }

        StartCoroutine(JumpInPlaceLoop());
    }
    #region Jiggle On Action
    void LineClear(int lines)
    {
        if (jiggleOnActionIsEnabled)
            Shake(lines * 0.2f, 0.15f);
    }

    void GameOver()
    {
        if (jiggleOnActionIsEnabled)
            Shake(3f, .25f);
    }

    void PunchDown()
    {
        if (!IsShakeValid(jiggleOnActionIsEnabled, null))//jumpInYTween))
            return;
        
        if (jumpInYTween != null)
            if (jumpInYTween.IsActive())
                if (jumpInYTween.IsPlaying())
                    jumpInYTween.Kill();
        
        if (jiggleOnActionIsEnabled)
            this.transform.DOMoveY(startPosition.y, 0.05f).SetUpdate(true).OnKill(() => DOJumpY(leanDistance * -1, jumpInPlaceDuration).OnKill(ResetPosition));
    }


    public void PressLeft()
    {
        if (!buttonRightHeld)
            buttonLeftHeld = true;
        buttonLeftHeldSecondary = true;

        if (!buttonRightHeld)
            LeanX(-1);
    }
    void ReleaseLeft()
    {
        buttonLeftHeld = false;
        buttonLeftHeldSecondary = false;
        if (!buttonRightHeld && buttonRightHeldSecondary)
            buttonRightHeld = true;

        if (!buttonRightHeld)
            LeanX(0);
        else
            LeanX(1);
    }
    public void PressRight()
    {
        if (!buttonLeftHeld)
            buttonRightHeld = true;
        buttonRightHeldSecondary = true;

        if (!buttonLeftHeld)
            LeanX(1);
    }
    void ReleaseRight()
    {
        buttonRightHeld = false;
        buttonRightHeldSecondary = false;
        if (!buttonLeftHeld && buttonLeftHeldSecondary)
            buttonLeftHeld = true;

        if (!buttonLeftHeld)
            LeanX(0);
        else
            LeanX(-1);
    }
    #endregion

    public void Shake(float duration, float strength)
    {        
        ShakePosition(duration, strength);
        ShakeRotation(duration, strength);
        ShakeScale(duration, strength);
    }

    public void ShakePosition(float duration, float strength)
    {
        if (!IsShakeValid(jiggleMoveIsEnabled, shakePositionTween))
            return;
        shakePositionTween = this.transform.DOShakePosition(duration, new Vector3(strength, strength, 0)).OnKill(ResetPosition);
    }

    public void ShakeRotation(float duration, float strength)
    {
        if (!IsShakeValid(jiggleRotateIsEnabled, shakeRotationTween))
            return;
        shakeRotationTween = this.transform.DOShakeRotation(duration, new Vector3(0, 0, strength * 40));
    }

    public void ShakeScale(float duration, float strength)
    {
        if (!IsShakeValid(jiggleScaleIsEnabled, shakeScaleTween))
            return;
        shakeScaleTween = this.transform.DOShakeScale(duration, strength);
    }

    private bool IsShakeValid(bool isJiggleTypeIsEnabled, Tween tweenToCheckIfPlaying = null)
    {
        if (myTransform == null)// || (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 0))
        {
            return false;  
        }                  
        if (!jiggleIsEnabled || !isJiggleTypeIsEnabled)
            return false;
        if (this.transform.localScale == Vector3.zero)
            return false;

        if (tweenToCheckIfPlaying != null)
            if (tweenToCheckIfPlaying.IsActive())
                if (tweenToCheckIfPlaying.IsPlaying())
                    return false;

        bool screenShake = (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) != 0);
        if (!screenShake)
            return false;

        return true;
    }

    public void LeanX(int dir)
    {
        if (!IsShakeValid(jiggleLeanIsEnabled, null))
            return;
        
        if (leanTweenX != null)
            if (leanTweenX.IsActive())
                if (leanTweenX.IsPlaying())
                    leanTweenX.Kill();

        leanTweenX = this.transform.DOMoveX(startPosition.x + leanDistance * dir, leanDuration).SetEase(idleMoveEase);
    }

    public void LeanY(int dir)
    {
        if (!IsShakeValid(jiggleLeanIsEnabled, null))
            return;
        
        if (leanTweenY != null)
            if (leanTweenY.IsActive())
                if (leanTweenY.IsPlaying())
                    leanTweenY.Kill();

        leanTweenY = this.transform.DOMoveY(startPosition.y + leanDistance * dir, leanDuration).SetEase(idleMoveEase);
    }

    #region Jumping
    public void JumpInPlace()
    {
        JumpInPlace(false);
    }

    private void JumpInPlace(bool autoJump = false)
    {
        if (jumpInYTween != null)
            if (jumpInYTween.IsActive())
                if (jumpInYTween.IsPlaying())
                    return;
        
        if (jumpInPlaceHeight != 0)
        {
            if (autoJump)
            {                       
                if (this.transform.localScale.x != startScale.x)
                    JumpInPlaceSequencerSend();
                else
                {
                    DOJumpY(1.5f * jumpInPlaceHeight, jumpInPlaceDuration).OnKill(JumpInPlaceSequencerSend);//this.transform.DOJump(this.transform.position, 1.5f * jumpInPlaceHeight, 1, jumpInPlaceDuration).OnComplete(JumpInPlaceSequencerSend);
                    
                    ShakeScale(jumpInPlaceDuration, 0.15f);
                    ShakeRotation(jumpInPlaceDuration, 0.15f);                         
                }                
            }
            else
            {
                DOJumpY(jumpInPlaceHeight, jumpInPlaceDuration).OnKill(ResetPosition);// this.transform.DOJump(this.transform.position, jumpInPlaceHeight, 1, jumpInPlaceDuration).OnKill(ResetPosition);
                ShakeRotation(jumpInPlaceDuration, 0.15f);           
            }            
        }
    }

    IEnumerator JumpInPlaceLoop()
    {
        if (jumpInPlaceLoopDuration >= 0)
        {
            yield return new WaitForSeconds(jumpInPlaceLoopDuration);
            JumpInPlaceSequencerReceive();
            StartCoroutine(JumpInPlaceLoop());
        }
    }

    private void JumpInPlaceSequencerReceive()
    {
        if (jumpInYTween != null)
            if (jumpInYTween.IsActive())
                if (jumpInYTween.IsPlaying())
                    JumpInPlaceSequencerSend();
        JumpInPlace(true);
    }
    private void JumpInPlaceSequencerSend()
    {
        ResetPosition();
        if (jumpInPlaceSequenceNextObject != null)
            jumpInPlaceSequenceNextObject.JumpInPlaceSequencerReceive();
        //ResetPosition();
    }

    void ResetPosition()
    {
        this.transform.DOMove(startPosition, 0.15f).SetUpdate(true);
    }

    public Sequence DOJumpY(float jumpPower, float duration)
    {
        Sequence s = DOTween.Sequence();

        if (!IsShakeValid(true, jumpInYTween))
            return s;
        
        float startPosY = this.transform.position.y;        

        s.Append(this.transform.DOMoveY(startPosY + jumpPower, duration / 2).SetEase(Ease.OutQuad));
        s.Append(this.transform.DOMoveY(startPosY, duration / 2).SetEase(Ease.OutQuad));

        jumpInYTween = s;

        return s;
    }
    #endregion
}
