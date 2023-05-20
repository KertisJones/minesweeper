using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.U2D;

public class GameManager : MonoBehaviour
{
    public enum GameModeType // your custom enumeration
    {
        standard,
        endless,
        classic, // No levels, score doesn't decay, score reset timer
        sprint40Line
    };
    public GameModeType gameModeType = GameModeType.standard; 

    float startTime;
    float endtime;
    private float score = 0;
    [SerializeField]
    public int scoreMultiplier = 0;
    public int scoreMultiplierLimit = 5000;
    public float scoreMultiplierDecayPerTick= 0.1f;
    public float scoreMultiplierTimer = 0f;
    private float lastMultiplierTick = 0;
    public int comboLinesFilled = -1; // C=-1; +1 when mino locks & line filled; C= when mino locks & line not filled
    public bool lastFillWasDifficult = false; // Difficult fills are Tetrises or T-Spins
    public bool perfectClearThisRound = true;
    public bool previousPCWasTetris = false;
    public int linesClearedTarget = 150;    
    public int linesCleared = 0;
    public int tetrisweepsCleared = 0;
    public int tSpinsweepsCleared = 0;
    public bool previousClearWasDifficultSweep = false;
    public int level = 1;
    public int currentMines = 0;
    public int currentFlags = 0;
    public int safeEdgeTilesGained = 0;
    public static int sizeX = 10;
    public static int sizeY = 24;
    public static int numMines = 5;

    static float screenShakeDuration = 0.2f;
    static float screenShakeStrength = 1f;

    //private bool minesPlaced = false;

    public static GameObject[][] gameBoard;
    List<GameObject> floorTiles;
    List<GameObject> leftBorderTiles;
    List<GameObject> rightBorderTiles;
    public GameObject currentTetromino = null;
    public GameObject previousTetromino = null;

    public bool isGameOver = false;
    public bool isEndless = false;
    public bool isPaused = false;
    public bool isTitleMenu = false;
    bool canPause = true;
    public bool hasQuit = false;
    public bool cheatGodMode = false;
    public bool cheatAutoFlagMode = false;

    public bool wallTilesPlayableActive = false;

    // Statistics
    public int piecesPlaced;
    public int holds;
    public int linesweepsCleared;
    public float highestScoreMultiplier;
    public int minesSweeped;
    public int perfectClears;
    public int singlesFilled;
    public int doublesFilled;
    public int triplesFilled;
    public int tetrisesFilled;
    public int tSpinMiniNoLines;
    public int tSpinMiniSingle;
    public int tSpinMiniDouble;
    public int tSpinNoLines;
    public int tSpinSingle;
    public int tSpinDouble;
    public int tSpinTriple;

    // Options
    /*public enum WallType // your custom enumeration
    {
        disabledUntilAdded,
        playableWalls,
        zeroTileWalls,
        noWalls
    };
    public WallType wallType = WallType.disabledUntilAdded;*/
    private InputManager inputManager;
    public GameObject tile;
    public GameObject tileGroup;
    public GameObject backgroundAnimated;
    public PauseMenuMove pauseMenu;
    public PauseMenuMove settingsMenu;
    public PauseMenuMove marathonOverMenu;
    public PauseMenuMove gameOverMenu;

    public AudioClip lineClearSound;
    public AudioClip lineFullSound1;
    public AudioClip lineFullSound2;
    public AudioClip lineFullSound3;
    public AudioClip lineFullSound4;
    public AudioClip perfectClearSound;
    public AudioClip gameOverSound;

    #region Game Setup
    // Start is called before the first frame update
    void Awake()
    {
        inputManager = InputManager.Instance;

        GameObject.FindGameObjectWithTag("ScoreKeeper").GetComponent<ScoreKeeper>().runs += 1;
        /*blankTile = Instantiate(new GameObject(), new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), this.gameObject.transform) as GameObject;        
        blankTile.AddComponent<Tile>();
        blankTile.GetComponent<Tile>().isDisplay = true;
        blankTile.name = "Blank Tile";*/

        BuildGameBoard();
        
        startTime = Time.time;
        //PopulateMines();
    }

    #region Input
    void OnEnable()
    {
        inputManager.escapePress.started += _ => PressEscape();
        inputManager.restartPress.started += _ => PressRestart();
        inputManager.hardClearPress.started += _ => PressHardClear();
    }
    void OnDisable()
    {
        inputManager.escapePress.started -= _ => PressEscape();
        inputManager.restartPress.started -= _ => PressRestart();
        inputManager.hardClearPress.started -= _ => PressHardClear();
    }
    void PressEscape()
    {
        Pause(!isPaused);
    }
    void PressRestart()
    {
        ReloadScene();
    }
    void PressHardClear()
    {
        deleteFullRows(false);
        previousTetromino = null;
        currentTetromino = null;
    }
    #endregion

