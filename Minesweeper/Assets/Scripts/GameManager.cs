using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.U2D;
using Krivodeling.UI.Effects;

public class GameManager : MonoBehaviour
{
    public enum GameModeType // your custom enumeration
    {
        standard,
        classic, // No levels, score doesn't decay, score reset timer
        tetrisweepOnly,
        zen,
        sprint40Line,
        throwback
    };
    public GameModeType gameModeType = GameModeType.standard; 

    float startTime;
    float endtime;
    private float score = 0;
    private float scoreMultiplier = 0;
    public float scoreMultiplierDecayPerTick= 0.1f;
    private int scoreMultiplierDecayTicksPerSecond = 4;
    private float lastMultiplierTick = 0;
    public int comboLinesFilled = -1; // C=-1; +1 when mino locks & line filled; C= when mino locks & line not filled
    public bool lastFillWasDifficult = false; // Difficult fills are Tetrises or T-Spins
    public bool perfectClearThisRound = true;
    public int linesCleared = 0;
    public int tetrisweepsCleared = 0;
    public int tSpinsweepsCleared = 0;
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
    public GameObject previousTetromino = null;

    public bool isGameOver = false;
    public bool isPaused = false;
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
    public GameObject tile;
    public GameObject tileGroup;
    public GameObject backgroundAnimated;
    public PauseMenuMove pauseMenu;
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
        GameObject.FindGameObjectWithTag("ScoreKeeper").GetComponent<ScoreKeeper>().runs += 1;
        /*blankTile = Instantiate(new GameObject(), new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), this.gameObject.transform) as GameObject;        
        blankTile.AddComponent<Tile>();
        blankTile.GetComponent<Tile>().isDisplay = true;
        blankTile.name = "Blank Tile";*/

        BuildGameBoard();
        
        startTime = Time.time;
        //PopulateMines();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastMultiplierTick >= 1f / scoreMultiplierDecayTicksPerSecond && !isGameOver)
        {
            if (GetScoreMultiplier() > 0)
            {
                SetScoreMultiplier(-1 * scoreMultiplierDecayPerTick, 0);
                if (GetScoreMultiplier() <= 0)
                    backgroundAnimated.SetActive(false);
            }
            lastMultiplierTick = Time.time;
        }

        // Fixed Marathon: 10 per level
        if (linesCleared >= level * 10)
            level += 1;
        
        if (cheatGodMode)
        {
            if (Input.GetKeyDown(KeyCode.M))
                SetScoreMultiplier(4f, 30);
            if (Input.GetKeyDown(KeyCode.K))
                AddSafeTileToEdges();
            if (Input.GetKeyDown(KeyCode.L))
                linesCleared++;
        }
        
        if (Input.GetKeyDown("escape"))
        {
            Pause(!isPaused);
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ReloadScene();
        }
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

    public void CheckForPerfectClear()
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

        // Give rewards for a perfect clear!
        AddScore(2000);
        SetScoreMultiplier(15, 30);
        AddSafeTileToEdges();

        perfectClears += 1;

        GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
        AudioSource.PlayClipAtPoint(perfectClearSound, new Vector3(0, 0, 0), 1f);
    }
    #endregion
    #region Tetrisweeper Solved Logic
    public static void deleteFullRows()
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (gm.isGameOver)
            return;
        
        for (int y = 0; y < sizeY; ++y)
        {
            if (isRowFull(y))
            {
                if (isRowSolved(y))
                {
                    scoreSolvedRow(y);
                    gm.linesCleared++;
                    deleteRow(y);
                    decreaseRowsAbove(y + 1);
                    --y;

                    gm.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                    AudioSource.PlayClipAtPoint(gm.lineClearSound, new Vector3(0, 0, 0), 0.75f);

                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);
                }
            }
        }

        gm.CheckForPerfectClear();
    }

    public static bool isRowSolved(int y)
    {
        bool isSolved = true;
        for (int x = 0; x < sizeX; ++x)
            if (gameBoard[x][y] != null)
            {
                if (!(gameBoard[x][y].GetComponent<Tile>().isRevealed && !gameBoard[x][y].GetComponent<Tile>().isMine)
                    && !(!gameBoard[x][y].GetComponent<Tile>().isRevealed && gameBoard[x][y].GetComponent<Tile>().isMine && gameBoard[x][y].GetComponent<Tile>().isFlagged))
                    isSolved = false;
            }
        return isSolved;
    }

    public void AddSafeTileToEdges() {

        //place display tiles at bottom
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
    #endregion
    #region Gamestate Logic
    public void EndGame()
    {
        if (cheatGodMode)
            return;
        
        endtime = GetTime();
        isGameOver = true;
        
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(1, 1);

        // Reveal all tiles!
        for (int i = -1; i <= sizeX; i++)
        {
            for (int j = -1; j < sizeY + 4; j++)
            {
                if (GetGameTile(i, j) != null)
                {
                    //GetGameTile(i, j).isFlagged = false;
                    GetGameTile(i, j).Reveal();
                }
            }
        }

        GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>().Stop();

        GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
        AudioSource.PlayClipAtPoint(gameOverSound, new Vector3(0, 0, 0), 0.1f);

        StartCoroutine(GameOver());        
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        gameOverMenu.isActive = true;
    }
    #endregion
    #region Scoring
    public void AddScore(int newScore, bool levelMultiplier = true) 
    {
        float tempScore = newScore;
        if (GetScoreMultiplier() > 0)
            tempScore = tempScore * (1 + GetScoreMultiplier());
        if (levelMultiplier)
            tempScore = tempScore * level;        
        score += tempScore;
    }

    public void SetScoreMultiplier(float mult, float duration) {
        scoreMultiplier = scoreMultiplier + mult;
        //if (duration > scoreMultiplierTimer)
            //scoreMultiplierTimer = duration;
        if (GetScoreMultiplier() > 0)
            backgroundAnimated.SetActive(true);
        if (GetScoreMultiplier() > highestScoreMultiplier)
            highestScoreMultiplier = GetScoreMultiplier();
    }

    public float GetScoreMultiplier()
    {
        scoreMultiplier = Mathf.Floor(scoreMultiplier * 10) / 10;
        return scoreMultiplier;
    }

    public static void scoreSolvedRow(int y)
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
        gm.AddScore(50 * (y + 1));
        gm.SetScoreMultiplier(0.2f * (y + 1), 2f);

        // Linesweep: Row was solved before the next tetromino was placed
        if (containsPreviousTetromino)
        {
            gm.AddScore(50 * (y + 1));
            gm.SetScoreMultiplier(0.2f * (y + 1), 2f);
            if (y > gm.safeEdgeTilesGained - 1)
            {
                //Debug.Log("Linesweep " + y);
                gm.linesweepsCleared += 1;
                gm.AddSafeTileToEdges();
            }
        }        
        
        gm.currentMines -= minesFlagged;
        gm.currentFlags -= minesFlagged;
        gm.minesSweeped += minesFlagged;
        //return rowScore;
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
                    gm.SetScoreMultiplier(0.3f, 5f);
                    break;
                case 2:
                    clipToPlay = gm.lineFullSound2;
                    gm.AddScore(300);
                    gm.SetScoreMultiplier(0.5f, 5f);
                    break;
                case 3:
                    clipToPlay = gm.lineFullSound3;
                    gm.AddScore(500);
                    gm.SetScoreMultiplier(0.8f, 5f);
                    break;
                default:
                    clipToPlay = gm.lineFullSound4;
                    int actionScore = 800;
                    if (gm.lastFillWasDifficult)
                        gm.AddScore(Mathf.RoundToInt(actionScore * 1.5f));
                    else
                        gm.AddScore(actionScore);
                    gm.SetScoreMultiplier(1f, 5f);
                    break;
            }
            // C-c-c-Combo!
            if (gm.comboLinesFilled > 0)
                gm.AddScore(50 * gm.comboLinesFilled);

            gm.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(clipToPlay, new Vector3(0, 0, 0));

            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);

            Debug.Log("Tetris rows full: " + fullRows);
            
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
    public void Quit()
    {
        Application.Quit();
        hasQuit = true;    
    }
    public void Pause(bool pause) 
    {
        if (isGameOver)
            return;
        if (pause && !canPause)
            return;

        if (pause)
        {
            Time.timeScale = 0;
            isPaused = true;
            pauseMenu.isActive = true;
            canPause = false;
        }
        else
        {
            if (!isGameOver)
            {
                Time.timeScale = 1;
                isPaused = false;       
                pauseMenu.isActive = false;
                StartCoroutine(ResetPause());
            }
        }        
    }
    IEnumerator ResetPause()
    {
        yield return new WaitForSeconds(0.4f);
        canPause = true;
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
