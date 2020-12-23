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
    
    float fallClock = 1;

    TextMeshProUGUI text;

    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        Vector2 v = GameManager.roundVec2(transform.position);
        coordX = (int)v.x;
        coordY = (int)v.y;

        if (isDisplay)
            GetComponentInChildren<Button>().interactable = false;
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

    void UpdateText()
    {
        string myText = "";
        if (isRevealed)
        {
            myText = nearbyMines.ToString();
            if (nearbyMines == 0)
                myText = "";
            if (isMine)
                myText = "💣";
        }
        else
        {
            if (isFlagged)
                myText = "!";
            else if (isQuestioned)
                myText = "?";
        }

        if (text != null)
            text.SetText(myText);
    }

    public void FlagToggle()
    {
        isFlagged = !isFlagged;
        if (isFlagged)
        {
            isQuestioned = false;
            GameManager.deleteFullRows();            
        }
    }

    public void QuestionToggle()
    {
        isQuestioned = !isQuestioned;
        if (isQuestioned)
            isFlagged = false;
    }

    public void Reveal()
    {
        if (!isRevealed && !isFlagged && !isDisplay)
        {
            isRevealed = true;
            //gm.RevealTile(coordX, coordY, nearbyMines, isMine);            

            if (isMine)
                gm.EndGame();

            ZeroCascade();

            GetComponentInChildren<Button>().interactable = false;

            GameManager.deleteFullRows();
        }
    }

    void ZeroCascade()
    {
        if (nearbyMines == 0 && !isMine && isRevealed)
        {
            foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
            {
                t.Reveal();
            }
        }
    }

    void DetectProximity()
    {
        int nearbyMinesTemp = 0;
        foreach (Tile t in gm.GetNeighborTiles(coordX, coordY))
        {
            if (t.isMine)
                nearbyMinesTemp += 1;
        }
        nearbyMines = nearbyMinesTemp;
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