    // Update is called once per frame
    void Update()
    {        
        //Debug.Log((Mathf.Floor(0.04f * 100) / 100) + 0.01f);
        if (scoreMultiplierTimer > 0 && !isGameOver)
            scoreMultiplierTimer -= Time.deltaTime;
        if (scoreMultiplierTimer <= 0)
        {
            float tickTime = 0.04f; // x0.25 / sec
            if (scoreMultiplier > scoreMultiplierLimit * 0.1f)
                tickTime = 0.03f; // x0.33 / sec
            if (scoreMultiplier > scoreMultiplierLimit * 0.2f)
                tickTime = 0.02f; // x0.50 / sec

            if (scoreMultiplier > scoreMultiplierLimit * 0.3f)
                tickTime = 0.01f; // x1 / sec
            if (scoreMultiplier > scoreMultiplierLimit * 0.4f)
                tickTime = 0.005f; // x1.5 / sec
            
            if (scoreMultiplier > scoreMultiplierLimit * 0.5f)
                tickTime = 0.0025f; // x2 / sec
            if (scoreMultiplier > scoreMultiplierLimit * 0.6f)
                tickTime = 0.00125f; // x2.5 / sec
            
            if (scoreMultiplier > scoreMultiplierLimit * 0.7f)
                tickTime = 0.000625f; // x3 / sec
            if (scoreMultiplier > scoreMultiplierLimit * 0.8f)
                tickTime = 0.0003125f; // x3.5 / sec
            if (scoreMultiplier > scoreMultiplierLimit * 0.9f)
                tickTime = 0.00015625f; // x4 / sec            
            if (Time.time - lastMultiplierTick >= tickTime && !isGameOver)
            {
                if (GetScoreMultiplier() > 0)
                {
                    // Get the current level, but cap it at 30: endless mode should not be affected.
                    float tempLevel = level;
                    if (tempLevel > 30)
                        tempLevel = 30;
                    // Multiplier drain per tick will be half of the level
                    int multiplierDrain = Mathf.CeilToInt(tempLevel / 2);
                    // If the level is a multiple of 5, increase it by 1 so that the display doesn't flash between .5 and no decimal; the true cap on marathon games is 16 at level 29.
                    if (multiplierDrain % 5 == 0)
                        multiplierDrain += 1;
                    // Drain the multiplier...
                    SetScoreMultiplier(-1 * multiplierDrain, 0);
                    // If the multiplier is gone, turn off the background animation.
                    if (GetScoreMultiplier() <= 0)
                        backgroundAnimated.SetActive(false);
                }            
            }
            scoreMultiplierTimer = 0;
        }
        
        

        // Fixed Marathon: 10 per level
        if (linesCleared >= level * 10)
            level += 1;
        
        /*if (cheatGodMode)
        {
            if (Input.GetKeyDown(KeyCode.M))
                SetScoreMultiplier(4f, 30);
            if (Input.GetKeyDown(KeyCode.K))
                AddSafeTileToEdges();
            if (Input.GetKeyDown(KeyCode.L))
                linesCleared++;
        }*/
    }

    void BuildGameBoard()
    {
        // Playable Game Board
        gameBoard = new GameObject[sizeX][];
        for (int i = 0; i < sizeX; i++)
        {
            GameObject[] tileColumn = new GameObject[sizeY];

            for (int j = 0; j < sizeY; j++)
            {
                tileColumn[j] = null; // blankTile;
            }

            gameBoard[i] = tileColumn;
        }

        // Bottom Display Tiles
        floorTiles = new List<GameObject>();
        for (int i = -1; i <= sizeX; i++)
        {
            //place display tiles at bottom
            GameObject newTile = Instantiate(tile, new Vector3(i, -1, 0), new Quaternion(0, 0, 0, 0), this.gameObject.transform) as GameObject;
            newTile.name = "Tile (" + i + ", " + -1 + ")";
            newTile.GetComponent<Tile>().coordX = i;
            newTile.GetComponent<Tile>().coordY = -1;
            newTile.GetComponent<Tile>().isRevealed = true;
            newTile.GetComponent<Tile>().isDisplay = true;
            floorTiles.Add(newTile);
        }

        // Left and Right Tiles
        leftBorderTiles = new List<GameObject>();
        rightBorderTiles = new List<GameObject>();
        if (wallTilesPlayableActive)
        {
            for (int i = 0; i < sizeY; i++)
            {
                //place display tile on left side
                GameObject newTile = Instantiate(tileGroup, new Vector3(-1, i, 0), new Quaternion(0, 0, 0, 0), this.gameObject.transform) as GameObject;
                newTile.name = "Tile Group (" + -1 + ", " + i + ")";
                newTile.GetComponentInChildren<Tile>().coordX = -1;
                newTile.GetComponentInChildren<Tile>().coordY = i;
                newTile.GetComponent<Group>().isDisplay = true;            
                newTile.GetComponent<Group>().minePercent = 10;
                leftBorderTiles.Add(newTile);

                //place display tile on right side
                newTile = Instantiate(tileGroup, new Vector3(sizeX, i, 0), new Quaternion(0, 0, 0, 0), this.gameObject.transform) as GameObject;
                newTile.name = "Tile Group (" + sizeX + ", " + i + ")";
                newTile.GetComponentInChildren<Tile>().coordX = sizeX;
                newTile.GetComponentInChildren<Tile>().coordY = i;
                newTile.GetComponent<Group>().isDisplay = true;            
                newTile.GetComponent<Group>().minePercent = 10;
                rightBorderTiles.Add(newTile);
            }

            // Place bombs Left and Right Tiles
            // Place mine at bottom most space
            leftBorderTiles[0].GetComponentInChildren<Tile>().isMine = true;
            rightBorderTiles[0].GetComponentInChildren<Tile>().isMine = true;

            // Prevent most 0-Cascade tiles from spawning at index 10-18
            for (int i = 2+Random.Range(0, 3); i < sizeY - 5; i+=Random.Range(1, 4))
            {
                if (i < sizeY - 4)
                    if (!leftBorderTiles[i-1].GetComponentInChildren<Tile>().isMine || !leftBorderTiles[i-2].GetComponentInChildren<Tile>().isMine)
                        leftBorderTiles[i].GetComponentInChildren<Tile>().isMine = true;
            }
            for (int i = 2+Random.Range(0, 3); i < sizeY - 5; i+=Random.Range(1, 4))
            {
                if (i < sizeY - 4)
                    if (!rightBorderTiles[i-1].GetComponentInChildren<Tile>().isMine || !rightBorderTiles[i-2].GetComponentInChildren<Tile>().isMine)
                        rightBorderTiles[i].GetComponentInChildren<Tile>().isMine = true;
            }
        }
    }

