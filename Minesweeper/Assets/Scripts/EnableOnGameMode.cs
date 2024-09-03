using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnGameMode : MonoBehaviour
{
    GameModifiers gameMods;
    GameManager gm;
    public bool inverseToDisableOnGameMode = false;
    public bool onShowTitle = false;
    public bool onShowCredits = false;
    public bool onEndlessIsEnabled = false;

    void OnEnable()
    {
        if (onShowTitle)
        {
            GameManager.OnLineClearEvent += _ => DropTitle();
            GameManager.OnGameOverEvent += DropTitle;
            GameManager.OnHardDropEvent += DropTitle;
        }        
    }
    void OnDisable()
    {
        if (onShowTitle)
        {
            GameManager.OnLineClearEvent -= _ => DropTitle();
            GameManager.OnGameOverEvent -= DropTitle;
            GameManager.OnHardDropEvent -= DropTitle;
        }        
    }

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        gameMods = gm.gameMods;
        

        bool isEnabled = false;
        if (onShowTitle && gameMods.showTitle)
            isEnabled = true;
        if (onShowCredits && gameMods.minesweeperTextType == GameModifiers.MinesweeperTextType.credits)
            isEnabled = true;
        if (onEndlessIsEnabled && gameMods.endlessIsEnabled)
            isEnabled = true;
        
        // INVERSE?
        if (inverseToDisableOnGameMode)
            isEnabled = !isEnabled;
        
        this.gameObject.SetActive(isEnabled);
    }

    void DropTitle()
    {
        if (GetComponentInChildren<SpringJoint2D>() != null)
            GetComponentInChildren<SpringJoint2D>().breakForce = 0;
    }
}
