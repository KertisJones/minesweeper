using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int sizeX = 10;
    public int sizeY = 10;
    public int numMines = 5;

    private bool minesPlaced = false;

    public GameObject[][] gameBoard;

    public GameObject tile;

    // Start is called before the first frame update
    void Start()
    {
        BuildGameBoard();
        //PopulateMines();

        //gameBoard = new GameObject[sizeX][sizeY];
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
    }

    void PopulateMines(int startX = -10, int startY = -10)
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
                if (gameBoard[randX][randY].GetComponent<Tile>().isMine == false)
                {
                    gameBoard[randX][randY].GetComponent<Tile>().isMine = true;
                    DetectProximity(randX, randY);
                    currentMines += 1;
                }
            }                        
        }

        minesPlaced = true;
    }

    void DetectProximity(int i, int j)
    {
        if (i > 0)
        {
            gameBoard[i - 1][j].GetComponent<Tile>().nearbyMines += 1;
            if (j > 0)
                gameBoard[i - 1][j - 1].GetComponent<Tile>().nearbyMines += 1;
            if (j < sizeY - 1)
                gameBoard[i - 1][j + 1].GetComponent<Tile>().nearbyMines += 1;

        }
        if (i < sizeX - 1)
        {
            gameBoard[i + 1][j].GetComponent<Tile>().nearbyMines += 1;
            if (j > 0)
                gameBoard[i + 1][j - 1].GetComponent<Tile>().nearbyMines += 1;
            if (j < sizeY - 1)
                gameBoard[i + 1][j + 1].GetComponent<Tile>().nearbyMines += 1;
        }
        if (j > 0)
            gameBoard[i][j - 1].GetComponent<Tile>().nearbyMines += 1;
        if (j < sizeY - 1)
            gameBoard[i][j + 1].GetComponent<Tile>().nearbyMines += 1;
    }

    public void RevealTile(int x, int y, int nearbyMines, bool isMine)
    {
        if (!minesPlaced)
            PopulateMines(x, y);

        if (isMine)
            EndGame();
        else if (nearbyMines == 0)
        {
            if (x > 0)
            {
                gameBoard[x - 1][y].GetComponent<Tile>().Reveal();
                if (y > 0)
                    gameBoard[x - 1][y - 1].GetComponent<Tile>().Reveal();
                if (y < sizeY - 1)
                    gameBoard[x - 1][y + 1].GetComponent<Tile>().Reveal();

            }
            if (x < sizeX - 1)
            {
                gameBoard[x + 1][y].GetComponent<Tile>().Reveal();
                if (y > 0)
                    gameBoard[x + 1][y - 1].GetComponent<Tile>().Reveal();
                if (y < sizeY - 1)
                    gameBoard[x + 1][y + 1].GetComponent<Tile>().Reveal();
            }
            if (y > 0)
                gameBoard[x][y - 1].GetComponent<Tile>().Reveal();
            if (y < sizeY - 1)
                gameBoard[x][y + 1].GetComponent<Tile>().Reveal();
        }
    }

    void EndGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
