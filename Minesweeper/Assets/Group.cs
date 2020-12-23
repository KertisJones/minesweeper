using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    GameManager gm;

    float fallSpeed = 0.5f;
    float lastFall = 0;

    float minePercent = 30;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        // Default position not valid? Then it's game over
        if (!isValidGridPos())
        {
            gm.EndGame();
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }

        // Populate random mines in children
        foreach (Transform child in transform)
        {
            float randNum = Random.Range(1, 100);
            if (randNum <= minePercent)
            {
                child.gameObject.GetComponent<Tile>().isMine = true;
            }
        }

        
    }

    bool isValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = GameManager.roundVec2(child.position);

            // Not inside Border?
            if (!GameManager.insideBorder(v))
                return false;
            //Debug.Log(v);
            // Block in grid cell (and not part of same group)?
            if (GameManager.gameBoard[(int)v.x][(int)v.y] != null &&
                GameManager.gameBoard[(int)v.x][(int)v.y].transform.parent != transform)
                return false;
        }
        return true;
    }


    void updateGrid()
    {
        // Remove old children from grid
        for (int y = 0; y < GameManager.sizeY; ++y)
            for (int x = 0; x < GameManager.sizeX; ++x)
                if (GameManager.gameBoard[x][y] != null)
                    if (GameManager.gameBoard[x][y].transform.parent == transform)
                        GameManager.gameBoard[x][y] = null;

        // Add new children to grid
        foreach (Transform child in transform)
        {
            Vector2 v = GameManager.roundVec2(child.position);
            child.gameObject.GetComponent<Tile>().coordX = (int)v.x;
            child.gameObject.GetComponent<Tile>().coordY = (int)v.y;
            GameManager.gameBoard[(int)v.x][(int)v.y] = child.gameObject;
        }
    }


    // Update is called once per frame
    void Update()
    {
        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Modify position
            transform.position += new Vector3(-1, 0, 0);

            // See if valid
            if (isValidGridPos())
                // It's valid. Update grid.
                updateGrid();
            else
                // It's not valid. revert.
                transform.position += new Vector3(1, 0, 0);
        }

        // Move Right
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Modify position
            transform.position += new Vector3(1, 0, 0);

            // See if valid
            if (isValidGridPos())
                // It's valid. Update grid.
                updateGrid();
            else
                // It's not valid. revert.
                transform.position += new Vector3(-1, 0, 0);
        }

        // Rotate
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Rotate(0, 0, -90);

            // See if valid
            if (isValidGridPos())
                // It's valid. Update grid.
                updateGrid();
            else
                // It's not valid. revert.
                transform.Rotate(0, 0, 90);
        }

        // Move Downwards and Fall
        else if (Input.GetKeyDown(KeyCode.DownArrow) ||
                 Time.time - lastFall >= fallSpeed)
        {
            // Modify position
            transform.position += new Vector3(0, -1, 0);

            // See if valid
            if (isValidGridPos())
            {
                // It's valid. Update grid.
                updateGrid();
            }
            else
            {
                // It's not valid. revert.
                transform.position += new Vector3(0, 1, 0);

                // Clear filled horizontal lines
                GameManager.deleteFullRows();

                // Spawn next Group
                FindObjectOfType<TetrominoSpawner>().spawnNext();

                // Disable script
                enabled = false;
            }

            lastFall = Time.time;
        }

    }
}
