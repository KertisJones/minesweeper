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
    // Start is called before the first frame update
    void Start()
    {
        gameMods = GameObject.FindGameObjectWithTag("ScoreKeeper").GetComponent<GameModifiers>();
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        bool isEnabled = false;
        if(onShowTitle && gameMods.showTitle)
            isEnabled = true;
        if(onShowCredits && gameMods.minesweeperTextType == GameModifiers.MinesweeperTextType.credits)
            isEnabled = true;
        
        // INVERSE?
        if (inverseToDisableOnGameMode)
            isEnabled = !isEnabled;
        
        this.gameObject.SetActive(isEnabled);
    }

    // Update is called once per frame
    void Update()
    {
        if (onShowTitle && gm.isGameOver)
        {
            if (GetComponentInChildren<SpringJoint2D>() != null)
                GetComponentInChildren<SpringJoint2D>().breakForce = 0;
        }
    }
}
