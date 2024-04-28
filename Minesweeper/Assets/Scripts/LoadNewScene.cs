using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class LoadNewScene : MonoBehaviour
{
    public Image blackScreen;

    public void Start()
    {
        blackScreen.gameObject.SetActive(true);
        blackScreen.DOFade(0, 3f).SetUpdate(true);
    }

    public void OpenNewScene(string newScene) 
    {
        Time.timeScale = 1;
        DOTween.Clear(true);
        DOTween.KillAll();
        SceneManager.LoadScene(newScene);
    }

    public void ReloadScene()
    {
        Time.timeScale = 1;
        DOTween.Clear(true);
        DOTween.KillAll();
        //DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