    /*void BuildMinesweeperBoard()
    {
        gameBoard = new GameObject[sizeX][];
        for (int i = 0; i < sizeX; i++)
        {
            GameObject[] tileColumn = new GameObject[sizeY];

            for (int j = 0; j < sizeY; j++)
            {
                GameObject newTile = Instantiate(tile, new Vector3(i, j, 0), new Quaternion(0, 0, 0, 0), this.gameObject.transform) as GameObject;
                newTile.name = "Tile (" + i + ", " + j + ")";
                newTile.GetComponent<Tile>().coordX = i;
                newTile.GetComponent<Tile>().coordY = j;

                tileColumn[j] = newTile;
            }

            gameBoard[i] = tileColumn;
        }
    }*/

    /*void PopulateMines(int startX = -10, int startY = -10)
    {
        int currentMines = 0;

        while (currentMines < numMines)
        {
            int randX = Random.Range(0, sizeX - 1);
            int randY = Random.Range(0, sizeY - 1);

            if (!(randX == startX && randY == startY)
                && !(randX == startX - 1 && randY + 1 == startY)
                && !(randX == startX - 1 && randY == startY)
                && !(randX == startX - 1 && randY - 1 == startY)
                && !(randX == startX && randY + 1 == startY)
                && !(randX == startX && randY - 1 == startY)
                && !(randX == startX + 1 && randY + 1 == startY)
                && !(randX == startX + 1 && randY == startY)
                && !(randX == startX + 1 && randY - 1 == startY))
            {
                if (GetGameTile(randX, randY).isMine == false)
                {
                    GetGameTile(randX, randY).isMine = true;
                    DetectProximity(randX, randY);
                    currentMines += 1;
                }
            }                        
        }

        minesPlaced = true;
    }*/

    /*void DetectProximity(int x, int y)
    {
        foreach (Tile t in GetNeighborTiles(x, y))
        {
            t.nearbyMines += 1;
        }
    }*/

    //public void RevealTile(int x, int y, int nearbyMines, bool isMine)
    //{
        //if (!minesPlaced)
            //PopulateMines(x, y);
    //}
    #endregion
    #region Minesweeper Logic
    public ArrayList GetNeighborTiles(int x, int y)
    {
        ArrayList neighbors = new ArrayList();

        if (GetGameTile(x - 1, y) != null)
            neighbors.Add(GetGameTile(x - 1, y));
        if (GetGameTile(x - 1, y - 1) != null)
            neighbors.Add(GetGameTile(x - 1, y - 1));
        if (GetGameTile(x - 1, y + 1) != null)
            neighbors.Add(GetGameTile(x - 1, y + 1));
        if (GetGameTile(x + 1, y) != null)
            neighbors.Add(GetGameTile(x + 1, y));
        if (GetGameTile(x + 1, y - 1) != null)
            neighbors.Add(GetGameTile(x + 1, y - 1));
        if (GetGameTile(x + 1, y + 1) != null)
            neighbors.Add(GetGameTile(x + 1, y + 1));
        if (GetGameTile(x, y - 1) != null)
            neighbors.Add(GetGameTile(x, y - 1));
        if (GetGameTile(x, y + 1) != null)
            neighbors.Add(GetGameTile(x, y + 1));

        
        /*if (x > 0)
        {
            if (y >= 0)
                if (GetGameTile(x - 1, y) != null)
                    neighbors.Add(GetGameTile(x - 1, y));

            if (y > 0)
                if (GetGameTile(x - 1, y - 1) != null)
                    neighbors.Add(GetGameTile(x - 1, y - 1));

            if (y < sizeY - 1)
                if (y >= -1)
                    if (GetGameTile(x - 1, y + 1) != null)
                        neighbors.Add(GetGameTile(x - 1, y + 1));
        }
        if (x < sizeX - 1)
        {
            if (y >= 0)
                if (GetGameTile(x + 1, y) != null)
                    neighbors.Add(GetGameTile(x + 1, y));
            
            if (y > 0)
                if (GetGameTile(x + 1, y - 1) != null)
                    neighbors.Add(GetGameTile(x + 1, y - 1));
            
            if (y < sizeY - 1)
                if (y >= -1)
                    if (GetGameTile(x + 1, y + 1) != null)
                        neighbors.Add(GetGameTile(x + 1, y + 1));
        }
        if (y > 0)
            if (GetGameTile(x, y - 1) != null)
                neighbors.Add(GetGameTile(x, y - 1));

        if (y < sizeY - 1)
            if (y >= -1)
                if (GetGameTile(x, y + 1) != null)
                    neighbors.Add(GetGameTile(x, y + 1));*/

        return neighbors;
    }

