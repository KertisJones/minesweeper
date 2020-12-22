using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        gameBoard = new GameObject[sizeX][];
        for (int i = 0; i < sizeX; i++)
        {
            GameObject[] tileColumn = new GameObject[sizeY];

            for (int j = 0; j < sizeY; j++)
            {
                GameObject newTile =  Instantiate(tile, new Vector3(i, j, 0), new Quaternion(0,0,0,0), this.gameObject.transform) as GameObject;
                newTile.name = "Tile (" + i + ", " + j + ")";
                newTile.GetComponent<Tile>().coordX = i;
                newTile.GetComponent<Tile>().coordY = j;
            }

            gameBoard[i] = tileColumn;
        }

        //gameBoard = new GameObject[sizeX][sizeY];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
