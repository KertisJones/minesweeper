using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.U2D;

public class Tile : MonoBehaviour
{
    public int coordX = 0;
    public int coordY = 0;

    public bool isMine = false;
    public bool isFlagged = false;
    public bool isQuestioned = false;
    public bool isRevealed = false;
    public bool isDisplay = false;
    public int nearbyMines = 0;
    public int nearbyFlags = 0;

    public float screenShakeDuration = 0.1f;
    public float screenShakeStrength = 0.1f;
    public bool isDestroyed = false;
    public bool isRowSolved = false;
    public bool is8Triggered = false;
    public Color solvedMarkColor;

    public AudioClip revealSound;
    public AudioClip flagSound;
    public AudioClip unflagSound;

    public SpriteRenderer tileBackground;
    public SpriteRenderer explodedMineBackground;
    public SpriteShapeRenderer shimmerOverlay;
    
    TextMeshProUGUI text;    
    
    GameManager gm;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        Vector2 v = GameManager.roundVec2(transform.position);
        coordX = (int)v.x;
        coordY = (int)v.y;
        
        CountMine();
        
        if (isDisplay)
        {
            GetComponentInChildren<Button>().interactable = false;
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
        this.transform.position = new Vector3(coordX, coordY, 0);
        this.name = "Tile (" + coordX + ", " + coordY + ")";
        
        UpdateText();
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
        
        if (isRowSolved)
        {
            tileBackground.color = solvedMarkColor;
            shimmerOverlay.gameObject.SetActive(true);
        }            
        else
        {
            tileBackground.color = Color.white;
            if (!isDisplay)
                shimmerOverlay.gameObject.SetActive(false);
        }
            

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
                myColor = new Color32(0, 0, 0, 255);
                break;
        }
        if (isQuestioned || isMine)
            myColor = Color.black;

        if (isRevealed)
        {
            myText = nearbyMines.ToString();

            if (nearbyMines == 0)
                myText = "";
            if (isMine)
                myText = "*";
                
        }
        else
        {
            if (isFlagged)
                myText = "<sprite=0>";
            else if (isQuestioned)
                myText = "?";
        }

        if (gm.isPaused && !gm.marathonOverMenu.isActive)
            myText = "";

        if (text != null)
        {
            text.SetText(myText);
            text.color = myColor;
        }        
    }

    public void FlagToggle()
    {
        if (gm.isGameOver || isRevealed || GetComponentInParent<Group>().isHeld)
            return;

        isFlagged = !isFlagged;
        if (isFlagged)
        {
            isQuestioned = false;

            GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(flagSound, new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);

            float sm = gm.GetScoreMultiplier();
            gm.SetScoreMultiplier(1, 1f, true);
            //Debug.Log(gameObject.name + ": " + sm + " + 0.01 = " + gm.GetScoreMultiplier());

            gm.currentFlags += 1;

            //GameManager.deleteFullRows();            
            GameManager.CheckForPossiblePerfectClear();
        }
        else
        {
            GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(unflagSound, new Vector3(0, 0, 0), 0.5f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));

            gm.ResetScoreMultiplier();

            gm.currentFlags -= 1;
        }
        GameManager.markSolvedRows();
    }

    public void QuestionToggle()
    {
        if (gm.isGameOver || isRevealed)
            return;

        isQuestioned = !isQuestioned;
        if (isQuestioned)
        {
            isFlagged = false;

            GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(flagSound, new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));
        }
        else
        {
            GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(unflagSound, new Vector3(0, 0, 0), 0.75f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));
        }
    }

    public void Reveal(bool isAutomatic = false)
    {
        if (!isRevealed && !isFlagged && !isDisplay && (!GetComponentInParent<Group>().isHeld || isAutomatic))
        {
            isRevealed = true;
            isQuestioned = false;
            //gm.RevealTile(coordX, coordY, nearbyMines, isMine);

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
                GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                AudioSource.PlayClipAtPoint(revealSound, new Vector3(0, 0, 0), 0.75f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));

                // Scoring
                if (!GetComponentInParent<Group>().isDisplay && !GetComponentInParent<Group>().isFalling && !GetComponentInParent<Group>().isHeld)
                {
                    // Each revealed tile is equal to 1 point.
                    gm.AddScore(nearbyMines * nearbyMines * (coordY + 1));
                    gm.SetScoreMultiplier(1, 1f, true);
                }
            }

            //DetectProximity();
            ZeroCascade();

            GetComponentInChildren<Button>().interactable = false;

            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);

            //GameManager.deleteFullRows();
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

        //Debug.Log("Attempting Chord...");
        if (isRevealed && nearbyFlags == nearbyMines && !isMine)
        {
            //Debug.Log("Chording!");
            foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
            {
                if (!t.isFlagged)
                    t.Reveal();
            }
        }
        //else
            //Debug.Log("Chord Failed.");
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

    /*void Fall()
    {
        if (coordY > 0)
        {
            Debug.Log("fall");
            gm.MoveTile(this.gameObject, coordX, coordY - 1);
        }
    }*/
}
