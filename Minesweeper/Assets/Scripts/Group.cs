using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    GameManager gm;

    float fallSpeed = 0.5f;
    float lastFall = 0;
    float lastMove = 0;

    public float minePercent = 30;

    float screenShakeDuration = 0.1f;
    float screenShakeStrength = 0.4f;

    public bool isDisplay = false;

    public Vector3 pivotPoint = new Vector3(0, 0, 0);

    public AudioClip moveSound;
    public AudioClip downSound;
    public AudioClip turnSound;
    public AudioClip landSound;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        // Default position not valid? Then it's game over
        if (!isValidGridPos() && !isDisplay)
        {
            gm.EndGame();
            //Debug.Log("GAME OVER");
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
        if (gm.isGameOver)
            return;
        if (gm.isPaused)
            return;
        if (isDisplay)
            return;        
        
        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || (Input.GetAxis("Horizontal") == -1 && Time.time - lastMove >= fallSpeed / 10))
            Move(-1);
        // Move Right
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || (Input.GetAxis("Horizontal") == 1 && Time.time - lastMove >= fallSpeed / 10))
            Move(1);

        // Rotate
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            //transform.Rotate(0, 0, -90);
            transform.RotateAround(transform.TransformPoint(pivotPoint), new Vector3(0, 0, 1), -90);


            // See if valid
            if (isValidGridPos())
            {
                // It's valid. Update grid.
                updateGrid();

                GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                AudioSource.PlayClipAtPoint(turnSound, new Vector3(0, 0, 0));
            }
            else
            {
                // It's not valid. revert.
                transform.Rotate(0, 0, 90);
            }
        }
        
        

        // Move Downwards and Fall
        else if (Time.time - lastFall >= fallSpeed || (Input.GetAxis("Vertical") == -1 && Time.time - lastFall >= fallSpeed / 10) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            // Modify position
            transform.position += new Vector3(0, -1, 0);

            // See if valid
            if (isValidGridPos())
            {
                // It's valid. Update grid.
                updateGrid();

                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                {
                    GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                    AudioSource.PlayClipAtPoint(downSound, new Vector3(0, 0, 0));
                }
                // Detect the moment it lands
                transform.position += new Vector3(0, -1, 0);
                if (!isValidGridPos())
                {
                    GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                    AudioSource.PlayClipAtPoint(landSound, new Vector3(0, 0, 0));

                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);
                }
                transform.position += new Vector3(0, 1, 0);

            }
            else
            {
                // It's not valid. revert.
                transform.position += new Vector3(0, 1, 0);

                // Spawn next Group
                FindObjectOfType<TetrominoSpawner>().spawnNext();

                // Clear filled horizontal lines
                GameManager.deleteFullRows();

                // Failsafe in case block is off screen
                foreach (Transform child in transform)
                {
                    if (child.position.y >= 20)
                    {
                        gm.EndGame();
                        //Debug.Log("GAME OVER");
                        Destroy(this.gameObject);
                    }
                }      

                // Disable script
                enabled = false;
            }

            lastFall = Time.time;
        }

    }

    void Move (float dir = 1) // -1 is Left, 1 is Right
    {
        // Modify position
        transform.position += new Vector3(dir, 0, 0);

        // See if valid
        if (isValidGridPos())
        {
            // It's valid. Update grid.
            updateGrid();

            GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(moveSound, new Vector3(0, 0, 0));
        }
        else
        {
            // It's not valid. revert.
            transform.position += new Vector3(dir * -1, 0, 0);
        }
        lastMove = Time.time;
    }
}
