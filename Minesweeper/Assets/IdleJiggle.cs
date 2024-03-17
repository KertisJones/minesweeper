using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;

public class IdleJiggle : MonoBehaviour
{
    public Vector3 idleMoveDistance = Vector3.zero;
    public float idleMoveDuration = 0f;
    public Ease idleMoveEase = Ease.InOutSine;

    private Tween shakePositionTween;
    private Tween shakeRotationTween;
    private Tween shakeScaleTween;

    /*public float punchWithControlsPower = 0f;
    public float punchWIthControlsDuration = 0f;
    public int punchWIthControlsVibrato = 10;
    public float punchWIthControlsElasticity = 1f;*/
    
    public bool jiggleIsEnabled = true;
    public bool jiggleRotateIsEnabled = true;
    public bool jiggleScaleIsEnabled = true;
    public bool jiggleMoveIsEnabled = true;
    public bool jiggleOnActionIsEnabled = true;
    void OnEnable()
    {
        if (jiggleOnActionIsEnabled)
        {
            //GameManager.OnHardDropEvent += HardDrop;
            GameManager.OnLineClearEvent += _ => LineClear(_);
            GameManager.OnGameOverEvent += GameOver;
        }        
    }
    void OnDisable()
    {
        if (jiggleOnActionIsEnabled)
        {
            //inputManager.hardDroptPress.started -= _ => PressHardDrop();
            //GameManager.OnHardDropEvent -= HardDrop;
            GameManager.OnLineClearEvent -= _ => LineClear(_);
            GameManager.OnGameOverEvent -= GameOver;
            if(!this.gameObject.scene.isLoaded) 
                return;
            transform.DOKill();
            shakePositionTween = null;
            shakeRotationTween = null;
            shakeScaleTween = null;
        }
    }

    void OnDestroy()
    {
        if (jiggleOnActionIsEnabled)
        {
            //GameManager.OnHardDropEvent -= HardDrop;
            GameManager.OnLineClearEvent -= _ => LineClear(_);
            GameManager.OnGameOverEvent -= GameOver;
        }
        if(!this.gameObject.scene.isLoaded) 
            return;
        transform.DOKill();
        shakePositionTween = null;
        shakeRotationTween = null;
        shakeScaleTween = null;        
    }

    // Start is called before the first frame update
    void Start()
    {
        if (transform == null)// || (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 0))
            return;
        if (!jiggleIsEnabled)
            return;
        
        if (idleMoveDistance != Vector3.zero && idleMoveDuration > 0)
        {
            this.transform.DOMove(transform.position + idleMoveDistance, idleMoveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(idleMoveEase);
        }
    }

    /*void HardDrop()
    {
        if (!jiggleIsEnabled || transform == null)// || (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 0))
            return;

        if (punchWithControlsPower != 0 && punchWIthControlsDuration > 0)
        {
            this.transform.DOPunchPosition(new Vector3(0, punchWithControlsPower, 0), punchWIthControlsDuration, punchWIthControlsVibrato, punchWIthControlsElasticity);
        }
    }*/

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
        shakePositionTween = this.transform.DOShakePosition(duration, new Vector3(strength, strength, 0));
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
        if (transform == null)// || (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 0))
            return false;        
        if (!jiggleIsEnabled || !isJiggleTypeIsEnabled)
            return false;
        if (this.transform.localScale == Vector3.zero)
            return false;

        if (tweenToCheckIfPlaying != null)
            if (tweenToCheckIfPlaying.IsPlaying())
                return false;

        bool screenShake = (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) != 0);
        if (!screenShake)
            return false;

        return true;
    }

}
