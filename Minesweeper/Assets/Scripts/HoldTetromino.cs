using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    public ProgressBar cleanseProgressBar;
    //public TMPro.TMP_Text cleansePointText;
    public AudioClip cleanseReadySound;
    public AudioClip cleanseActivateSound;
    private bool cleanseReady = false;
    public int cleanseRecharge = 0;
    int manualTileSolveStreak = 0;
    public AudioClip holdSwitchSound;
    public AudioClip holdFailedSound;
    
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
            if (!cleanseReady) // Do when cleanse is ready, not every frame
            {
                //GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                AudioSource.PlayClipAtPoint(cleanseReadySound, new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f)); 
                if (cleanseButton != null)               
                {
                    cleanseButton.SetActive(true);
                    cleanseButton.GetComponent<ButtonJiggle>().Reset();
                    cleanseProgressBar.GetComponent<IdleJiggle>().jumpInPlaceHeight = 0;
                }                
                cleanseReady = true;
            }
        }
        else
        {
            if (cleanseReady)
            {
                cleanseButton.GetComponent<ButtonJiggle>().ShrinkToZero();
                cleanseProgressBar.GetComponent<IdleJiggle>().jumpInPlaceHeight = cleanseButton.GetComponent<IdleJiggle>().jumpInPlaceHeight;
                cleanseReady = false;
            }
        }

        /*if (cleansePointRechargeText != null)
            cleansePointRechargeText.text = manualTileSolveStreak + ", " + manualTileSolvePerfectStreak + ", " + manualTileSolvePerfectStreakIncludedWallsTEST + " (m" + manualTileSolveStreakHighest + ", p" + manualTileSolvePerfectStreakHighest + ", w" + manualTileSolvePerfectStreakIncludedWallsHighestTEST + ")\n+c" 
            + cleanseScoreHighest + " ~ " + manualTileSolvePerfectStreakPoints + ", w" + manualTileSolvePerfectStreakIncludedWallsPointsTEST + " *L(" + manualTileSolvePerfectStreakPointsWithLevelTEST + ", w" + manualTileSolvePerfectStreakIncludedWallsPointsWithLevelTEST + ")\n-" 
            + scoreMissingTest + " + " + scoreRevealRemainingTest + " = " + scoreRevealTest + " (-" + scoreRevealNoLevelTest + "=" + (scoreRevealTest - scoreRevealNoLevelTest) + ")";*/
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
        {
            //gm.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(holdFailedSound, new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));
            return;
        }
        if (swapPartner != null)
        {
            if (swapPartner.GetComponent<Group>().isFalling)
            {
                //gm.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                AudioSource.PlayClipAtPoint(holdFailedSound, new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                return;
            }
        }
        
        RemoveFromBoard(currentTetromino);
        AddToBoard(heldTetromino);

        gm.holds += 1;

        heldTetrominoPrevious = heldTetromino;
        heldTetromino = currentTetromino;

        //gm.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
        AudioSource.PlayClipAtPoint(holdSwitchSound, new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));

        if (cleanseProgressBar != null)
            cleanseProgressBar.ChangeColor(heldTetromino.GetComponentInChildren<Button>().image.color);

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
        if (cleanseRecharge < 50)
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
        
        gm.AddScore(250);
        cleanseRecharge = 0;
        if (cleanseProgressBar != null)
        {
            cleanseProgressBar.current = 0;
            cleanseProgressBar.currentTween = 0;
        }        

        //GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
        AudioSource.PlayClipAtPoint(cleanseActivateSound, new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));
        cleanseReady = false;

        if (cleanseButton != null)
            cleanseButton.GetComponent<ButtonJiggle>().ShrinkToZero();

        Tooltip.HideTooltip_Static();
    }

    public void AddToManualSolveStreak(bool isFlag)//bool isPerfect, bool isEasyTile = false)
    {
        if (manualTileSolveStreak < 100)
            manualTileSolveStreak++; 
        gm.AddScore(manualTileSolveStreak, false);

        if (!isFlag)
        {
            if (cleanseRecharge < 50)
                cleanseRecharge++;
            if (cleanseProgressBar != null)
                cleanseProgressBar.current = cleanseRecharge;
        }
    }
        /*if (isPerfect) // Not on edge or bottom row
        {
            manualTileSolvePerfectStreak++;
            manualTileSolvePerfectStreakIncludedWallsTEST++;

            gm.AddScore(manualTileSolvePerfectStreak, false);

            manualTileSolvePerfectStreakPoints += Mathf.FloorToInt(manualTileSolvePerfectStreak * (1 + gm.GetScoreMultiplier()));//50 * Mathf.FloorToInt(manualTileSolvePerfectStreak / 100) * gm.level * (1 + gm.GetScoreMultiplier()));
            manualTileSolvePerfectStreakIncludedWallsPointsTEST += Mathf.FloorToInt(manualTileSolvePerfectStreakIncludedWallsTEST * (1 + gm.GetScoreMultiplier())); //Mathf.FloorToInt(50 * Mathf.FloorToInt(manualTileSolvePerfectStreakIncludedWallsTEST / 100) * gm.level * (1 + gm.GetScoreMultiplier()));
            manualTileSolvePerfectStreakPointsWithLevelTEST += Mathf.FloorToInt(manualTileSolvePerfectStreak * (1 + gm.GetScoreMultiplier()) * gm.level);
            manualTileSolvePerfectStreakIncludedWallsPointsWithLevelTEST += Mathf.FloorToInt(manualTileSolvePerfectStreakIncludedWallsTEST * (1 + gm.GetScoreMultiplier()) * gm.level);
        }            
        else if (isEasyTile) // On edge or bottom row
        {
            manualTileSolvePerfectStreakIncludedWallsTEST++;
            manualTileSolvePerfectStreakIncludedWallsPointsTEST += Mathf.FloorToInt(manualTileSolvePerfectStreakIncludedWallsTEST * (1 + gm.GetScoreMultiplier()));
            manualTileSolvePerfectStreakIncludedWallsPointsWithLevelTEST += Mathf.FloorToInt(manualTileSolvePerfectStreakIncludedWallsTEST * (1 + gm.GetScoreMultiplier()) * gm.level);
        }
        else
        {
            manualTileSolvePerfectStreak = 0;
            manualTileSolvePerfectStreakIncludedWallsTEST = 0;
        }
        
        manualTileSolveStreakHighest = Mathf.Max(manualTileSolveStreak, manualTileSolveStreakHighest);
        manualTileSolvePerfectStreakHighest = Mathf.Max(manualTileSolvePerfectStreak, manualTileSolvePerfectStreakHighest);

        manualTileSolvePerfectStreakIncludedWallsHighestTEST = Mathf.Max(manualTileSolvePerfectStreakIncludedWallsTEST, manualTileSolvePerfectStreakIncludedWallsHighestTEST);*/
    

    public void ResetManualSolveStreak()
    {
        manualTileSolveStreak = 0;        
    }
}