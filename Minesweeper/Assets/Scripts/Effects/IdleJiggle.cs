using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;
using Unity.Mathematics;

public class IdleJiggle : MonoBehaviour
{
    private Transform myTransform;
    private Canvas parentCanvas;
    private Vector2 parentCanvasDisplaySize = Vector2.zero;

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
    private Sequence punchDownSequence;

    [SerializeField]
    private Vector3 startPositionLocal;
    [SerializeField]
    private Vector3 parentStartPosition = Vector3.zero;
    private Vector3 startScale;   
    private Vector3 startRotation; 
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
            GameManager.OnTileSolveOrLandEvent += MinorShake;
            GameManager.OnTSpinEvent += dir => PunchRotate(dir);

            GameManager.OnLeftStuckEvent += PressLeft;
            GameManager.OnRightStuckEvent += PressRight;
            GameManager.OnMinoLockEvent += ResetLean;
            //InputManager.Instance.leftPress.started += _ => PressLeft();
            InputManager.Instance.leftPress.canceled += _ => ReleaseLeft();
            //InputManager.Instance.rightPress.started += _ => PressRight();
            InputManager.Instance.rightPress.canceled += _ => ReleaseRight();            
        }
        GameManager.OnResetStartingPositionsEvent += SetNewStartingValues;
        GameManager.OnKillTweenEvent += ResetAll;
    }
    void OnDisable()
    {
        DisableEverything();
    }

    void OnDestroy()
    {
        DisableEverything();
    }

    void DisableEverything()
    {
        if (jiggleOnActionIsEnabled)
        {
            //GameManager.OnHardDropEvent -= HardDrop;
            GameManager.OnLineClearEvent -= _ => LineClear(_);
            GameManager.OnGameOverEvent -= GameOver;
            GameManager.OnHardDropEvent -= PunchDown;
            GameManager.OnTileSolveOrLandEvent -= MinorShake;
            GameManager.OnTSpinEvent -= dir => PunchRotate(dir);

            GameManager.OnLeftStuckEvent -= PressLeft;
            GameManager.OnRightStuckEvent -= PressRight;
            GameManager.OnMinoLockEvent -= ResetLean;
            //InputManager.Instance.leftPress.started -= _ => PressLeft();
            InputManager.Instance.leftPress.canceled -= _ => ReleaseLeft();
            //InputManager.Instance.rightPress.started -= _ => PressRight();
            InputManager.Instance.rightPress.canceled -= _ => ReleaseRight();
        }
        GameManager.OnResetStartingPositionsEvent -= SetNewStartingValues;
        GameManager.OnKillTweenEvent -= ResetAll;

        if (!this.gameObject.scene.isLoaded) 
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
        parentCanvas = this.gameObject.GetComponentInParent<Canvas>();


        if (parentCanvas != null)
            if (parentCanvas.renderMode == RenderMode.WorldSpace)
                parentCanvasDisplaySize = parentCanvas.renderingDisplaySize;

        SetNewStartingValues();

        if (myTransform == null)// || (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 0))
            return;
        if (!jiggleIsEnabled)
            return;
        if (!this.gameObject.activeInHierarchy)
            return;



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
            Shake(lines * 0.2f, 0.15f, false);
    }

    void GameOver()
    {
        transform.DOKill();
        if (jiggleOnActionIsEnabled)
            Shake(3f, 0.25f, true);
    }

    void PunchDown()
    {
        if (!IsShakeValid(jiggleLeanIsEnabled, null))
            return;
        if (jumpInPlaceHeight == 0)
            return;


        punchDownSequence = DOTween.Sequence();
        
        if (jumpInPlaceLoopDuration > 0)
        {
            if (jumpInYTween != null)
                if (jumpInYTween.IsActive())
                    if (jumpInYTween.IsPlaying())
                        return;

            punchDownSequence.Append(DOJumpY(jumpInPlaceHeight + .3f, jumpInPlaceDuration * 1.5f).OnPlay(() => ShakeRotation(jumpInPlaceDuration * 1.5f, .15f, true, true)).OnComplete(JumpInPlaceSequencerSend));
        }
        else
        {
            /*if (jumpInYTween != null)
                if (jumpInYTween.IsActive())
                    if (jumpInYTween.IsPlaying())
                    {
                        jumpInYTween.Kill();
                        jumpInYTween = null;
                    }*/

            punchDownSequence.Append(DOJumpY(jumpInPlaceHeight * -1, jumpInPlaceHeight));
        }
        punchDownSequence.Append(this.transform.DOMoveY(GetStartPositionLocalToWorldSpace().y, 0.05f));
        
    }

    void MinorShake()
    {
        Shake(0.1f, 0.05f);
    }

    public void PressLeft()
    {
        if (buttonLeftHeld || buttonLeftHeldSecondary)
            return;

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
        if (buttonRightHeld || buttonRightHeldSecondary)
            return;

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
    void ResetLean()
    {
        buttonLeftHeld = false;
        buttonRightHeld = false;
        buttonLeftHeldSecondary = false;
        buttonRightHeldSecondary = false;
        LeanX(0);
    }
    #endregion

    public void Shake(float duration, float strength, bool autoReset = false, bool overrideTween = false, bool loopTween = false)
    {        
        ShakePosition(duration, strength, autoReset, overrideTween, loopTween);
        ShakeRotation(duration, strength, autoReset, overrideTween, loopTween);
        ShakeScale(duration, strength, autoReset, overrideTween, loopTween);
    }

    public void ShakePosition(float duration, float strength, bool autoReset = false, bool overrideTween = false, bool loopTween = false)
    {
        if (overrideTween)
            ShakePositionKill();
        
        if (!IsShakeValid(jiggleMoveIsEnabled, shakePositionTween))
            return;
        shakePositionTween = this.transform.DOShakePosition(duration, new Vector3(strength, strength, 0), 10, 90, false, !loopTween);
        if (autoReset)
            shakePositionTween.OnKill(ResetPosition);
        if (loopTween)
            shakePositionTween.SetLoops(-1);
    }

    public void ShakeRotation(float duration, float strength, bool autoReset = false, bool overrideTween = false, bool loopTween = false)
    {
        if (overrideTween)
            ShakeRotationKill();
        
        if (!IsShakeValid(jiggleRotateIsEnabled, shakeRotationTween))
            return;
        shakeRotationTween = this.transform.DOShakeRotation(duration, new Vector3(0, 0, strength * 40), 10, 90, !loopTween).SetUpdate(true);
        if (autoReset)
        {
            //startRotation = this.transform.rotation.eulerAngles;// new Vector3(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z);
            shakeRotationTween.OnKill(ResetRotation);
        }            
        if (loopTween)
            shakeRotationTween.SetLoops(-1);
    }

    public void ShakeScale(float duration, float strength, bool autoReset = false, bool overrideTween = false, bool loopTween = false)
    {
        if (overrideTween)
            ShakeScaleKill();
        
        if (!IsShakeValid(jiggleScaleIsEnabled, shakeScaleTween))
            return;
        shakeScaleTween = this.transform.DOShakeScale(duration, strength, 10, 90, !loopTween);
        if (autoReset)
            shakeScaleTween.OnKill(ResetScale);
        if (loopTween)
            shakeScaleTween.SetLoops(-1);
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
    public void ShakeKill()
    {
        ShakePositionKill();
        ShakeRotationKill();
        ShakeScaleKill();
    }
    private void ShakePositionKill() 
    {
        if (shakePositionTween != null)
            if (shakePositionTween.IsActive())
                if (shakePositionTween.IsPlaying())
                    {
                        shakePositionTween.Kill();
                        shakePositionTween = null;
                    }
    }

    private void ShakeRotationKill()
    {
        if (shakeRotationTween != null)
            if (shakeRotationTween.IsActive())
                if (shakeRotationTween.IsPlaying())
                    {
                        shakeRotationTween.Kill();
                        shakeRotationTween = null;
                    }
    }

    private void ShakeScaleKill() 
    {
        if (shakeScaleTween != null)
            if (shakeScaleTween.IsActive())
                if (shakeScaleTween.IsPlaying())
                    {
                        shakeScaleTween.Kill();
                        shakeScaleTween = null;
                    }
    }
    void LeanXKill()
    {
        if (leanTweenX != null)
            if (leanTweenX.IsActive())
                if (leanTweenX.IsPlaying())
                    leanTweenX.Kill();
    }

    void LeanYKill()
    {
        if (leanTweenY != null)
            if (leanTweenY.IsActive())
                if (leanTweenY.IsPlaying())
                    leanTweenY.Kill();
    }

    public void LeanX(int dir)
    {
        if (!IsShakeValid(jiggleLeanIsEnabled, null))
            return;

        LeanXKill();

        leanTweenX = this.transform.DOMoveX(GetStartPositionLocalToWorldSpace().x + leanDistance * dir, leanDuration).SetEase(idleMoveEase);
    }

    public void LeanY(int dir)
    {
        if (!IsShakeValid(jiggleLeanIsEnabled, null))
            return;

        LeanYKill();

        leanTweenY = this.transform.DOMoveY(GetStartPositionLocalToWorldSpace().y + leanDistance * dir, leanDuration).SetEase(idleMoveEase);
    }

    public void PunchRotate(int dir)
    {
        if (!IsShakeValid(true))
            return;
        
        this.transform.DOPunchRotation(new Vector3(0, 0, -1 * dir), leanDuration).SetUpdate(true).OnKill(ResetRotation);
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
                    
                    ShakeScale(jumpInPlaceDuration, 0.15f, false);
                    ShakeRotation(jumpInPlaceDuration, 0.15f, false);                         
                }                
            }
            else
            {
                DOJumpY(jumpInPlaceHeight, jumpInPlaceDuration).OnKill(ResetPositionY);// this.transform.DOJump(this.transform.position, jumpInPlaceHeight, 1, jumpInPlaceDuration).OnKill(ResetPosition);
                ShakeRotation(jumpInPlaceDuration, 0.15f, false);           
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
        ResetPositionY();
        if (jumpInPlaceSequenceNextObject != null)
            jumpInPlaceSequenceNextObject.JumpInPlaceSequencerReceive();
        //ResetPosition();
    }

    void ResetAll()
    {
        if (myTransform == null)
            return;

        //ShakeKill();
        ShakePositionKill();
        LeanXKill();
        LeanYKill();
        JumpYKill();
        PunchDownKill();
        this.transform.localPosition = startPositionLocal;
        this.transform.localScale = startScale;
        this.transform.localRotation = Quaternion.Euler(startRotation);
    }
    void ResetPosition()
    {
        ResetPositionX();
        ResetPositionY();
        this.transform.DOMoveZ(GetStartPositionLocalToWorldSpace().z, 0.15f).SetUpdate(true);
    }
    void ResetPositionX()
    {
        this.transform.DOMoveX(GetStartPositionLocalToWorldSpace().x, 0.15f).SetUpdate(true);
    }
    void ResetPositionY()
    {
        this.transform.DOMoveY(GetStartPositionLocalToWorldSpace().y, 0.15f).SetUpdate(true);
    }
    void ResetScale()
    {
        this.transform.DOScale(startScale, 0.15f).SetUpdate(true);        
    }
    void ResetRotation()
    {
        this.transform.DORotate(startRotation, 0.15f).SetUpdate(true);
    }
    void JumpYKill()
    {
        if (jumpInYTween != null)
            if (jumpInYTween.IsActive())
                if (jumpInYTween.IsPlaying())
                {
                    jumpInYTween.Kill();
                    jumpInYTween = null;
                }        
    }
    void PunchDownKill()
    {
        if (punchDownSequence != null)
            if (punchDownSequence.IsActive())
                if (punchDownSequence.IsPlaying())
                {
                    punchDownSequence.Kill();
                    punchDownSequence = null;
                }
    }

    public Sequence DOJumpY(float jumpPower, float duration, bool overrideTween = false)
    {
        if (overrideTween)
            JumpYKill();

        Sequence s = DOTween.Sequence();

        if (!IsShakeValid(true, jumpInYTween))
            return s;
        
        float startPosY = GetStartPositionLocalToWorldSpace().y;        

        s.Append(this.transform.DOMoveY(startPosY + jumpPower, duration / 2).SetEase(Ease.OutQuad));
        s.Append(this.transform.DOMoveY(startPosY, duration / 2).SetEase(Ease.OutQuad));

        jumpInYTween = s;

        return s;
    }
    #endregion
    
    public void SetNewStartingValues()
    {
        startPositionLocal = this.transform.localPosition;
        if (this.transform.parent != null )
            parentStartPosition = this.transform.parent.position;
        startScale = this.transform.localScale;
        startRotation = this.transform.rotation.eulerAngles;
        //startRotation = new Vector3(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z);
    }
    Vector3 GetStartPositionLocalToWorldSpace()
    {
        if (this.transform.parent == null)
            return startPositionLocal;

        Vector3 parentPositionToCenterAt = this.transform.parent.position;
        Vector2 displaySizeScale = Vector2.one;
        /*if (parentCanvas != null)
            if (parentCanvas.renderMode == RenderMode.WorldSpace)
            {
                parentPositionToCenterAt = parentStartPosition;
                displaySizeScale = new Vector2 (parentCanvas.renderingDisplaySize.x / parentCanvasDisplaySize.x, parentCanvas.renderingDisplaySize.y / parentCanvasDisplaySize.y);
            }*/


        return new Vector3(startPositionLocal.x * this.transform.parent.lossyScale.x * displaySizeScale.x,
        startPositionLocal.y * this.transform.parent.lossyScale.y * displaySizeScale.y,
        startPositionLocal.z * this.transform.parent.lossyScale.z) + parentPositionToCenterAt;
    }
}
