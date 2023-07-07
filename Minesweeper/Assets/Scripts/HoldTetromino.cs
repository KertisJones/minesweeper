using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldTetromino : MonoBehaviour
{
    GameManager gm;
    private InputManager inputManager;
    public TetrominoSpawner tetrominoSpawner;
    public Transform targetPosition;
    public GameObject heldTetromino;
    public GameObject heldTetrominoPrevious;
    public GameObject swapPartner;
    public GameObject cleanseButton;
    public TMPro.TMP_Text cleansePointText;
    public TMPro.TMP_Text cleansePointRechargeText;    
    int manualTileSolveStreak = 0;    
    public int manualTileSolvePerfectStreak = 0;    
    public int manualTileSolvePerfectStreakIncludedWallsTEST = 0; 
    public int manualTileSolvePerfectStreakIncludedWallsHighestScoreTEST = 0; 
    public int scoreMissingTest = 0;
    int cleanseScoreHighest = 0;
    int manualTileSolveStreakHighest = 0;
    int manualTileSolvePerfectStreakHighest = 0;
    public int manualTileSolvePerfectStreakIncludedWallsHighestTEST = 0; 
    public int manualTileSolvePerfectStreakPoints = 0;
    public int manualTileSolvePerfectSteakTotal = 0;
    void Awake()
    {
        inputManager = InputManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.isPaused || gm.isGameOver)
            return;
        if (CleanseIsPossible())
        {
            cleanseButton.SetActive(true);
            string exclamationSuffix = "";
            for (int i = 0; i < Mathf.Min(5, (Mathf.FloorToInt(manualTileSolvePerfectStreak / 100f))); i++)
            {
                exclamationSuffix += "!";
            }
            cleanseScoreHighest = Math.Max(cleanseScoreHighest, Mathf.FloorToInt(manualTileSolveStreak * 5 * gm.level * (1 + gm.GetScoreMultiplier()) * GetCleanseStreakMultiplier()));
            //manualTileSolvePerfectStreakIncludedWallsHighestScoreTEST = Math.Max(cleanseScoreHighest, Mathf.FloorToInt(manualTileSolvePerfectStreakIncludedWallsTEST * 5 * gm.level * (1 + gm.GetScoreMultiplier()) * GetCleanseStreakMultiplier()));
            cleansePointText.text = (manualTileSolveStreak * 5 * gm.level * (1 + gm.GetScoreMultiplier()) * GetCleanseStreakMultiplier()).ToString("#,#") + " Points" + exclamationSuffix;
            if (GetCleanseStreakMultiplier() > 5)
            {
                cleansePointText.GetComponent<TMPro.Examples.VertexJitter>().enabled = true;
                //cleansePointText.GetComponent<VertexColorCyclerGradient>().enabled = true;
            }
            else
            {
                cleansePointText.GetComponent<TMPro.Examples.VertexJitter>().enabled = false;
                //cleansePointText.GetComponent<VertexColorCyclerGradient>().enabled = false;
            }
        }                    
        else
            cleanseButton.SetActive(false);  
        if (cleansePointRechargeText != null)
            cleansePointRechargeText.text = manualTileSolveStreak + ", " + manualTileSolvePerfectStreak + ", " + manualTileSolvePerfectStreakIncludedWallsTEST + " (" + manualTileSolveStreakHighest + ", " + manualTileSolvePerfectStreakHighest + ", " + manualTileSolvePerfectStreakIncludedWallsHighestTEST + ")\n+" 
            + cleanseScoreHighest + " - " + manualTileSolvePerfectStreakPoints + ", " + manualTileSolvePerfectStreakIncludedWallsHighestScoreTEST + "(" + manualTileSolvePerfectSteakTotal +")\n-" 
            + scoreMissingTest;          
        /*if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.RightShift))
        {
            Hold();
        }*/
    }

    #region Input
    void OnEnable()
    {
        inputManager.holdPress.started += _ => PressHold();
        inputManager.cleansePress.started += _ => Cleanse();
    }
    void OnDisable()
    {
        inputManager.holdPress.started -= _ => PressHold();
        inputManager.cleansePress.started -= _ => Cleanse();
    }
    void PressHold()
    {
        Hold();
    }
    #endregion

    void Hold()
    {
        if (gm.isGameOver || gm.isPaused)
            return;
        
        GameObject currentTetromino = tetrominoSpawner.currentTetromino;
        if (currentTetromino == heldTetromino || currentTetromino == heldTetrominoPrevious)
            return;
        if (swapPartner != null)
            if (swapPartner.GetComponent<Group>().isFalling)
                return;
        
        RemoveFromBoard(currentTetromino);
        AddToBoard(heldTetromino);

        gm.holds += 1;

        heldTetrominoPrevious = heldTetromino;
        heldTetromino = currentTetromino;

        // Input DAS for next tetromino
        if (heldTetromino.GetComponent<Group>().buttonLeftHeld)
            gm.GetActiveTetromino().PressLeft();
        if (heldTetromino.GetComponent<Group>().buttonRightHeld)
            gm.GetActiveTetromino().PressRight();
        heldTetromino.GetComponent<Group>().currentRotation = 0;
    }

    void RemoveFromBoard(GameObject tetromino)
    {
        if (tetromino == null)
            return;
        //Debug.Log("HOLD! RemoveFromBoard");
        tetromino.transform.position = targetPosition.position;
        tetromino.transform.rotation = new Quaternion(0, 0, 0, 0);
        tetromino.GetComponent<Group>().isHeld = true;        
        tetromino.GetComponent<Group>().UpdateGrid();

        if (heldTetromino == null)
        {
            swapPartner = tetrominoSpawner.nextTetromino;
            tetrominoSpawner.spawnNext();
        }            
        else
            tetrominoSpawner.currentTetromino = null;
    }

    void AddToBoard(GameObject tetromino)
    {
        if (tetromino == null)
            return;
        //Debug.Log("HOLD! AddToBoard");
        tetromino.transform.position = tetrominoSpawner.transform.position;
        tetromino.GetComponent<Group>().isHeld = false;
        tetromino.GetComponent<Group>().UpdateGrid();

        tetrominoSpawner.currentTetromino = tetromino;
    }

    bool CleanseIsPossible()
    {
        // Don't allow Cleanse if player doesn't have enough edge tiles to spend
        //if (gm.safeEdgeTilesGained < 4)
            //return false;
        if (manualTileSolveStreak < 50)
            return false;

        if (heldTetromino == null)
            return false;
        
        // Don't allow Cleanse if the held mino doesn't have any unrevealed tiles
        foreach (Tile tile in heldTetromino.GetComponent<Group>().GetChildTiles())
        {
            if (!tile.isRevealed)
            {
                return true;
            }
        }

        return false;
    }

    public void Cleanse()
    {
        if (gm.isPaused || gm.isGameOver)
            return;
        if (!CleanseIsPossible())
            return;
        
        
        
        //gm.RemoveSafeTileFromEdges();
        //gm.RemoveSafeTileFromEdges();
        //gm.RemoveSafeTileFromEdges();
        //gm.RemoveSafeTileFromEdges();

        foreach (Tile tile in heldTetromino.GetComponent<Group>().GetChildTiles())
        {
            if (tile.isMine)
            {
                tile.isMine = false;
                gm.currentMines -= 1;
            }
            if (tile.isFlagged)
            {
                tile.isFlagged = false;
                gm.currentFlags -= 1;
            }
            tile.Reveal(true);
        }
        
        gm.AddScore(manualTileSolveStreak * 5 * GetCleanseStreakMultiplier());
        manualTileSolveStreak = 0;

        Tooltip.HideTooltip_Static();
    }

    public void AddToManualSolveStreak(bool isPerfect, bool isEasyTile = false)
    {
        manualTileSolveStreak++; 

        if (isPerfect)
        {
            manualTileSolvePerfectStreak++;
            manualTileSolvePerfectStreakIncludedWallsTEST++;
            if (manualTileSolvePerfectStreak >= 100)
                manualTileSolvePerfectSteakTotal++;
            manualTileSolvePerfectStreakPoints += Mathf.FloorToInt(50 * Mathf.FloorToInt(manualTileSolvePerfectStreak / 100) * gm.level * (1 + gm.GetScoreMultiplier()));;
            manualTileSolvePerfectStreakIncludedWallsHighestScoreTEST += Mathf.FloorToInt(50 * Mathf.FloorToInt(manualTileSolvePerfectStreakIncludedWallsTEST / 100) * gm.level * (1 + gm.GetScoreMultiplier()));
        }            
        else if (isEasyTile)
        {
            manualTileSolvePerfectStreakIncludedWallsTEST++;
            manualTileSolvePerfectStreakIncludedWallsHighestScoreTEST += Mathf.FloorToInt(50 * Mathf.FloorToInt(manualTileSolvePerfectStreakIncludedWallsTEST / 100) * gm.level * (1 + gm.GetScoreMultiplier()));;
        }
        else
        {
            manualTileSolvePerfectStreak = 0;
            manualTileSolvePerfectStreakIncludedWallsTEST = 0;
        }
        
        manualTileSolveStreakHighest = Mathf.Max(manualTileSolveStreak, manualTileSolveStreakHighest);
        manualTileSolvePerfectStreakHighest = Mathf.Max(manualTileSolvePerfectStreak, manualTileSolvePerfectStreakHighest);

        manualTileSolvePerfectStreakIncludedWallsHighestTEST = Mathf.Max(manualTileSolvePerfectStreakIncludedWallsTEST, manualTileSolvePerfectStreakIncludedWallsHighestTEST);
    }

    public void ResetManualSolveStreak()
    {
        manualTileSolveStreak = 0;
        ResetManualSolvePerfectStreak();
    }

    public void ResetManualSolvePerfectStreak()
    {
        manualTileSolvePerfectStreak = 0;
        manualTileSolvePerfectStreakIncludedWallsTEST = 0;
    }

    int GetCleanseStreakMultiplier()
    {
        return Mathf.Min(5, Mathf.Max(1, Mathf.FloorToInt(manualTileSolvePerfectStreak / 100f)));
    }
}