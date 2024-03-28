using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.U2D;
using TMPro.Examples;

public class Tile : MonoBehaviour
{
    public int coordX = 0;
    public int coordY = 0;

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
    public Color solvedMarkColor;

    public AudioClip revealSound;
    public AudioClip flagSound;
    public AudioClip unflagSound;
    public AudioClip chordSound;
    public AudioClip chordFlagSound;
    public AudioClip chordFailSound;

    public SpriteRenderer tileBackground;
    public SpriteRenderer explodedMineBackground;
    public SpriteShapeRenderer shimmerOverlay;
    
    TextMeshProUGUI text;    
    
    GameManager gm;
    Camera cam;
    HoldTetromino holdTetromino;
    GameModifiers gameMods;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        holdTetromino = GameObject.FindGameObjectWithTag("Hold").GetComponent<HoldTetromino>();
        gameMods = GameObject.FindGameObjectWithTag("ScoreKeeper").GetComponent<GameModifiers>();

        Vector2 v = GameManager.roundVec2(transform.position);
        coordX = (int)v.x;
        coordY = (int)v.y;
        
        CountMine();
        
        if (isDisplay)
        {
            GetComponentInChildren<Button>().interactable = false;
            GetComponent<ButtonJiggle>().scaleMultiplierEnlarge = ((GetComponent<ButtonJiggle>().scaleMultiplierEnlarge - 1) * 0.25f) + 1;
            //Debug.Log ("Display " + gameObject.name);
            //tileBackground.color = new Color(215, 215, 215, 255);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.cheatAutoFlagMode)
        {
            if (isMine)
                isFlagged = true;
            else
                Reveal();
        }
        this.transform.position = new Vector3(coordX, coordY, this.transform.position.z);
        this.name = "Tile (" + coordX + ", " + coordY + ")";
        
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
                    gm.AddScore(8888);
                    gm.SetScoreMultiplier(8, 1f, true);
                    if (gm.safeEdgeTilesGained < 8)
                        gm.AddSafeTileToEdges();
                    is8Triggered = true;
                }
            }            
        }
        TextMeshProUGUI supportText = text;
        if (GetComponentInChildren<SetRandomSupporterName>() != null)
            supportText = GetComponentInChildren<SetRandomSupporterName>().supportText;
        
        if (isRowSolved)
        {
            tileBackground.color = solvedMarkColor;
            shimmerOverlay.gameObject.SetActive(true);
            if (true || !isDisplay)
            {
                text.GetComponent<VertexJitter>().enabled = true;
                supportText.GetComponent<VertexJitter>().enabled = true;      
            }
            
                  

            /*if (GetComponent<IdleJiggle>() != null)
            {
                GetComponent<IdleJiggle>().ShakeRotation(0.15f, 0.5f);
                GetComponent<ButtonJiggle>().Enlarge(true);
                //GetComponent<IdleJiggle>().ShakeScale(0.15f, 0.05f);
            } */           
        }            
        else
        {
            tileBackground.color = Color.white;
            if (!isDisplay)
                shimmerOverlay.gameObject.SetActive(false);
            
            text.GetComponent<VertexJitter>().enabled = false;
            text.SetText(text.text);
            supportText.GetComponent<VertexJitter>().enabled = false;
            supportText.SetText(supportText.text);
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
                myText = "*";
            else if (gameMods.showCredits && !gm.isTitleMenu) // During Credits, hide numbers
                myText = "";
                
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

        if (gm.isPaused && !gm.marathonOverMenu.isActive)
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
        if (gm.isGameOver || isRevealed || GetComponentInParent<Group>().isHeld)
            return;

        isFlagged = !isFlagged;
        if (isFlagged)
        {
            //isQuestioned = false;

            //GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            gm.soundManager.PlayTileRevealSound();
            //AudioSource.PlayClipAtPoint(flagSound, new Vector3(0, 0, 0), 0.5f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
            //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);
            gm.TriggerOnTileSolveOrLandEvent();

            float sm = gm.GetScoreMultiplier();
            gm.SetScoreMultiplier(1, 1f, true);
            //Debug.Log(gameObject.name + ": " + sm + " + 0.01 = " + gm.GetScoreMultiplier());

            gm.currentFlags += 1;

            if (isMine)
            {
                if (!isFailedToChord)
                {
                    holdTetromino.AddToManualSolveStreak(true);
                }
            }                
            else
            {
                holdTetromino.ResetManualSolveStreak();
            }

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
            holdTetromino.ResetManualSolveStreak();

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

    public void Reveal(bool isAutomatic = false, bool isManual = false)
    {
        if (!isRevealed && !isFlagged && !isDisplay && (!GetComponentInParent<Group>().isHeld || isAutomatic))
        {
            isRevealed = true;
            //isQuestioned = false;

            revealedThisFrame = true;
            //gm.RevealTile(coordX, coordY, nearbyMines, isMine);

            UpdateText();

            if (isMine)
            {
                if (!gm.isGameOver)
                    explodedMineBackground.enabled = true;
                gm.EndGame();
            }
            else if (gm == null) // Error catching, can sometimes happen when the scene loads
            {
                Debug.LogWarning("Game Manager can't be found, I'll assume it's a new game");
            }
            else if (!gm.isGameOver)
            {
                //GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                gm.soundManager.PlayTileRevealSound();
                //AudioSource.PlayClipAtPoint(revealSound, new Vector3(0, 0, 0), 0.75f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));

                if (isManual && !isFailedToChord)
                {
                    holdTetromino.AddToManualSolveStreak(false);
                }
                    

                // Scoring
                if (!GetComponentInParent<Group>().isDisplay && !GetComponentInParent<Group>().isFalling && !GetComponentInParent<Group>().isHeld)
                {
                    // If the mino is falling, don't give a huge bonus for being revealed in the air
                    if (GetComponentInParent<Group>().isFalling)
                    {
                        gm.AddScore(nearbyMines, false); // * nearbyMines
                        /*holdTetromino.scoreMissingTest += Mathf.FloorToInt(((nearbyMines * nearbyMines) - nearbyMines) * gm.level * (1 + gm.GetScoreMultiplier()));
                        holdTetromino.scoreRevealRemainingTest += Mathf.FloorToInt(nearbyMines * gm.level * (1 + gm.GetScoreMultiplier()));                        
                        holdTetromino.scoreRevealTest += Mathf.FloorToInt(((nearbyMines * nearbyMines)) * gm.level * (1 + gm.GetScoreMultiplier()));
                        holdTetromino.scoreRevealNoLevelTest += Mathf.FloorToInt(nearbyMines * (1 + gm.GetScoreMultiplier()));*/
                    }                        
                    else
                    {
                        gm.AddScore(nearbyMines * (coordY + 1), false); // * nearbyMines
                        /*holdTetromino.scoreMissingTest += Mathf.FloorToInt(((nearbyMines * nearbyMines * (coordY + 1)) - (nearbyMines * (coordY + 1))) * gm.level * (1 + gm.GetScoreMultiplier()));
                        holdTetromino.scoreRevealRemainingTest += Mathf.FloorToInt((nearbyMines * (coordY + 1)) * gm.level * (1 + gm.GetScoreMultiplier()));
                        holdTetromino.scoreRevealTest += Mathf.FloorToInt(((nearbyMines * nearbyMines * (coordY + 1))) * gm.level * (1 + gm.GetScoreMultiplier()));
                        holdTetromino.scoreRevealNoLevelTest += Mathf.FloorToInt((nearbyMines * (coordY + 1)) * (1 + gm.GetScoreMultiplier()));*/
                    }
                        
                    
                    gm.SetScoreMultiplier(1, 1f, true);
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
            explodedMineBackground.enabled = true;

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

                foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
                {
                    if (!t.isFlagged)
                    {
                        if (!t.isDisplay)    
                        {
                            t.Reveal(false, true);      
                        }
                    }                                        
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
                    holdTetromino.ResetManualSolveStreak();
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

                foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
                {
                    if (!t.isFlagged)
                    {
                        t.FlagToggle();
                    }                                        
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
                        realFail = true;                        
                    }                                        
                }
                if (realFail)
                {                    
                    holdTetromino.ResetManualSolveStreak();
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
}
