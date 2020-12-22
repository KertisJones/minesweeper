using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int sizeX = 10;
    public int sizeY = 10;
    public int numMines = 5;


    public GameObject[][] gameBoard;

    public GameObject tile;

    // Start is called before the first frame update
    void Start()
    {
        BuildGameBoard();
        PopulateMines();

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

    void PopulateMines()
    {
        int currentMines = 0;

        while (currentMines < numMines)
        {
            int randX = Random.Range(0, sizeX - 1);
            int randY = Random.Range(0, sizeY - 1);

            Debug.Log(randX + ", " + randY);
            Debug.Log(gameBoard[randX].GetValue(randY));

            if (gameBoard[randX][randY].GetComponent<Tile>().isMine == false)
            {
                gameBoard[randX][randY].GetComponent<Tile>().isMine = true;

                currentMines += 1;
            }            
        }
    }

    
}
