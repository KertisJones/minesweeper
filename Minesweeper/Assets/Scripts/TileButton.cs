using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TileButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Tile tile;
    private InputManager inputManager;
    GameManager gm;
    bool hover = false;
    bool buttonRevealDown = false;
    bool buttonFlagDown = false;
    void Awake()
    {
        inputManager = InputManager.Instance;
    }
    private void Start() {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }
    void Update()
    {
        tile = GetComponentInParent<Tile>();
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    #region Input
    void OnEnable()
    {
        inputManager.revealTilePress.started += _ => PressReveal();
        inputManager.revealTilePress.canceled += _ => ReleaseReveal();
        inputManager.flagTilePress.started += _ => PressFlag();
        inputManager.flagTilePress.canceled += _ => ReleaseFlag();
        inputManager.chordTilePress.started += _ => PressChord();
    }
    void OnDisable()
    {
        inputManager.revealTilePress.started -= _ => PressReveal();
        inputManager.revealTilePress.canceled -= _ => ReleaseReveal();
        inputManager.flagTilePress.started -= _ => PressFlag();
        inputManager.flagTilePress.canceled -= _ => ReleaseFlag();
        inputManager.chordTilePress.started -= _ => PressChord();
    }
    void PressReveal()
    {
        if (gm == null)
            return;
        if (gm.isGameOver || gm.isPaused || tile == null)
            return;
        
        if (hover)
        {
            tile.Reveal();
            if (buttonFlagDown)
                tile.Chord();
        }
            
        buttonRevealDown = true;
    }
    void ReleaseReveal()
    {
        buttonRevealDown = false;
    }
    void PressFlag()
    {
        if (gm == null)
            return;
        if (gm.isGameOver || gm.isPaused || tile == null)
            return;        
        if (hover)
        {
            tile.FlagToggle();
            if (buttonRevealDown)
                tile.Chord();
        }
            
        buttonFlagDown = true;
    }
    void ReleaseFlag()
    {
        buttonFlagDown = false;
    }
    void PressChord()
    {
        if (gm == null)
            return;
        if (gm.isGameOver || gm.isPaused || tile == null)
            return;
        if (hover)
        {
            //tile.FlagToggle();
            tile.Chord();
        }        
    }
    #endregion



    public void OnPointerEnter(PointerEventData eventData) 
    {
        hover = true;
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        hover = false;
    }

}