    public Tile GetGameTile(int x, int y)
    {
        /*if (x < 0 || y < 0 || x >= gameBoard.GetLength(0) || y >= gameBoard.GetLength(1))
        {
            Debug.Log("GetGameTile: Out of Bounds; " + x + ", " + y);
            return null;
        }*/
        if (x >= 0 && x < sizeX && y >= 0 && y < sizeY)
        {
            if (gameBoard[x][y] != null)
                return gameBoard[x][y].GetComponent<Tile>();
            else
                return null;
        }
        else if (x == -1 && y >= 0 && y < leftBorderTiles.Count)
        {
            return leftBorderTiles[y].GetComponentInChildren<Tile>();
        }
        else if (x == sizeX && y >= 0 && y < rightBorderTiles.Count)
        {
            return rightBorderTiles[y].GetComponentInChildren<Tile>();
        }
        else if (x >= -1 && x <= sizeX && y == -1)
        {
            return floorTiles[x+1].GetComponent<Tile>();
        }
        //Debug.Log("Failed to find tile " + x + ", " + y);
        return null;
    }   

    public static bool insideBorder(Vector2 pos)
    {
        return ((int)pos.x >= 0 &&
                (int)pos.x < sizeX &&
                (int)pos.y >= 0 &&
                (int)pos.y < sizeY);
    }
    #endregion
    #region Tetris Logic
    public static void deleteRow(int y)
    {
        for (int x = 0; x < sizeX; ++x)
        {
            gameBoard[x][y].gameObject.GetComponent<Tile>().isDestroyed = true;
            Destroy(gameBoard[x][y].gameObject);
            gameBoard[x][y] = null;
        }
    }

    public static void decreaseRow(int y)
    {
        Group activeTetrominoInRow = null;
        for (int x = 0; x < sizeX; ++x)
        {
            if (gameBoard[x][y] != null)
            {
                if (gameBoard[x][y].GetComponentInParent<Group>().isFalling) // Active tetromino should not be moved directly, or it will cause a rotation error
                {
                    activeTetrominoInRow = gameBoard[x][y].GetComponentInParent<Group>();
                }
                else // All static tiles should be moved down
                {
                    // Move one towards bottom
                    gameBoard[x][y - 1] = gameBoard[x][y];
                    gameBoard[x][y] = null;

                    // Update Block position
                    gameBoard[x][y - 1].GetComponent<Tile>().coordY -= 1;
                }                
            }
        }

        if (activeTetrominoInRow != null) // Move the active tetromino after the rest of the row has finished
        {
            // The active tetromino should not change position, unless it needs to fall in order for blocks above it to have a place to land
            activeTetrominoInRow.WallKickMove(0, 1); // Attempt to move the tetromino up. This will do nothing if the space is blocked
            activeTetrominoInRow.Fall(); // Move the tetromino down. This will put it back in the same place if it was moved up, otherwise it will get the tetromino out of the way
        }
    }

    public Group GetActiveTetromino()
    {
        for (int x = 0; x < sizeX; ++x)
        {
            for (int y = 0; y < sizeY; ++y)
            {
                if (gameBoard[x][y] != null)
                {
                    if (gameBoard[x][y].GetComponentInParent<Group>().isFalling) // Active tetromino should not be moved directly, or it will cause a rotation error
                    {
                        return gameBoard[x][y].GetComponentInParent<Group>();
                    }
                }
            }
        }
        return null;
    }

    public static void decreaseRowsAbove(int y)
    {
        for (int i = y; i < sizeY; ++i)
            decreaseRow(i);
    }

    public static bool isRowFull(int y)
    {
        for (int x = 0; x < sizeX; ++x)
            if (gameBoard[x][y] == null)
                return false;
            else if (gameBoard[x][y].GetComponentInParent<Group>().isFalling)
                return false;
        return true;
    }

    public static bool isRowEmpty(int y)
    {
        for (int x = 0; x < sizeX; ++x)
            if (gameBoard[x][y] != null)
                if (!gameBoard[x][y].GetComponentInParent<Group>().isFalling)
                    return false;
        return true;
    }

