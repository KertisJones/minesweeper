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

    public float punchWithControlsPower = 0f;
    public float punchWIthControlsDuration = 0f;
    public int punchWIthControlsVibrato = 10;
    public float punchWIthControlsElasticity = 1f;

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
        transform.DOKill();
    }

    void OnDestroy()
    {
        //GameManager.OnHardDropEvent -= HardDrop;
        GameManager.OnLineClearEvent -= _ => LineClear(_);
        GameManager.OnGameOverEvent -= GameOver;
        transform.DOKill();
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
        Debug.Log("LINE CLEAR in jiggle " + lines);
        /*if (!jiggleIsEnabled || this.transform == null)// || (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 0))
            return;*/

        //this.transform.DOShakeRotation(lines, new Vector3(0, 0, 20), 10, 90, true, ShakeRandomnessMode.Harmonic);
        int l = lines;
        if (l > 5)
            l = 5;
        Shake(l, l * 0.1f);
    }

    void GameOver()
    {
        Shake(2f, .5f);
        //this.transform.DOShakeRotation(2, new Vector3(0, 0, 20), 10, 90, true, ShakeRandomnessMode.Harmonic).SetUpdate(true);
    }

    void Shake(float duration, float strength)
    {
        if (this.gameObject == null)
            return;
        if (transform == null)// || (PlayerPrefs.GetInt("ScreenShakeEnabled", 1) == 0))
            return;
        if (!jiggleIsEnabled)
            return;
        this.transform.DOShakePosition(duration, new Vector3(strength, strength, 0));
        this.transform.DOShakeRotation(duration, new Vector3(0, 0, strength * 40));
        this.transform.DOShakeScale(duration, strength);
    }

}
