using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LoadNewScene : MonoBehaviour
{
    public String newSceneName;

    public void OpenNewScene () 
    {
        DOTween.Clear(true);
        SceneManager.LoadScene(newSceneName);
    }

    public void ReloadScene()
    {
        Time.timeScale = 1;
        DOTween.Clear(true);
        //DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
