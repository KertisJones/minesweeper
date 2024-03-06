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
    void OnEnable()
    {
        //GameManager.OnHardDropEvent += HardDrop;
        GameManager.OnLineClearEvent += _ => LineClear(_);
        GameManager.OnGameOverEvent += GameOver;
    }
    void OnDisable()
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

    void OnDestroy()
    {
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

        //transform.DOKill(); when destroyed
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
        //Debug.Log("LINE CLEAR in jiggle " + lines);
        /*if (!jiggleIsEnabled || this.transform == null)// || (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 0))
            return;*/

        //this.transform.DOShakeRotation(lines, new Vector3(0, 0, 20), 10, 90, true, ShakeRandomnessMode.Harmonic);
        /*int l = lines;
        if (l > 8)
            l = 8;*/
        Shake(lines * 0.2f, 0.15f);
    }

    void GameOver()
    {
        Shake(3f, .25f);
        //this.transform.DOShakeRotation(2, new Vector3(0, 0, 20), 10, 90, true, ShakeRandomnessMode.Harmonic).SetUpdate(true);
    }

    public void Shake(float duration, float strength)
    {        
        ShakePosition(duration, strength);
        ShakeRotation(duration, strength);
        ShakeScale(duration, strength);
    }

    public void ShakePosition(float duration, float strength)
    {
        if (transform == null)// || (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 0))
            return;
        if (!jiggleIsEnabled)
            return;
        if (shakePositionTween != null)
            if (shakePositionTween.IsPlaying())
                return;
        if (this.transform.localScale == Vector3.zero)
            return;
        shakePositionTween = this.transform.DOShakePosition(duration, new Vector3(strength, strength, 0));
    }

    public void ShakeRotation(float duration, float strength)
    {
        if (transform == null)// || (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 0))
            return;
        if (!jiggleIsEnabled)
            return;
        if (shakeRotationTween != null)
            if (shakeRotationTween.IsPlaying())
                return;
        if (this.transform.localScale == Vector3.zero)
            return;
        shakeRotationTween = this.transform.DOShakeRotation(duration, new Vector3(0, 0, strength * 40));
    }

    public void ShakeScale(float duration, float strength)
    {
        if (transform == null)// || (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 0))
            return;
        if (!jiggleIsEnabled)
            return;
        if (shakeScaleTween != null)
            if (shakeScaleTween.IsPlaying())
                return;
        if (this.transform.localScale == Vector3.zero)
            return;
        shakeScaleTween = this.transform.DOShakeScale(duration, strength);
    }

}
