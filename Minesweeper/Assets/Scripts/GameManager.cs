using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public static int sizeX = 10;
    public static int sizeY = 24;
    public static int numMines = 5;

    static float screenShakeDuration = 0.2f;
    static float screenShakeStrength = 1f;

    private bool minesPlaced = false;

    public static GameObject[][] gameBoard;
    GameObject[] leftBorderTiles;
    GameObject[] rightBorderTiles;
    public static GameObject[][] leftOuterBoard;
    public static GameObject[][] rightOuterBoard;


    public bool isGameOver = false;
    public bool isPaused = false;

    public GameObject tile;
    public GameObject tileGroup;

    public AudioClip lineClearSound;
    public AudioClip lineFullSound1;
    public AudioClip lineFullSound2;
    public AudioClip lineFullSound3;
    public AudioClip lineFullSound4;
    public AudioClip gameOverSound;

    [Range(0.0f, 1.0f)]
    public float audioVolume = 0.5f; 



    //private GameObject blankTile;

    // Start is called before the first frame update
    void Awake()
    {
        /*blankTile = Instantiate(new GameObject(), new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), this.gameObject.transform) as GameObject;        
        blankTile.AddComponent<Tile>();
        blankTile.GetComponent<Tile>().isDisplay = true;
        blankTile.name = "Blank Tile";*/

        BuildGameBoard();
        AudioListener.volume = audioVolume;
        //PopulateMines();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        for (int i = -1; i <= sizeX; i++)
        {
            //place display tiles at bottom
            GameObject newTile = Instantiate(tile, new Vector3(i, -1, 0), new Quaternion(0, 0, 0, 0), this.gameObject.transform) as GameObject;
            newTile.name = "Tile (" + i + ", " + -1 + ")";
            newTile.GetComponent<Tile>().coordX = i;
            newTile.GetComponent<Tile>().coordY = -1;
            newTile.GetComponent<Tile>().isRevealed = true;
            newTile.GetComponent<Tile>().isDisplay = true;
        }

        // Left and Right Tiles
        leftBorderTiles = new GameObject[sizeY];
        rightBorderTiles = new GameObject[sizeY];
        for (int i = 0; i < sizeY; i++)
        {
            //place display tile on left side
            GameObject newTile = Instantiate(tileGroup, new Vector3(-1, i, 0), new Quaternion(0, 0, 0, 0), this.gameObject.transform) as GameObject;
            newTile.name = "Tile Group (" + -1 + ", " + i + ")";
            newTile.GetComponentInChildren<Tile>().coordX = -1;
            newTile.GetComponentInChildren<Tile>().coordY = i;
            newTile.GetComponent<Group>().isDisplay = true;            
            newTile.GetComponent<Group>().minePercent = 1;
            leftBorderTiles[i] = newTile;

            //place display tile on right side
            newTile = Instantiate(tileGroup, new Vector3(sizeX, i, 0), new Quaternion(0, 0, 0, 0), this.gameObject.transform) as GameObject;
            newTile.name = "Tile Group (" + sizeX + ", " + i + ")";
            newTile.GetComponentInChildren<Tile>().coordX = sizeX;
            newTile.GetComponentInChildren<Tile>().coordY = i;
            newTile.GetComponent<Group>().isDisplay = true;            
            newTile.GetComponent<Group>().minePercent = 1;
            rightBorderTiles[i] = newTile;
        }

        // Place bombs evenly on Left and Right Tiles
        for (int i = 0; i < sizeY - 4; i+=4)
        {
            int randNumLeft = Random.Range(0, 8);
            int randNumRight = Random.Range(0, 8);
            if (i == 0)
            {
                randNumLeft = Random.Range(0, 5);
                randNumRight = Random.Range(0, 5);
            }

            if (i + randNumLeft < sizeY - 4)
                leftBorderTiles[i + randNumLeft].GetComponentInChildren<Tile>().isMine = true;
            if (i + randNumRight < sizeY - 4)
                rightBorderTiles[i + randNumRight].GetComponentInChildren<Tile>().isMine = true;
        }
    }

    /*void BuildMinesweeperOuterBoards()
    {
        int outerSize = 13;

        leftOuterBoard = new GameObject[outerSize][];
        rightOuterBoard = new GameObject[outerSize][];
        for (int i = 0; i < outerSize; i++)
        {
            GameObject[] leftTileColumn = new GameObject[sizeY];
            GameObject[] rightTileColumn = new GameObject[sizeY];

            for (int j = -1; j < sizeY; j++)
            {
                
                GameObject newTileLeft = Instantiate(tile, new Vector3(i, j, 0), new Quaternion(0, 0, 0, 0), this.gameObject.transform) as GameObject;
                newTileLeft.name = "Tile (" + i + ", " + j + ")";
                newTileLeft.GetComponent<Tile>().coordX = i;
                newTileLeft.GetComponent<Tile>().coordY = j;

                leftTileColumn[j] = newTileLeft;
            }

            leftOuterBoard[i] = leftTileColumn;
            rightOuterBoard[i] = rightTileColumn;
        }
    }*/

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
        else if (x == -1 && y >= 0 && y < sizeY)
        {
            return leftBorderTiles[y].GetComponentInChildren<Tile>();
        }
        else if (x == sizeX && y >= 0 && y < sizeY)
        {
            return rightBorderTiles[y].GetComponentInChildren<Tile>();
        }
        //Debug.Log("Failed to find tile " + x + ", " + y);
        return null;
    }

    public static Vector2 roundVec2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x),
                           Mathf.Round(v.y));
    }

    public static bool insideBorder(Vector2 pos)
    {
        return ((int)pos.x >= 0 &&
                (int)pos.x < sizeX &&
                (int)pos.y >= 0 &&
                (int)pos.y < sizeY);
    }

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
        for (int x = 0; x < sizeX; ++x)
        {
            if (gameBoard[x][y] != null)
            {
                // Move one towards bottom
                gameBoard[x][y - 1] = gameBoard[x][y];
                gameBoard[x][y] = null;

                // Update Block position
                gameBoard[x][y - 1].GetComponent<Tile>().coordY -= 1;
            }
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
        return true;
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

    public static int scoreSolvedRow(int y)
    {
        int rowScore = 0;
        for (int x = 0; x < sizeX; ++x)
            if (gameBoard[x][y] != null)
            {
                if (gameBoard[x][y].GetComponent<Tile>().isMine)
                    rowScore += 50;
                else
                    rowScore += gameBoard[x][y].GetComponent<Tile>().nearbyMines;
            }
        return rowScore;
    }

    public static void scoreFullRows(Transform tetronimo)
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (gm.isGameOver)
            return;

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
                    gm.score += 40;
                    break;
                case 2:
                    clipToPlay = gm.lineFullSound2;
                    gm.score += 100;
                    break;
                case 3:
                    clipToPlay = gm.lineFullSound3;
                    gm.score += 300;
                    break;
                default:
                    clipToPlay = gm.lineFullSound4;
                    gm.score += 1200;
                    break;
            }

            gm.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(clipToPlay, new Vector3(0, 0, 0));

            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);

            Debug.Log("Tetris rows full: " + fullRows);
            deleteFullRows();
        }        
    }

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
                    gm.score += scoreSolvedRow(y);
                    deleteRow(y);
                    decreaseRowsAbove(y + 1);
                    --y;

                    gm.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                    AudioSource.PlayClipAtPoint(gm.lineClearSound, new Vector3(0, 0, 0));

                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);
                }
            }
        }
    }

    public void EndGame()
    {
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

        StartCoroutine(ReloadScene());
        
    }

    IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(2.9f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
