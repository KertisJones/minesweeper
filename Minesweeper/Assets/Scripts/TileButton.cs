using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Tile;

public class TileButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //test
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
    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        tile = GetComponentInParent<Tile>();
    }
    void Update()
    {
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
        inputManager.chordFlagTilePress.started += _ => PressChordFlag();
    }
    void OnDisable()
    {
        inputManager.revealTilePress.started -= _ => PressReveal();
        inputManager.revealTilePress.canceled -= _ => ReleaseReveal();
        inputManager.flagTilePress.started -= _ => PressFlag();
        inputManager.flagTilePress.canceled -= _ => ReleaseFlag();
        inputManager.chordTilePress.started -= _ => PressChord();
        inputManager.chordFlagTilePress.started -= _ => PressChordFlag();
    }
    void PressReveal()
    {
        if (gm == null || tile == null || !CanMinesweepInput())
            return;        
        
        if (hover)
        {
            if (tile.aura == Tile.AuraType.burning)
            {
                tile.PlaySoundSteamHiss();
            }
            else if (tile.aura == AuraType.frozen)
            {
                tile.PlaySoundFrozenHit();
            }
            else if (tile.aura == AuraType.wet)
            {
                tile.PlaySoundSwim();
            }

            if (gm.isGameOver || gm.isPaused)
                return;

            tile.Reveal(false, true);
            
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
        if (gm == null || tile == null || !CanMinesweepInput())
            return;
                
        if (hover)
        {
            if (tile.aura == Tile.AuraType.burning)
            {
                tile.PlaySoundSteamHiss();
            }
            else if (tile.aura == AuraType.frozen)
            {
                tile.PlaySoundFrozenHit();
            }
            else if (tile.aura == AuraType.wet)
            {
                tile.PlaySoundSwim();
            }

            if (gm.isGameOver || gm.isPaused)
                return;

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
        if (gm == null || tile == null || !CanMinesweepInput())
            return;
        
        if (hover && tile.isRevealed)
        {
            if (tile.aura == Tile.AuraType.burning)
            {
                tile.PlaySoundSteamHiss();
            }
            else if (tile.aura == AuraType.frozen)
            {
                tile.PlaySoundFrozenHit();
                return;
            }

            if (gm.isGameOver || gm.isPaused)
                return;

            //tile.FlagToggle();
            tile.Chord();
        }        
    }
    void PressChordFlag()
    {
        if (gm == null || tile == null || !CanMinesweepInput())
            return;
        
        if (hover && tile.isRevealed)
        {
            if (tile.aura == Tile.AuraType.burning)
            {
                tile.PlaySoundSteamHiss();
            }
            else if (tile.aura == AuraType.frozen)
            {
                tile.PlaySoundFrozenHit();
                return;
            }

            if (gm.isGameOver || gm.isPaused)
                return;

            //tile.FlagToggle();
            tile.ChordFlag();
        }        
    }
    #endregion

    bool CanMinesweepInput()
    {
        if (!gm.isStarted)
            return false;
        if (Time.time - gm.lastLineClearTime >= PlayerPrefs.GetFloat("LineClearPreventMinesweepDelay", 50) / 1000)
            return true;
        return false;
    }


    public void OnPointerEnter(PointerEventData eventData) 
    {
        hover = true;
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        hover = false;
    }

}