    // Checks whether a perfect clear is possible if all currently solved lines were cleared, then clears those lines if it would create a PC.
    public static void CheckForPossiblePerfectClear()
    {        
        for (int y = 0; y < sizeY; ++y)
        {
            if (!isRowEmpty(y))
                if (!isRowSolved(y))
                    return;
        }
        int rowsFilled = 0;
        if (GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().previousTetromino != null)
            rowsFilled = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().previousTetromino.GetComponent<Group>().rowsFilled;
        deleteFullRows(true, rowsFilled);
    }

    public void CheckForPerfectClear(int rowsCleared, int rowsFilled, bool isInstantPC)
    {
        if (perfectClearThisRound)
            return;
        if (linesCleared == 0)
            return;
        //if (linesCleared == 0)
            //return;
        // Make sure nothing is on the board
        for (int y = 0; y < sizeY; ++y)
        {
            if (!isRowEmpty(y))
                return;
        }

        perfectClearThisRound = true;
        
        int rowsActuallyFilled = rowsFilled;
        if (rowsCleared < rowsActuallyFilled)
            rowsActuallyFilled = rowsCleared;
        
        float pcScore = 800;

        // Give rewards for a perfect clear!
        if (rowsActuallyFilled == 0 || rowsActuallyFilled == 1)
        {
            pcScore = 800 + (100 * (rowsCleared - 1));        
            SetScoreMultiplier(10, 30);
            previousPCWasTetris = false;
        }
        else if (rowsActuallyFilled == 2)
        {
            pcScore = 1200 + (150 * (rowsCleared - 2));        
            SetScoreMultiplier(25, 30);
            previousPCWasTetris = false;
        }
        else if (rowsActuallyFilled == 3)
        {
            pcScore = 1800 + (200 * (rowsCleared - 3));        
            SetScoreMultiplier(50, 30);
            previousPCWasTetris = false;
        }
        else if (rowsActuallyFilled >= 4)
        {
            pcScore = 500 * rowsCleared;
            if (previousPCWasTetris)
                pcScore = pcScore * 1.6f;            
            SetScoreMultiplier(100, 30);
            previousPCWasTetris = true;
        }

        if (isInstantPC)
        {
            AddSafeTileToEdges();
            pcScore = pcScore * 1.5f;
        }

        AddScore(((int)pcScore));

        /*if (previousTetromino.GetComponent<Group>().rowsFilled == 4) // Tetrisweep Perfect Clear!
        {
            Debug.Log("Tetrisweep Perfect Clear!");
            AddScore(3200);
            SetScoreMultiplier(50, 50);
        }
        else // Normal Perfect Clear!
        {
            Debug.Log("Perfect Clear!");
            AddScore(2000);
            SetScoreMultiplier(15, 30);
        }*/
        
        perfectClears += 1;        

        GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
        AudioSource.PlayClipAtPoint(perfectClearSound, new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));
    }
    #endregion
    #region Tetrisweeper Solved Logic
    public static int deleteFullRows(bool getMultiplier = true, int rowsFilled = 0, bool isTriggeredByLock = false)
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (gm.isGameOver)
            return 0;
        
        int rowsCleared = 0;
        int linesweepsCleared = 0;
        
        // Score all of the solved rows
        for (int y = 0; y < sizeY; ++y)
        {
            if (isRowSolved(y))
            {
                if (!getMultiplier)
                {
                    gm.ResetScoreMultiplier();
                    gm.RemoveSafeTileFromEdges();
                }

                bool isLinesweep = scoreSolvedRow(y, getMultiplier);
                
                gm.linesCleared++;
                rowsCleared++;
                if (isLinesweep)
                {
                    linesweepsCleared++;
                    gm.linesweepsCleared += 1;
                }
                    
            }
        }

        // Delete the finished Rows
        for (int y = 0; y < sizeY; ++y)
        {
            if (isRowSolved(y))
            {
                deleteRow(y);
                decreaseRowsAbove(y + 1);
                --y;
            }
        }

        // Linesweep: Row was solved before the next tetromino was placed
        if (linesweepsCleared > 0)
        {
            int linesweepScore = 75 * (linesweepsCleared * linesweepsCleared);
            
            if (isTriggeredByLock) // Instant Sweep multiplier
                gm.AddScore((int)(linesweepScore * 1.5f));
            else
                gm.AddScore(linesweepScore);

            if (getMultiplier)
                gm.SetScoreMultiplier(10 * linesweepsCleared, 10f);
        } 

        if (rowsCleared > 0)
        {
            gm.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(gm.lineClearSound, new Vector3(0, 0, 0), 0.75f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));

            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);
        
            // Lines Cleared Points
            gm.AddScore(75 * (rowsCleared * rowsCleared));
            //gm.SetScoreMultiplier(0.2f * (y + 1), 2f);
            if (getMultiplier)
                gm.SetScoreMultiplier(rowsCleared * 5, rowsCleared * 2);
            
            if (rowsCleared > gm.safeEdgeTilesGained)
                gm.AddSafeTileToEdges();
            
            bool isDifficultSweep = false;
            if (gm.previousTetromino != null)
                if (gm.previousTetromino.GetComponent<Group>().CheckForTetrisweeps(getMultiplier))
                    isDifficultSweep = true;
            if (gm.currentTetromino != null)
                if (gm.currentTetromino.GetComponent<Group>().CheckForTetrisweeps(getMultiplier, isTriggeredByLock))
                    isDifficultSweep = true;

            gm.previousClearWasDifficultSweep = isDifficultSweep;
            //currentTetromino = null;
        }
        gm.CheckForPerfectClear(rowsCleared, rowsFilled, isTriggeredByLock);
        GameManager.markSolvedRows();
        if (gm.linesCleared >= gm.linesClearedTarget && gm.isEndless == false) 
            gm.Pause(true, true);
        return rowsCleared;
    }

    public static void markSolvedRows()
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (gm.isGameOver)
            return;
        
        for (int y = 0; y < sizeY; ++y)
        {
            if (isRowSolved(y))
            {
                for (int x = 0; x < sizeX; ++x)
                {
                    gm.GetGameTile(x, y).isRowSolved = true;
                }
                if (gm.GetGameTile(-1, y) != null)
                    gm.GetGameTile(-1, y).isRowSolved = true;
                if (gm.GetGameTile(sizeX, y) != null)
                    gm.GetGameTile(sizeX, y).isRowSolved = true;
            }
            else
            {
                for (int x = 0; x < sizeX; ++x)
                {
                    if (gm.GetGameTile(x, y) != null)
                    {
                        gm.GetGameTile(x, y).isRowSolved = false;
                    }
                }
                if (gm.GetGameTile(-1, y) != null)
                    gm.GetGameTile(-1, y).isRowSolved = false;
                if (gm.GetGameTile(sizeX, y) != null)
                    gm.GetGameTile(sizeX, y).isRowSolved = false;
            }
        }
    }

    public static bool isRowSolved(int y)
    {
        if (!isRowFull(y))
            return false;
        
        bool isSolved = true;
        for (int x = 0; x < sizeX; ++x)
        {
            if (gameBoard[x][y] != null)
            {
                if (!(gameBoard[x][y].GetComponent<Tile>().isRevealed && !gameBoard[x][y].GetComponent<Tile>().isMine)
                    && !(!gameBoard[x][y].GetComponent<Tile>().isRevealed && gameBoard[x][y].GetComponent<Tile>().isMine && gameBoard[x][y].GetComponent<Tile>().isFlagged))
                    isSolved = false;
            }
        }
        return isSolved;
    }

    public void AddSafeTileToEdges() {

        // Place display tiles at bottom
        GameObject newTile = Instantiate(tile, new Vector3(-1, leftBorderTiles.Count, 0), new Quaternion(0, 0, 0, 0), this.gameObject.transform) as GameObject;
        newTile.name = "Tile (" + -1 + ", " + leftBorderTiles.Count + ")";
        newTile.GetComponent<Tile>().coordX = -1;
        newTile.GetComponent<Tile>().coordY = leftBorderTiles.Count;
        newTile.GetComponent<Tile>().isRevealed = true;
        newTile.GetComponent<Tile>().isDisplay = true;
        newTile.GetComponentInChildren<Tile>().shimmerOverlay.gameObject.SetActive(true);
        leftBorderTiles.Add(newTile); 

        newTile = Instantiate(tile, new Vector3(10, rightBorderTiles.Count, 0), new Quaternion(0, 0, 0, 0), this.gameObject.transform) as GameObject;
        newTile.name = "Tile (" + 10 + ", " + rightBorderTiles.Count + ")";
        newTile.GetComponent<Tile>().coordX = 10;
        newTile.GetComponent<Tile>().coordY = rightBorderTiles.Count;
        newTile.GetComponent<Tile>().isRevealed = true;
        newTile.GetComponent<Tile>().isDisplay = true;
        newTile.GetComponentInChildren<Tile>().shimmerOverlay.gameObject.SetActive(true);
        rightBorderTiles.Add(newTile); 

        // Left and Right Tiles
        /*
        //GameObject topLeftTile = leftBorderTiles[leftBorderTiles.Count - 1];
        //GameObject topRightTile = rightBorderTiles[rightBorderTiles.Count - 1];
        int posY = leftBorderTiles[0].GetComponentInChildren<Tile>().coordY;
        leftBorderTiles.RemoveAt(leftBorderTiles.Count - 1);
        rightBorderTiles.RemoveAt(rightBorderTiles.Count - 1);
        //Destroy(topLeftTile);
        //Destroy(topRightTile);
        for (int i = 0; i < leftBorderTiles.Count; i++)
        {
            leftBorderTiles[i].GetComponentInChildren<Tile>().coordY += 1;
            rightBorderTiles[i].GetComponentInChildren<Tile>().coordY += 1;

            if (leftBorderTiles[i].GetComponentInChildren<Tile>().coordY >= sizeY - 4)
            {
                if (leftBorderTiles[i].GetComponentInChildren<Tile>().isMine)
                {
                    currentMines--;
                    leftBorderTiles[i].GetComponentInChildren<Tile>().isMine = false;
                }
                if (leftBorderTiles[i].GetComponentInChildren<Tile>().isFlagged)
                {
                    currentFlags--;
                    leftBorderTiles[i].GetComponentInChildren<Tile>().isFlagged = false;
                }
                
            }
            if (rightBorderTiles[i].GetComponentInChildren<Tile>().coordY >= sizeY - 4)
            {
                if (rightBorderTiles[i].GetComponentInChildren<Tile>().isMine)
                {
                    currentMines--;
                    rightBorderTiles[i].GetComponentInChildren<Tile>().isMine = false;
                }
                if (rightBorderTiles[i].GetComponentInChildren<Tile>().isFlagged)
                {
                    currentFlags--;
                    rightBorderTiles[i].GetComponentInChildren<Tile>().isFlagged = false;
                }
            }
        }
        topLeftTile.GetComponentInChildren<Tile>().coordY = posY;
        topRightTile.GetComponentInChildren<Tile>().coordY = posY;
        topLeftTile.GetComponentInChildren<Tile>().Reveal();
        topRightTile.GetComponentInChildren<Tile>().Reveal();
        topLeftTile.GetComponentInChildren<Tile>().shimmerOverlay.gameObject.SetActive(true);
        topRightTile.GetComponentInChildren<Tile>().shimmerOverlay.gameObject.SetActive(true);
        leftBorderTiles.Insert(0, topLeftTile); 
        rightBorderTiles.Insert(0, topRightTile); 
        */

        if (safeEdgeTilesGained == 0)
        {
            GameObject.Find("Tile (-1, -1)").GetComponent<Tile>().shimmerOverlay.gameObject.SetActive(true);
            GameObject.Find("Tile (10, -1)").GetComponent<Tile>().shimmerOverlay.gameObject.SetActive(true);
        }

        safeEdgeTilesGained++;
    }

    public void RemoveSafeTileFromEdges() 
    {
        if (safeEdgeTilesGained == 0)
            return;
        // Remove display tiles from the top
        GameObject leftTile = leftBorderTiles[leftBorderTiles.Count-1];
        leftBorderTiles.RemoveAt(leftBorderTiles.Count-1);
        Destroy(leftTile);

        GameObject rightTile = rightBorderTiles[rightBorderTiles.Count-1];
        rightBorderTiles.RemoveAt(rightBorderTiles.Count-1);
        Destroy(rightTile);

        safeEdgeTilesGained--;
    }
    #endregion
    #region Gamestate Logic
    public void EndGame()
    {
        if (cheatGodMode)
            return;
        if (isGameOver)
            return;
        
        endtime = GetTime();
        isGameOver = true;

        if (!isTitleMenu)
            GameObject.FindGameObjectWithTag("ScoreKeeper").GetComponent<ScoreKeeper>().SaveCurrentGame();
        
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(1, 1);

        // Reveal all tiles!
        for (int i = -1; i <= sizeX; i++)
        {
            for (int j = -1; j < sizeY + 4; j++)
            {
                if (GetGameTile(i, j) != null)
                {
                    //GetGameTile(i, j).isFlagged = false;
                    GetGameTile(i, j).Reveal(true);
                }
            }
        }

        if (!isTitleMenu)
            GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>().Stop();

        GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
        AudioSource.PlayClipAtPoint(gameOverSound, new Vector3(0, 0, 0), 0.1f * PlayerPrefs.GetFloat("SoundVolume", 0.5f));

        StartCoroutine(GameOver());        
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        if (!isTitleMenu)
            gameOverMenu.isActive = true;
    }
    #endregion
    #region Scoring
    public void AddScore(int newScore, bool levelMultiplierActive = true, bool scoreMultiplierActive = true) 
    {
        float tempScore = newScore;
        if (GetScoreMultiplier() > 0)
            tempScore = tempScore * (1 + GetScoreMultiplier());
        if (levelMultiplierActive)
            tempScore = tempScore * level;        
        score += tempScore;
    }

    public void SetScoreMultiplier(int mult, float duration, bool isSweep = false)
    {
        float sm = GetScoreMultiplier();
        scoreMultiplier += mult;
        if (scoreMultiplier < 0)
            scoreMultiplier = 0;
        if (scoreMultiplier > scoreMultiplierLimit)
            scoreMultiplier = scoreMultiplierLimit;
        //Debug.Log(gameObject.name + ": " + sm + " + " + mult + " = " + scoreMultiplier + " -> " + GetScoreMultiplier());
        //if (duration > scoreMultiplierTimer)
            //scoreMultiplierTimer = duration;
        float addDuration = 0.25f;
        if (isSweep)
            addDuration = 1f;
        if (mult > 0)
        {
            if (scoreMultiplierTimer >= 0)
                scoreMultiplierTimer += addDuration;
            else
                scoreMultiplierTimer = addDuration;
            if (scoreMultiplierTimer > 16)
                scoreMultiplierTimer = 16;
        }

        if (GetScoreMultiplier() > 0)
            backgroundAnimated.SetActive(true);
        if (GetScoreMultiplier() > highestScoreMultiplier)
            highestScoreMultiplier = GetScoreMultiplier();
        lastMultiplierTick = Time.time;
    }

    public void ResetScoreMultiplier()
    {
        //scoreMultiplier = 0;
        scoreMultiplierTimer = 0;        
    }

    public float GetScoreMultiplier()
    {
        //scoreMultiplier = Mathf.Floor(scoreMultiplier * 100) / 100;
        return scoreMultiplier / 100f;
    }

    // Returns true if this row was a linesweep
    public static bool scoreSolvedRow(int y, bool getMultiplier = true)
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //int rowScore = 0;
        int minesFlagged = 0;
        bool containsPreviousTetromino = false;
        for (int x = 0; x < sizeX; ++x)
        {
            if (gameBoard[x][y] != null)
            {
                if (gameBoard[x][y].GetComponent<Tile>().isMine)
                {
                    //rowScore += 50;
                    minesFlagged += 1;
                }
                /*else
                {
                    rowScore += gameBoard[x][y].GetComponent<Tile>().nearbyMines;
                }*/
                if (gameBoard[x][y].GetComponentInParent<Group>().gameObject == gm.previousTetromino)
                    containsPreviousTetromino = true;
            }
        }
        //gm.AddScore(50 * (y + 1));
        //gm.SetScoreMultiplier(0.2f * (y + 1), 2f);
        //if (getMultiplier)
            //gm.SetScoreMultiplier(0.5f, 2f);
        
        gm.currentMines -= minesFlagged;
        gm.currentFlags -= minesFlagged;
        gm.minesSweeped += minesFlagged;
        return containsPreviousTetromino;
    }

    public static int scoreFullRows(Transform tetronimo)
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (gm.isGameOver)
            return 0;

        HashSet<int> rowsToCheck = new HashSet<int>();

        foreach (Transform child in tetronimo.transform)
        {
            if (child.gameObject.GetComponent<Tile>() != null)
            {
                rowsToCheck.Add(child.gameObject.GetComponent<Tile>().coordY);
            }
        }

        int fullRows = 0;
        foreach (int row in rowsToCheck)
        {
            if (isRowFull(row))
            {
                fullRows++;
            }
        }
        if (fullRows > 0)
        {
            AudioClip clipToPlay = null;
            switch (fullRows) {
                case 1:
                    clipToPlay = gm.lineFullSound1;
                    gm.AddScore(100);
                    gm.SetScoreMultiplier(3, 5f);
                    break;
                case 2:
                    clipToPlay = gm.lineFullSound2;
                    gm.AddScore(300);
                    gm.SetScoreMultiplier(5, 5f);
                    break;
                case 3:
                    clipToPlay = gm.lineFullSound3;
                    gm.AddScore(500);
                    gm.SetScoreMultiplier(8, 5f);
                    break;
                default:
                    clipToPlay = gm.lineFullSound4;
                    int actionScore = 800;
                    if (gm.lastFillWasDifficult)
                    {
                        gm.AddScore(Mathf.RoundToInt(actionScore * 1.5f));
                        gm.SetScoreMultiplier(20, 5f);
                    }                        
                    else
                    {
                        gm.AddScore(actionScore);
                        gm.SetScoreMultiplier(10, 5f);
                    }
                    break;
            }
            // C-c-c-Combo!
            if (gm.comboLinesFilled > 0)
                gm.AddScore(50 * gm.comboLinesFilled);

            gm.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(clipToPlay, new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));

            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);

            //Debug.Log("Tetris rows full: " + fullRows);
            
        }        
        return fullRows;
    }
    #endregion
    #region Helper Functions
    public void ReloadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitToTitleMenu () 
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Title");
    }
    public void Quit()
    {
        Application.Quit();
        hasQuit = true;    
    }
    public void PauseFromButton(bool pause) 
    {
        Pause(pause, false, true);
    }
    public void Pause(bool pause, bool isMarathonOverPause = false, bool bypassTitlePause = false) 
    {
        if (isGameOver)
            return;
        if (isTitleMenu && !bypassTitlePause)
            return;
        if (pause && !canPause)
            return;

        if (pause)
        {
            Time.timeScale = 0;
            isPaused = true;            
            canPause = false;
            if (isMarathonOverPause)
            {
                isEndless = true;
                marathonOverMenu.isActive = true;                
            }
            else
            {
                if (!bypassTitlePause)
                    pauseMenu.isActive = true;
            }
        }
        else
        {
            if (!isGameOver)
            {
                Time.timeScale = 1;
                isPaused = false;       
                pauseMenu.isActive = false;
                marathonOverMenu.isActive = false;
                if (!bypassTitlePause)
                    settingsMenu.isActive = false;
                StartCoroutine(ResetPause());
            }
        }        
    }
    IEnumerator ResetPause()
    {
        yield return new WaitForSeconds(0.4f);
        canPause = true;
    }
    public void Resume()
    {
        Pause(false);
    }
    public float GetTime()
    {
        if (isGameOver)
            return endtime;
        return Time.time - startTime;
    }
    public float GetScore()
    {
        return score;
    }
    public static Vector2 roundVec2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }
    #endregion
}
