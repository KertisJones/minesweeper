using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.U2D;
using TMPro.Examples;
using DG.Tweening;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class Tile : MonoBehaviour
{
    public int coordX = 0;
    public int coordY = 0;
    private int coordXPrevious = 0;
    private int coordYPrevious = 0;

    public bool isMine = false;
    public bool isFlagged = false;
    //public bool isQuestioned = false;
    public bool isRevealed = false;
    public bool isDisplay = false;
    public int nearbyMines = 0;
    public int nearbyFlags = 0;

    /*public float screenShakeDuration = 0.1f;
    public float screenShakeStrength = 0.1f;*/
    public bool isDestroyed = false;
    public bool isRowSolved = false;
    public bool is8Triggered = false;
    public bool isFailedToChord = false;
    private bool revealedThisFrame = false;    

    // Auras
    public enum AuraType
    {
        normal,
        burning,
        frozen,
        wet
    }

    public AuraType aura = AuraType.normal;
    float burnTime = 15f;
    float meltTime = 15f;
    float putOutTime = 5f;
    float auraDecayClock = 0;
    private bool burnoutInvisible = false;
    float auraClock = 0;
    List<AuraType>  adjacentAuras = new List<AuraType>();
    public Material[] auraMaterials;    
    public AudioClip[] burningPutOutSteamHiss;
    public AudioClip[] burningBurnOutFlame;
    public AudioClip[] hardHitSounds;
    public AudioClip[] snowSounds;
    public AudioClip meltSound;
    public AudioClip boilSoftSound;
    public AudioClip fireSizzleSound;

    Tile tileToBurn;
    public AudioSource decaySoundSource;

    // Assets
    public Color solvedMarkColor;

    public AudioClip revealSound;
    public AudioClip flagSound;
    public AudioClip unflagSound;
    public AudioClip chordSound;
    public AudioClip chordFlagSound;
    public AudioClip chordFailSound;

    public SpriteRenderer tileBackground;
    public SpriteRenderer explodedMineBackground;
    public SpriteRenderer wrongFlagBackground;
    public SpriteMask shimmerOverlay;
    public Image fadeOverlay;
    public Image unrevealedButtonImage;
    public SpriteRenderer burnGameOverImage;
    public Image auraOverlayImage;
    public SpriteRenderer auraBackgroundOverlayImage;
    TextMeshProUGUI text;
    Light2D light;
    
    GameManager gm;
    Camera cam;
    HoldTetromino holdTetromino;
    GameModifiers gameMods;
    [HideInInspector]
    public Group group;

    void OnDestroy()
    {
        if (aura == AuraType.burning)
        {
            gm.numBurningTiles -= 1;
        }
        else if (aura == AuraType.frozen)
        {
            gm.numFrozenTiles -= 1;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        holdTetromino = GameObject.FindGameObjectWithTag("Hold").GetComponent<HoldTetromino>();
        gameMods = GameObject.FindGameObjectWithTag("ScoreKeeper").GetComponent<GameModifiers>();
                
        group = GetComponentInParent<Group>();
        unrevealedButtonImage = GetComponentInChildren<TileButton>().GetComponent<Image>();
        light = GetComponentInChildren<Light2D>();

        auraOverlayImage.color = unrevealedButtonImage.color;

        Vector2 v = GameManager.roundVec2(transform.position);
        coordX = (int)v.x;
        coordY = (int)v.y;
        
        CountMine();
        SetAura(aura);

        if (isDisplay)
        {
            GetComponentInChildren<Button>().interactable = false;
            GetComponent<ButtonJiggle>().scaleMultiplierEnlarge = ((GetComponent<ButtonJiggle>().scaleMultiplierEnlarge - 1) * 0.25f) + 1;
            tileBackground.enabled = true;
            //Debug.Log ("Display " + gameObject.name);
            //tileBackground.color = new Color(215, 215, 215, 255);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.isGameOver && isFlagged && !isMine)
            Reveal(true);

        if (gm.isGameOver)
            return;

        if (gm.cheatAutoFlagMode)
        {
            if (isMine)
                isFlagged = true;
            else
                Reveal();
        }

        UpdateLocation();
        UpdateAura();
        UpdateText();
        UpdateButtonJiggle(); 

        if (!isMine)
        {
            //DetectProximity();
            ZeroCascade();
        }
        
        if (!is8Triggered && nearbyMines == 8)
        {
            // You're an 8 -- a minesweeper unicorn! Give 'em some points!
            // Make sure this mino is locked
            if (!GetComponentInParent<Group>().isFalling)
            {
                // Make sure neighbors are locked
                bool neighborsAreLocked = true;
                foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
                {
                    if (t.GetComponentInParent<Group>().isFalling)
                        neighborsAreLocked = false;
                }
                if (neighborsAreLocked)
                {
                    gm.AddScore(8888, 2);
                    gm.SetScoreMultiplier(8, 1f, true);
                    if (gm.safeEdgeTilesGained < 8)
                        gm.AddSafeTileToEdges();
                    is8Triggered = true;
                }
            }            
        }
        
        // Triggered the frame after the row is solved
        if (isRowSolved && tileBackground.color != solvedMarkColor)
        {
            text.GetComponent<IdleJiggle>().Shake(30, 0.1f, true, true, true);
            if (GetComponentInChildren<SetRandomSupporterName>() != null)
                GetComponentInChildren<SetRandomSupporterName>().supportText.GetComponent<IdleJiggle>().Shake(30, 0.1f, true, true, true);;

            tileBackground.color = solvedMarkColor;
            shimmerOverlay.gameObject.SetActive(true);
        }            
        // Triggered the frame after the row is no longer solved
        else if (!isRowSolved && tileBackground.color == solvedMarkColor)
        {
            text.GetComponent<IdleJiggle>().ShakeKill();
            if (GetComponentInChildren<SetRandomSupporterName>() != null)
                GetComponentInChildren<SetRandomSupporterName>().supportText.GetComponent<IdleJiggle>().ShakeKill();
            tileBackground.color = Color.white;
            
            if (!isDisplay)
                shimmerOverlay.gameObject.SetActive(false);
        }

        revealedThisFrame = false;

        

        /*fallClock -= Time.deltaTime;
        if (fallClock <= 0)
        {
            fallClock = 1;
            Fall();
        }*/
    }

    public void CountMine() {
        if (isMine && gm != null)
            gm.currentMines += 1;
    }

    private void UpdateLocation()
    {
        this.transform.position = new Vector3(coordX, coordY, this.transform.position.z);

        if (coordX != coordXPrevious || coordY != coordYPrevious)
        {
            coordXPrevious = coordX;
            coordYPrevious = coordY;            
            this.name = "Tile (" + coordX + ", " + coordY + ")";

            if (aura != AuraType.normal)
            {
                List<AuraType> newAuras = new List<AuraType>();
                foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
                {
                    newAuras.Add(t.aura);
                }

                if ((aura == AuraType.frozen && newAuras.Contains(AuraType.burning))
                    || (aura == AuraType.burning && newAuras.Contains(AuraType.frozen)))
                {
                    PlaySoundSteamHiss();
                }
            }
        }
        
    }

    void UpdateText()
    {
        string myText = "";
        Color32 myColor = new Color32(0, 0, 0, 255);

        switch (nearbyMines)
        {
            case 1:
                myColor = new Color32(0, 0, 253, 255);
                break;
            case 2:
                myColor = new Color32(0, 126, 0, 255);
                break;
            case 3:
                myColor = new Color32(254, 0, 0, 255);
                break;
            case 4:
                myColor = new Color32(1, 0, 128, 255);
                break;
            case 5:
                myColor = new Color32(130, 1, 2, 255);
                break;
            case 6:
                myColor = new Color32(0, 128, 128, 255);
                break;
            case 7:
                myColor = new Color32(0, 0, 0, 255);
                break;
            case 8:
                myColor = new Color32(128, 128, 128, 255);
                break;
            default:
                myColor = new Color32(255, 255, 255, 255);
                break;
        }
        if (isMine)
            myColor = Color.black;

        if (isRevealed)
        {
            myText = nearbyMines.ToString();

            if (nearbyMines == 0)
                myText = "";
            if (isMine)
            {
                myText = "*";
                text.GetComponent<TextOutline>().DisableOutline();
            }
                
            else if (!gm.isTitleMenu) // During Credits, hide numbers
            {
                if (gameMods.minesweeperTextType == GameModifiers.MinesweeperTextType.credits)
                    myText = "";
                else if (gameMods.minesweeperTextType == GameModifiers.MinesweeperTextType.dots)
                    myText = "©";
            }
                
                
        }
        else
        {
            if (isFlagged)
                myText = "<sprite=0>";
            //else if (isQuestioned)
                //myText = "?";
            if (!isDisplay)
                myColor = Color.white;
        }

        if ((gm.isPaused && !gm.isGameOver && !gm.marathonOverMenu.GetIsActive()) || (burnoutInvisible && !isMine))
            myText = "";

        if (text != null)
        {
            if (text.text != myText)
                text.SetText(myText);
            text.color = myColor;
            if (GetComponentInChildren<SetRandomSupporterName>() != null)
                GetComponentInChildren<SetRandomSupporterName>().supportText.color = myColor;
        }        
    }

    public void FlagToggle()
    {
        if (gm.isGameOver || isRevealed || GetComponentInParent<Group>().isHeld || burnoutInvisible || aura == AuraType.frozen)
            return;        

        isFlagged = !isFlagged;
        if (isFlagged)
        {
            if (!isMine && aura == Tile.AuraType.burning)
            {
                AudioSource.PlayClipAtPoint(burningBurnOutFlame[Random.Range(0, burningBurnOutFlame.Length)], new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                burnGameOverImage.enabled = true;
                gm.EndGame();
                return;
            }

            //isQuestioned = false;

            //GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            gm.soundManager.PlayTileRevealSound(true);
            //AudioSource.PlayClipAtPoint(flagSound, new Vector3(0, 0, 0), 0.5f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
            //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);
            gm.TriggerOnTileSolveOrLandEvent();

            float sm = gm.GetScoreMultiplier();
            gm.SetScoreMultiplier(1, 1f, true);
            //Debug.Log(gameObject.name + ": " + sm + " + 0.01 = " + gm.GetScoreMultiplier());

            gm.currentFlags += 1;

            /*if (isMine)
            {
                if (!isFailedToChord)
                {
                    holdTetromino.AddToManualSolveStreak(true);
                }
            }                
            else
            {
                holdTetromino.ResetManualSolveStreak();
            }*/

            if (gm.lineClearInstantly)
                GameManager.deleteFullRows();
            GameManager.CheckForPossiblePerfectClear();            
        }
        else
        {
            //GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(unflagSound, new Vector3(0, 0, 0), 0.5f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
            gm.soundManager.ResetTileRevealPitch();

            gm.ResetScoreMultiplier();
            isFailedToChord = true; // Don't double count this tile for points
            //holdTetromino.ResetManualSolveStreak();
            gm.ResetRevealStreakManual();

            /*
            +30K, -70K
            +33k, -42k
            +23k, -14k
            +24k, -38k
            ......
            +70k, -32k
            +16k, -10k
            */

            gm.currentFlags -= 1;
        }
        GameManager.markSolvedRows();
    }

    /*public void QuestionToggle()
    {
        if (gm.isGameOver || isRevealed)
            return;

        isQuestioned = !isQuestioned;
        if (isQuestioned)
        {
            isFlagged = false;

            //GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(flagSound, new Vector3(0, 0, 0), 0.5f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
        }
        else
        {
            //GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(unflagSound, new Vector3(0, 0, 0), 0.5f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
        }
    }*/

    public void Reveal(bool isForcedReveal = false, bool isManual = false)
    {
        if (burnoutInvisible || aura == AuraType.frozen)
            return;

        

        if (!isRevealed && !isFlagged && !isDisplay && (!GetComponentInParent<Group>().isHeld || isForcedReveal))
        {           
            isRevealed = true;
            
            //isQuestioned = false;

            revealedThisFrame = true;
            //gm.RevealTile(coordX, coordY, nearbyMines, isMine);

            fadeOverlay.color =  new Color(0, 0, 0, 0);

            UpdateText();

            

            if (isMine)
            {
                if (!gm.isGameOver)
                {
                    auraOverlayImage.enabled = false;
                    auraBackgroundOverlayImage.enabled = false;
                    explodedMineBackground.enabled = true;
                }
                gm.EndGame();
            }
            else if (gm == null) // Error catching, can sometimes happen when the scene loads
            {
                Debug.LogWarning("Game Manager can't be found, I'll assume it's a new game");
            }
            else if (!gm.isGameOver)
            {
                //GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                gm.soundManager.PlayTileRevealSound(isManual);

                tileBackground.enabled = true;

                if (aura == AuraType.burning)
                {
                    auraOverlayImage.enabled = false;
                    auraBackgroundOverlayImage.enabled = true;

                }

                //AudioSource.PlayClipAtPoint(revealSound, new Vector3(0, 0, 0), 0.75f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));

                if (!isFailedToChord)
                {
                    holdTetromino.AddToCleanseRecharge();//.AddToManualSolveStreak();
                    if (isManual)
                        gm.AddRevealStreakManual();
                }
                    

                // Scoring
                if (!GetComponentInParent<Group>().isDisplay && !GetComponentInParent<Group>().isHeld)
                {
                    gm.AddRevealCombo(!isManual, isFailedToChord, coordY);
                    gm.SetScoreMultiplier(1, 1f, true);

                    // If the mino is falling, don't give a huge bonus for being revealed in the air
                    //if (GetComponentInParent<Group>().isFalling)
                    //{
                        //gm.AddScore(nearbyMines, 2, false); // * nearbyMines
                        /*holdTetromino.scoreMissingTest += Mathf.FloorToInt(((nearbyMines * nearbyMines) - nearbyMines) * gm.level * (1 + gm.GetScoreMultiplier()));
                        holdTetromino.scoreRevealRemainingTest += Mathf.FloorToInt(nearbyMines * gm.level * (1 + gm.GetScoreMultiplier()));                        
                        holdTetromino.scoreRevealTest += Mathf.FloorToInt(((nearbyMines * nearbyMines)) * gm.level * (1 + gm.GetScoreMultiplier()));
                        holdTetromino.scoreRevealNoLevelTest += Mathf.FloorToInt(nearbyMines * (1 + gm.GetScoreMultiplier()));*/
                    //}                        
                    //else
                    //{
                        //gm.AddScore(nearbyMines * (coordY + 1), 2, false); // * nearbyMines
                        /*holdTetromino.scoreMissingTest += Mathf.FloorToInt(((nearbyMines * nearbyMines * (coordY + 1)) - (nearbyMines * (coordY + 1))) * gm.level * (1 + gm.GetScoreMultiplier()));
                        holdTetromino.scoreRevealRemainingTest += Mathf.FloorToInt((nearbyMines * (coordY + 1)) * gm.level * (1 + gm.GetScoreMultiplier()));
                        holdTetromino.scoreRevealTest += Mathf.FloorToInt(((nearbyMines * nearbyMines * (coordY + 1))) * gm.level * (1 + gm.GetScoreMultiplier()));
                        holdTetromino.scoreRevealNoLevelTest += Mathf.FloorToInt((nearbyMines * (coordY + 1)) * (1 + gm.GetScoreMultiplier()));*/
                    //}
                }
            }

            //DetectProximity();
            ZeroCascade();
            GetComponent<ButtonJiggle>().scaleMultiplierEnlarge = ((GetComponent<ButtonJiggle>().scaleMultiplierEnlarge - 1) * 0.25f) + 1;

            GetComponentInChildren<Button>().interactable = false;

            //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);
            gm.TriggerOnTileSolveOrLandEvent();

            if (gm.lineClearInstantly)
                GameManager.deleteFullRows();
            GameManager.markSolvedRows();
            GameManager.CheckForPossiblePerfectClear();
        }
        else if (isFlagged && gm.isGameOver && !isMine)
        {
            isRevealed = true;
            //explodedMineBackground.enabled = true;
            wrongFlagBackground.enabled = true;
            tileBackground.enabled = true;
            UpdateText();

            ZeroCascade();
            GetComponentInChildren<Button>().interactable = false;
        }
    }

    // When this mine has no adjacent mines, all adjacent tiles should be revealed.
    void ZeroCascade()
    {
        if (gm.isGameOver)
            return;
        DetectProximity();
        if (nearbyMines == 0 && !isMine && isRevealed)
        {
            foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
            {
                if (!t.isMine)
                    t.Reveal();
            }
        }
    }

    // When an uncovered square with a number has exactly the correct number of adjacent squares flagged, performing a click on it will uncover all unmarked squares.
    public void Chord()
    {
        DetectProximity();

        if (GetComponentInParent<Group>() != null)
            if (GetComponentInParent<Group>().isHeld)
                return;


        //Debug.Log("Attempting Chord...");
        if (isRevealed && !isMine)
        {
            if (nearbyFlags == nearbyMines) // Chord
            {
                //AudioSource.PlayClipAtPoint(chordSound, new Vector3(0, 0, 0), 0.5f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));

                bool frozenChord = false;
                foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
                {
                    if (!t.isFlagged)
                    {
                        if (!t.isDisplay)    
                        {
                            t.Reveal(false, true);      
                            if (t.aura == AuraType.frozen)
                                frozenChord = true;
                        }
                    }                                        
                }
                if (frozenChord && nearbyMines > 0)
                {
                    PlaySoundFrozenHit();
                }
                    
            }
            else if (!revealedThisFrame) // Failed Chord
            {                
                bool realFail = false;
                AudioSource.PlayClipAtPoint(chordFailSound, new Vector3(0, 0, 0), 0.5f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                gm.soundManager.ResetTileRevealPitch();

                foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
                {
                    if (!t.isFlagged)
                    {
                        if (!t.isDisplay)
                        {
                            t.isFailedToChord = true;
                            realFail = true;                            
                        }
                    }                                        
                }
                if (realFail)
                {                    
                    //holdTetromino.ResetManualSolveStreak();
                    gm.ResetRevealStreakManual();

                    if (aura == Tile.AuraType.burning)
                    {
                        AudioSource.PlayClipAtPoint(burningBurnOutFlame[Random.Range(0, burningBurnOutFlame.Length)], new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                        burnGameOverImage.enabled = true;
                        gm.EndGame();
                    }
                }
            }
            
        }

        //ChordFlag();
        //else
            //Debug.Log("Chord Failed.");
    }

    public void ChordFlag()
    {
        DetectProximity();

        if (GetComponentInParent<Group>() != null)
            if (GetComponentInParent<Group>().isHeld)
                return;

        //Debug.Log("Attempting Flag Chord...");
        if (isRevealed && !isMine)
        {
            ArrayList adjacentUnopenedTiles = new ArrayList();

            foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
            {
                if (!t.isDisplay && !t.isRevealed)
                {
                    adjacentUnopenedTiles.Add(t);
                }                                        
            }

            if (nearbyMines == adjacentUnopenedTiles.Count) // Flag Chord
            {
                //AudioSource.PlayClipAtPoint(chordFlagSound, new Vector3(0, 0, 0), 0.5f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));

                bool frozenChord = false;
                foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
                {
                    if (!t.isFlagged)
                    {
                        t.FlagToggle();
                        if (t.aura == AuraType.frozen)
                            frozenChord = true;
                    }                                        
                }
                if (frozenChord && nearbyMines > 0)
                {
                    PlaySoundFrozenHit();
                }
            }
            else // Failed Chord
            {
                bool realFail = false;                
                AudioSource.PlayClipAtPoint(chordFailSound, new Vector3(0, 0, 0), 0.5f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                gm.soundManager.ResetTileRevealPitch();

                foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
                {
                    if (!t.isFlagged)
                    {
                        t.isFailedToChord = true;
                        if (t.aura != AuraType.frozen)
                            realFail = true;                        
                    }                                        
                }
                if (realFail)
                {                    
                    //holdTetromino.ResetManualSolveStreak();
                    gm.ResetRevealStreakManual();

                    if (aura == Tile.AuraType.burning)
                    {
                        AudioSource.PlayClipAtPoint(burningBurnOutFlame[Random.Range(0, burningBurnOutFlame.Length)], new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                        burnGameOverImage.enabled = true;
                        gm.EndGame();
                    }
                }
            }
            
        }
        //else
            //Debug.Log("Flag Chord Failed.");
    }

    void DetectProximity()
    {
        int nearbyMinesTemp = 0;
        int nearbyFlagsTemp = 0;
        foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
        {
            if (t.isMine)
                nearbyMinesTemp += 1;
            if (t.isFlagged)
                nearbyFlagsTemp += 1;
        }
        nearbyMines = nearbyMinesTemp;
        nearbyFlags = nearbyFlagsTemp;
        //if (nearbyMines == 0)
            //ZeroCascade();
        
    }

    private void UpdateButtonJiggle() 
    {
        bool shouldButtonJiggleBeActive = CheckButtonJiggle();
        if (shouldButtonJiggleBeActive != GetComponent<ButtonJiggle>().jiggleIsEnabled)
            GetComponent<ButtonJiggle>().jiggleIsEnabled = shouldButtonJiggleBeActive;
    }
    private bool CheckButtonJiggle()
    {
        if (!isRevealed)
        {
            // If the tile is held, jiggle is off
            if (GetComponentInParent<Group>() != null)
                if (GetComponentInParent<Group>().isHeld)
                    return false;
            // If the tile is flagged, jiggle is off
            if (isFlagged)
                return false;
            // if the tile is not revealed, jiggle is on                        
            return true;
        }
        /*else
        {           
            DetectProximity();
            // If the tile is revealed, only jiggle if its
            foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
            {
                if (!t.isRevealed && !t.isFlagged)
                    return true;
            }
        }*/
        return false;
    }

    /*void Fall()
    {
        if (coordY > 0)
        {
            Debug.Log("fall");
            gm.MoveTile(this.gameObject, coordX, coordY - 1);
        }
    }*/

    #region Auras
    public void SetAura(AuraType newAura)
    {
        // Transition Count and Sound Effects!
        if (aura == AuraType.burning && newAura != AuraType.burning)
        {
            gm.numBurningTiles -= 1;
            PlaySoundSteamHiss();
        }
        else if (aura != AuraType.burning && newAura == AuraType.burning)
        {
            gm.numBurningTiles += 1;
        }

        if (aura == AuraType.frozen && newAura != AuraType.frozen)
        {
            gm.numFrozenTiles -= 1;            
        }
        else if (aura != AuraType.frozen && newAura == AuraType.frozen)
        {
            gm.numFrozenTiles += 1;
        }

        // Change Aura
        aura = newAura;
        auraClock = 0;
        auraDecayClock = 0;

        // Has Outline
        if (aura == AuraType.burning || aura == AuraType.frozen)
            this.transform.position += new Vector3(this.transform.position.x, this.transform.position.y, -0.05f);
        else
            this.transform.position += new Vector3(this.transform.position.x, this.transform.position.y, 0f);

        // Text needs outline
        if (aura == AuraType.burning)
            text.GetComponent<TextOutline>().EnableOutline();
        else
            text.GetComponent<TextOutline>().DisableOutline();

        // Tile Emits Light
        if (aura == AuraType.burning)
        {
            light.intensity = .25f;
        }
        else
        {
            light.intensity = 0f;
        }

        // Decay Sounds
        if (decaySoundSource.isPlaying)
        {
            decaySoundSource.Stop();
            decaySoundSource.volume = 0;
        }


        Material auraMaterialLocal = new Material(auraMaterials[(int)aura]);
        unrevealedButtonImage.material = auraMaterialLocal;
        tileBackground.material = auraMaterialLocal;
        explodedMineBackground.material = auraMaterialLocal;
        auraOverlayImage.material = auraMaterialLocal;
        auraBackgroundOverlayImage.material = auraMaterialLocal;

        if (aura == AuraType.frozen)
        {
            auraOverlayImage.enabled = true;
            unrevealedButtonImage.material = new Material(auraMaterials[(int)AuraType.wet]);
            unrevealedButtonImage.material.SetFloat("_OverlayTextureScrollXSpeed", 0);
            unrevealedButtonImage.material.SetFloat("_OverlayTextureScrollYSpeed", 0);
            unrevealedButtonImage.material.SetFloat("_ColorSwapBlend", 0.5f);
        }
        else if (aura == AuraType.burning)
        {
            
            unrevealedButtonImage.material = new Material(auraMaterials[(int)AuraType.normal]);
            tileBackground.material = new Material(auraMaterials[(int)AuraType.normal]);
            /*unrevealedButtonImage.material.SetFloat("_OverlayTextureScrollXSpeed", 0);
            unrevealedButtonImage.material.SetFloat("_OverlayTextureScrollYSpeed", 0);
            unrevealedButtonImage.material.SetFloat("_ColorSwapBlend", 0.5f);*/
            if (!isRevealed)
                auraOverlayImage.enabled = true;
            else
                auraBackgroundOverlayImage.enabled = true;

        }
        else
        {
            auraOverlayImage.enabled = false;
            auraBackgroundOverlayImage.enabled = false;
        }
    }
    private void UpdateAura()
    {
        if (aura == AuraType.normal)
            return;

        List<AuraType> newAuras = new List<AuraType>();
        foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
        {
            newAuras.Add(t.aura);
        }
        adjacentAuras = newAuras;

        if (aura == AuraType.burning)
            UpdateAuraBurning();
        else if (aura == AuraType.frozen)
            UpdateAuraFrozen();
    }

    private void UpdateAuraBurning()
    {
        foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
        {
            if (t.aura == AuraType.wet)
            {
                //SetAura(AuraType.normal);
                //t.SetAura(AuraType.normal);
                auraDecayClock += Time.deltaTime;
            }
        }

        if (auraDecayClock > 0)
        {
            //(float fade, float distortion, float glow) = GetAuraBurnoutLerp(auraDecayClock, putOutTime);
            auraOverlayImage.material.SetFloat("_FadeAmount", GetAuraOverlayFadeoutLerp(auraDecayClock, putOutTime));
            //auraOverlayImage.material.SetFloat("_FadeAmount", fade); // Fade
        }

        if (auraDecayClock >= putOutTime)
        {
            ResetBurnAura();
            SetAura(AuraType.normal);
            return;
        }
            


        if (!group.isFalling)
        {
            if (tileToBurn == null)
            {
                tileToBurn = gm.GetGameTile(coordX, coordY - 1);
                if (tileToBurn != null)
                {
                    auraClock = 0;

                    if (!tileToBurn.CanBeBurned())
                        tileToBurn = null;
                    else
                    {
                        PlaySoundSteamHiss();

                        decaySoundSource.clip = fireSizzleSound;
                        decaySoundSource.Play();
                        decaySoundSource.DOFade(0.25f * PlayerPrefs.GetFloat("SoundVolume", 0.5f), 0.5f);
                    }
                        
                }
                else
                {
                    auraClock += Time.deltaTime;
                    if (auraClock >= burnTime / 3)
                    {
                        auraClock = 0;
                        AudioSource.PlayClipAtPoint(burningBurnOutFlame[Random.Range(0, burningBurnOutFlame.Length)], new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                        BurnFall();
                    }
                }
            }
            else
            {
                if (tileToBurn == gm.GetGameTile(coordX, coordY - 1))
                {                    
                    if (auraClock >= burnTime)
                    {
                        auraClock = 0;
                        BurnFall();
                    }
                    auraClock += Time.deltaTime;

                    (float fade, float distortion, float glow) = GetAuraBurnoutLerp(auraClock, burnTime);

                    tileToBurn.unrevealedButtonImage.material.SetFloat("_FadeAmount", fade); // Fade
                    tileToBurn.tileBackground.material.SetFloat("_FadeAmount", fade); // Fade

                    tileToBurn.unrevealedButtonImage.material.SetFloat("_DistortionAmount", distortion); // distortion
                    tileToBurn.tileBackground.material.SetFloat("_DistortionAmount", distortion); // distortion

                    tileToBurn.unrevealedButtonImage.material.SetFloat("_Glow", glow); // glow
                    tileToBurn.tileBackground.material.SetFloat("_Glow", glow); // glow


                    if (auraClock > 14.5 && !tileToBurn.burnoutInvisible)
                    {
                        AudioSource.PlayClipAtPoint(burningBurnOutFlame[Random.Range(0, burningBurnOutFlame.Length)], new Vector3(0, 0, 0), 0.8f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                        tileToBurn.burnoutInvisible = true;
                    }
                }
                else 
                {
                    ResetBurnAura();
                }
            }
        }        
    }
    private void UpdateAuraFrozen()
    {
        foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
        {
            if (t.aura == AuraType.burning)
            {
                auraDecayClock += Time.deltaTime;
            }
        }

        /*if (newAuras.Where(s => s == AuraType.burning).Count() > adjacentAuras.Where(s => s == AuraType.burning).Count())
        {
            PlaySoundSteamHiss();
        }*/

        if (!adjacentAuras.Contains(AuraType.burning) && auraDecayClock > 0)
            auraDecayClock -= Time.deltaTime;

        if (adjacentAuras.Contains(AuraType.burning))
        {
            if (decaySoundSource.volume == 0)
            {
                decaySoundSource.clip = meltSound;
                decaySoundSource.Play();
                decaySoundSource.DOFade(0.25f * PlayerPrefs.GetFloat("SoundVolume", 0.5f), 0.5f);
            }
        }
        else
        {
            if (decaySoundSource.isPlaying)
            {
                decaySoundSource.Stop();
                decaySoundSource.volume = 0;
            }                

            if (auraDecayClock > 0)
                auraDecayClock -= Time.deltaTime;
        }

        if (auraDecayClock > 0)
        {
            //(float fade, float distortion, float glow) = GetAuraBurnoutLerp(auraClock, meltTime);
            auraOverlayImage.material.SetFloat("_FadeAmount", auraDecayClock / meltTime); // Fade            
        }
        else if (auraOverlayImage.material.GetFloat("_FadeAmount") > 0)
        {
            auraOverlayImage.material.SetFloat("_FadeAmount", 0); // Reset Fade            
        }
            


        if (auraDecayClock >= meltTime)
        {
            PlaySoundSteamHiss();
            SetAura(AuraType.wet);
        }            
    }
    float GetAuraOverlayFadeoutLerp(float timer, float maxTime)
    {
        float normalizedTime = timer / maxTime;
        float normalizedOutput = Mathf.Lerp(0.25f, 0.65f, normalizedTime);
        return normalizedOutput;
    }
    (float fade, float distortion, float glow) GetAuraBurnoutLerp(float timer, float maxTime)
    {
        float timeStage1 = 1 * (maxTime / 15);
        float timeStage2 = 5 * (maxTime / 15);
        float timeStage3 = 14.5f * (maxTime / 15);

        if (timer <= timeStage1) 
        {
            float t = timer / timeStage1;

            return (Mathf.Lerp(0, 0.2f, t), // Fade
                Mathf.Lerp(0, 0.0075f, t), // Distortion
                Mathf.Lerp(0, 0.03f, t)); // Glow
        }
        else if (timer <= timeStage2)
        {
            float t = (timer - timeStage1) / (timeStage2 - timeStage1);

            return (Mathf.Lerp(0.2f, 0.25f, t), // Fade
                Mathf.Lerp(0.0075f, 0.1f, t), // Distortion
                Mathf.Lerp(0.03f, 0.5f, t)); // Glow
        }
        else if (timer <= timeStage3)
        {
            float t = (timer - timeStage2) / (timeStage3 - timeStage2);

            return (Mathf.Lerp(0.25f, 0.5f, t), // Fade
                Mathf.Lerp(0.1f, 0.25f, t), // Distortion
                Mathf.Lerp(0.5f, 5, t)); // Glow
        }
        else if (timer <= maxTime)
        {
            float t = (timer - timeStage3) / (maxTime - timeStage3);

            return (Mathf.Lerp(0.5f, 1, t), // Fade
                0.25f, // Distortion
                0.5f); // Glow            
        }

        return (0, 0, 0);
    }

    public bool CanBeBurned()
    {
        if (group == null) 
            return false;
        if (aura == AuraType.burning || aura == AuraType.frozen || aura == AuraType.wet)
            return false;
        if (group.isFalling || group.isDisplay || group.isHeld)
            return false;

        return true;
    }

    public void ResetBurnAura()
    {
        if (tileToBurn == null) 
            return;

        decaySoundSource.Stop();
        decaySoundSource.volume = 0;

        tileToBurn.unrevealedButtonImage.material.SetFloat("_FadeAmount", 0); // Fade
        tileToBurn.tileBackground.material.SetFloat("_FadeAmount", 0); // Fade

        tileToBurn.unrevealedButtonImage.material.SetFloat("_DistortionAmount", 0); // distortion
        tileToBurn.tileBackground.material.SetFloat("_DistortionAmount", 0); // distortion

        tileToBurn.unrevealedButtonImage.material.SetFloat("_Glow", 0); // glow
        tileToBurn.tileBackground.material.SetFloat("_Glow", 0); // glow

        tileToBurn.burnoutInvisible = false;
        tileToBurn = null;
    }

    public void BurnFall()
    {
        if (aura != AuraType.burning || group.isFalling)
            return;

        // Burn baby burn
        if (tileToBurn != null)
        {
            if (tileToBurn == gm.GetGameTile(coordX, coordY - 1) && tileToBurn.CanBeBurned())
            {
                // Lose x25% multiplier (x0.25 out of x50) when a tile is burnt
                gm.SetScoreMultiplier(-25, 0);

                tileToBurn.isDestroyed = true;
                Destroy(tileToBurn.gameObject);
                GameManager.gameBoard[coordX][coordY - 1] = null;
            }
        }

        // Fall into the burnt tile's space, and bring any burning tiles above you along the way
        Tile tileAbove = gm.GetGameTile(coordX, coordY + 1);

        // Move one towards bottom
        GameManager.gameBoard[coordX][coordY - 1] = GameManager.gameBoard[coordX][coordY];
        GameManager.gameBoard[coordX][coordY] = null;

        // Update Block position
        coordY -= 1;

        decaySoundSource.Stop();
        decaySoundSource.volume = 0;

        if (tileAbove != null)
            tileAbove.BurnFall();

    }

    public void PlaySoundSteamHiss()
    {
        AudioSource.PlayClipAtPoint(burningPutOutSteamHiss[Random.Range(0, burningPutOutSteamHiss.Length)], new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));
    }

    public void PlaySoundFrozenHit()
    {        
        AudioSource.PlayClipAtPoint(hardHitSounds[Random.Range(0, hardHitSounds.Length)], new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));
        PlaySoundSnow();
    }

    public void PlaySoundSnow()
    {
        AudioSource.PlayClipAtPoint(snowSounds[Random.Range(0, snowSounds.Length)], new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));
    }
    #endregion
}
