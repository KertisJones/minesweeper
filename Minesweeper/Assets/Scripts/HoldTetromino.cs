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
    //public TMPro.TMP_Text cleansePointText;
    public TMPro.TMP_Text cleansePointRechargeText;    
    public AudioClip cleanseReadySound;
    public AudioClip cleanseActivateSound;
    private bool cleanseReady = false;
    public int cleanseRecharge = 0;
    int manualTileSolveStreak = 0;    
    public int manualTileSolvePerfectStreak = 0;    
    public int manualTileSolvePerfectStreakIncludedWallsTEST = 0; 
    
    public int scoreMissingTest = 0;
    public int scoreRevealRemainingTest = 0;
    public int scoreRevealTest = 0;
    public int scoreRevealNoLevelTest = 0;
    int cleanseScoreHighest = 0;
    int manualTileSolveStreakHighest = 0;
    int manualTileSolvePerfectStreakHighest = 0;
    public int manualTileSolvePerfectStreakIncludedWallsHighestTEST = 0; 
    public int manualTileSolvePerfectStreakPoints = 0;
    public int manualTileSolvePerfectStreakIncludedWallsPointsTEST = 0; 

    public int manualTileSolvePerfectStreakPointsWithLevelTEST = 0;
    public int manualTileSolvePerfectStreakIncludedWallsPointsWithLevelTEST = 0; 

    public AudioClip holdSwitchSound;
    public AudioClip holdFailedSound;

    //public int manualTileSolvePerfectSteakPointsNOMISTAKESTEST = 0;
    //public int manualTileSolvePerfectSteakTotalNOMISTAKESTEST = 0;
    
    void Awake()
    {
        inputManager = InputManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //cleanseButton.transform.localScale = Vector3.zero;

        //manualTileSolvePerfectStreakPoints += Mathf.FloorToInt(50 * Mathf.FloorToInt(manualTileSolvePerfectStreak / 100) * gm.level * (1 + gm.GetScoreMultiplier()));
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
                cleanseButton.SetActive(true);
                cleanseButton.GetComponent<ButtonJiggle>().Reset();
                cleanseReady = true;
            }
            
            string exclamationSuffix = "";
            for (int i = 0; i < Mathf.Min(5, (Mathf.FloorToInt(manualTileSolvePerfectStreak / 100f))); i++)
            {
                exclamationSuffix += "!";
            }
            cleanseScoreHighest = Math.Max(cleanseScoreHighest, Mathf.FloorToInt(manualTileSolveStreak * 5 * gm.level * (1 + gm.GetScoreMultiplier()) * GetCleanseStreakMultiplier()));
            //manualTileSolvePerfectStreakIncludedWallsPointsTEST = Math.Max(cleanseScoreHighest, Mathf.FloorToInt(manualTileSolvePerfectStreakIncludedWallsTEST * 5 * gm.level * (1 + gm.GetScoreMultiplier()) * GetCleanseStreakMultiplier()));
            /*cleansePointText.text = (manualTileSolveStreak * 5 * gm.level * (1 + gm.GetScoreMultiplier()) * GetCleanseStreakMultiplier()).ToString("#,#") + " Points" + exclamationSuffix;
            if (GetCleanseStreakMultiplier() > 5)
            {
                cleansePointText.GetComponent<TMPro.Examples.VertexJitter>().enabled = true;
                //cleansePointText.GetComponent<VertexColorCyclerGradient>().enabled = true;
            }
            else
            {
                cleansePointText.GetComponent<TMPro.Examples.VertexJitter>().enabled = false;
                //cleansePointText.GetComponent<VertexColorCyclerGradient>().enabled = false;
            }*/
        }                    
        //else
            //cleanseButton.SetActive(false);  
        if (cleansePointRechargeText != null)
            cleansePointRechargeText.text = manualTileSolveStreak + ", " + manualTileSolvePerfectStreak + ", " + manualTileSolvePerfectStreakIncludedWallsTEST + " (m" + manualTileSolveStreakHighest + ", p" + manualTileSolvePerfectStreakHighest + ", w" + manualTileSolvePerfectStreakIncludedWallsHighestTEST + ")\n+c" 
            + cleanseScoreHighest + " ~ " + manualTileSolvePerfectStreakPoints + ", w" + manualTileSolvePerfectStreakIncludedWallsPointsTEST + " *L(" + manualTileSolvePerfectStreakPointsWithLevelTEST + ", w" + manualTileSolvePerfectStreakIncludedWallsPointsWithLevelTEST + ")\n-" 
            + scoreMissingTest + " + " + scoreRevealRemainingTest + " = " + scoreRevealTest + " (-" + scoreRevealNoLevelTest + "=" + (scoreRevealTest - scoreRevealNoLevelTest) + ")";
         
        /*if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.RightShift))
        {
            Hold();
        }*/

        /*
        +22680 - 11782, --(<100) ...107151
            -16173

        +123971 - 90981, --(134) ...592599
            -107154

        +50736 - 79240, --(140) ...243189
            -27963

        +88173 - 138941, --(144) ...390863
            -46952

        +77880 - 56607, --(<100) ...451433
            -92068
        
        449, 0, 0 (449, 112, 120)
        +149,664 - 99,051, 112,439 ...606,127
        -71439

425, 9, 9 (425, 98, 102)
+51207 - 33388, 39225(98) ...244243
-38933

168, 36, 37 (168, 100, 107)
+2470 - 7257, 8240(100) ...14884
-2738

5, 0, 0 (481, 95, 104)
+79761 - 55071, 68258 ...423406
-56531

218, 0, 0 (218, 140, 168)
+9471 - 32684, 43939 ...106153
-15349

134, 2, 2 (m267, p203, w221)
+c116411 ~ 199376, w238700 *L(647065, w787170)
-97469

298, 1, 1 (m298, p80, w83)
+c17750 ~ 21720, w25048 *L(46122, w54675)
-20857 + 14905 = 35827(6632)
        */
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
        
        //gm.AddScore(manualTileSolveStreak * 5 * GetCleanseStreakMultiplier());
        gm.AddScore(250);
        cleanseRecharge = 0;

        //GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
        AudioSource.PlayClipAtPoint(cleanseActivateSound, new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));
        cleanseReady = false;

        Tooltip.HideTooltip_Static();
    }

    public void AddToManualSolveStreak(bool isPerfect, bool isEasyTile = false)
    {
        manualTileSolveStreak++; 
        cleanseRecharge++;

        if (isPerfect)
        {
            manualTileSolvePerfectStreak++;
            manualTileSolvePerfectStreakIncludedWallsTEST++;

            gm.AddScore(manualTileSolvePerfectStreak, false);

            manualTileSolvePerfectStreakPoints += Mathf.FloorToInt(manualTileSolvePerfectStreak * (1 + gm.GetScoreMultiplier()));//50 * Mathf.FloorToInt(manualTileSolvePerfectStreak / 100) * gm.level * (1 + gm.GetScoreMultiplier()));
            manualTileSolvePerfectStreakIncludedWallsPointsTEST += Mathf.FloorToInt(manualTileSolvePerfectStreakIncludedWallsTEST * (1 + gm.GetScoreMultiplier())); //Mathf.FloorToInt(50 * Mathf.FloorToInt(manualTileSolvePerfectStreakIncludedWallsTEST / 100) * gm.level * (1 + gm.GetScoreMultiplier()));
            manualTileSolvePerfectStreakPointsWithLevelTEST += Mathf.FloorToInt(manualTileSolvePerfectStreak * (1 + gm.GetScoreMultiplier()) * gm.level);
            manualTileSolvePerfectStreakIncludedWallsPointsWithLevelTEST += Mathf.FloorToInt(manualTileSolvePerfectStreakIncludedWallsTEST * (1 + gm.GetScoreMultiplier()) * gm.level);
        }            
        else if (isEasyTile)
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