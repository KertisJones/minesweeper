using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsSceneSkip : MonoBehaviour
{
    public string nextScene = "";
    void OnEnable()
    {
        InputManager.Instance.anyKey.started += _ => NextScene();
    }
    
    void NextScene()
    {
        GetComponent<LoadNewScene>().OpenNewScene(nextScene);
    }
}
