using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public AudioClip revealSound;
    public AudioClip flagSound;
    public AudioClip unflagSound;

    public SpriteRenderer tileBackground;
    public SpriteRenderer explodedMineBackground;
    
    TextMeshProUGUI text;    

    GameManager gm;
    Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

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
        this.transform.position = new Vector3(coordX, coordY, 0);
        this.name = "Tile (" + coordX + ", " + coordY + ")";
        
        UpdateText();
        if (!isMine)
            DetectProximity();

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
        if (isQuestioned)
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
            AudioSource.PlayClipAtPoint(flagSound, new Vector3(0, 0, 0));
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);

            gm.currentFlags += 1;

            GameManager.deleteFullRows();            
        }
        else
        {
            GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(unflagSound, new Vector3(0, 0, 0), 0.5f);

            gm.currentFlags -= 1;
        }
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
            AudioSource.PlayClipAtPoint(flagSound, new Vector3(0, 0, 0));
        }
        else
        {
            GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(unflagSound, new Vector3(0, 0, 0), 0.75f);
        }
    }

    public void Reveal()
    {
        if (!isRevealed && !isFlagged && !isDisplay && !GetComponentInParent<Group>().isHeld)
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
                AudioSource.PlayClipAtPoint(revealSound, new Vector3(0, 0, 0), 0.75f);

                // Scoring
                if (!GetComponentInParent<Group>().isDisplay)
                {
                    // Each revealed tile is equal to 1 point.
                    gm.AddScore(5);
                }
            }

            ZeroCascade();

            GetComponentInChildren<Button>().interactable = false;

            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);

            GameManager.deleteFullRows();
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
        Debug.Log("Attempting Chord...");
        if (isRevealed && nearbyFlags == nearbyMines && !isMine)
        {
            Debug.Log("Chording!");
            foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
            {
                if (!t.isFlagged)
                    t.Reveal();
            }
        }
        else
            Debug.Log("Chord Failed.");
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
        if (nearbyMines == 0)
            ZeroCascade();
        
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
