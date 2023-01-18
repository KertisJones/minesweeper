using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    GameManager gm;

    float fallSpeed = 0.8f;
    float lastFall = 0;
    float lastMove = 0;

    public float minePercent = 10;

    float screenShakeDuration = 0.1f;
    float screenShakeStrength = 0.4f;

    public bool isDisplay = false;
    public bool isBonus = false;
    public bool isHeld = false;
    public bool isFalling = true;
    [HideInInspector]
    public int rowsFilled = 0;
    [HideInInspector]
    public int maximumFallDistance = 0;
    bool canHardDrop = false;

    public Transform pivot;
    public Vector3 pivotStaticBackup = new Vector3();


    public AudioClip moveSound;
    public AudioClip downSound;
    public AudioClip turnSound;
    public AudioClip landSound;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        //Official Guidline Gravity Curve: Time = (0.8-((Level-1)*0.007))(Level-1)
        fallSpeed = Mathf.Pow(0.8f - ((gm.level - 1) * 0.007f), gm.level);

        // Default position not valid? Then it's game over
        if (!isValidGridPos() && !isDisplay)
        {
            gm.EndGame();
            //Debug.Log("GAME OVER");
            //Destroy(gameObject);
        }
        
        if (!isHeld)
            LayMines();
    }

    public void LayMines()
    {
        if (isDisplay && transform.position.y >= 20)
            return;
        
        if (!isBonus)
        {
            // Populate random mines in children
            int numberOfMines = 0;
            foreach (Transform child in transform)
            {
                if (child.gameObject.GetComponent<Tile>() != null)
                {
                    float randNum = Random.Range(1, 100);
                    if (randNum <= minePercent && !child.gameObject.GetComponent<Tile>().isMine)
                    {
                        child.gameObject.GetComponent<Tile>().isMine = true;
                        child.gameObject.GetComponent<Tile>().CountMine();
                        numberOfMines += 1;
                    }
                }
            }

            if (numberOfMines == 0 && !isDisplay)
            {
                if (Random.Range(1,20) > 1) // 5% chance to still spawn with 0 mines
                {
                    Tile child = this.transform.GetChild(Random.Range(0, 4)).GetComponent<Tile>();
                    child.isMine = true;
                    child.gameObject.GetComponent<Tile>().CountMine();
                    numberOfMines += 1;
                }
            }
        }
        else // Bonus Tiles should be revealed
        {
            foreach (Transform child in transform) 
            {
                if (child.gameObject.GetComponent<Tile>() != null)
                {
                    child.GetComponent<Tile>().Reveal();//.isRevealed = true;
                    child.GetComponent<Tile>().isDisplay = true;
                }
            }
        }
    }

    public bool isValidGridPos()
    {
        if (isHeld)
            return true;
        
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<Tile>() != null)
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
        }
        return true;
    }


    public void UpdateGrid()
    {
        UpdateGridRemove();
        UpdateGridAdd();
        SetMaximumFallDistance();
    }

    public void UpdateGridRemove()
    {
        // Remove old children from grid
        for (int y = 0; y < GameManager.sizeY; ++y)
            for (int x = 0; x < GameManager.sizeX; ++x)
                if (GameManager.gameBoard[x][y] != null)
                    if (GameManager.gameBoard[x][y].transform.parent == transform)
                        GameManager.gameBoard[x][y] = null;
    }

    public void UpdateGridAdd()
    {
        // Add new children to grid
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<Tile>() != null)
            {
                Vector2 v = GameManager.roundVec2(child.position);
                child.gameObject.GetComponent<Tile>().coordX = (int)v.x;
                child.gameObject.GetComponent<Tile>().coordY = (int)v.y;
                if (!isHeld)
                    GameManager.gameBoard[(int)v.x][(int)v.y] = child.gameObject;
            }
        }    
    }


    // Update is called once per frame
    void Update()
    {
        if (this.transform.childCount == 0)
        {
            // Detect if TETRISWEEP was achieved (4-row Tetris was solved with minesweeper before the next piece locks)
            if (rowsFilled == 4 && gm.previousTetromino == this.gameObject)
            {
                gm.tetrisweepsCleared += 1;
                gm.AddScore(595); // Special challenge created by Random595! https://youtu.be/QR4j_RgvFsY
                gm.SetScoreMultiplier(rowsFilled, 30);
            }
            // Clean up
            Destroy(this.gameObject);
        }
        if (gm.isGameOver)
            return;
        if (gm.isPaused)
            return;
        if (isDisplay)
            return;
        if (isHeld)
            return;
        if (!isFalling)
            return;
        //if (FindObjectOfType<TetrominoSpawner>().currentTetromino != this.gameObject)
            //return;
        
        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Keypad4) || (Input.GetAxis("Horizontal") == -1 && Time.time - lastMove >= fallSpeed / 10))
            Move(-1);
        // Move Right
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Keypad6) || (Input.GetAxis("Horizontal") == 1 && Time.time - lastMove >= fallSpeed / 10))
            Move(1);

        // Rotate
        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Keypad9)) // Rotate Clockwise
            Rotate(-1);
        else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Keypad7)) // Rotate Counterclockwise
            Rotate(1);
        
        //if (gm.isPaused)
            //return;

        // Move Downwards and Fall
        int fallDistance = 1;
        // Soft Drop
        bool isSoftDrop = (Input.GetAxis("Vertical") == -1 && Time.time - lastFall >= fallSpeed / 10) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Keypad2);
        // Hard Drop
        bool isHardDrop = false;
        if (canHardDrop && ((Input.GetKeyDown(KeyCode.Space)  || Input.GetKeyDown(KeyCode.Keypad8)) && lastFall > 0 || Input.GetKeyDown(KeyCode.Return)))
        {
            fallDistance = maximumFallDistance;
            isHardDrop = true;
            //maximumFallDistance;
            gm.SetScoreMultiplier(0.25f, 2f);
            gm.AddScore(maximumFallDistance * 2);
            //Fall(maximumFallDistance);
            //Fall();
            //lastFall = 0;
            /*while (isFalling)
            {
                gm.AddScore(2);
                Fall();
            } */           
        }
        // Soft Drop
        
        if (isSoftDrop)
        {
            if (isFalling)
                gm.AddScore(1);
        }
        // Basic Fall
        if (Time.time - lastFall >= fallSpeed || isSoftDrop || isHardDrop)
        {
            Fall(fallDistance, isHardDrop);
            canHardDrop = true;
        }
    }

    public void Fall(int fallDistance = 1, bool isHardDrop = false)
    {
        // Modify position
        transform.position += new Vector3(0, fallDistance * -1, 0);
        //Debug.Log(fallDistance + ", Maximum " + maximumFallDistance);
        // See if valid
        if (isValidGridPos())
        {
            // Detect the moment it lands
            transform.position += new Vector3(0, -1, 0);
            if (!isValidGridPos())
            {
                GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                AudioSource.PlayClipAtPoint(landSound, new Vector3(0, 0, 0));

                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);
            }
            transform.position += new Vector3(0, 1, 0);

            if (isHardDrop)
            {
                UpdateGrid();
                LockTetromino();
            }
            else
            {
                // It's valid. Update grid.
                UpdateGrid();

                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                {
                    GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                    AudioSource.PlayClipAtPoint(downSound, new Vector3(0, 0, 0));
                }                
            }
        }
        else // Lock the piece in place
        {
            // It's not valid. revert.
            transform.position += new Vector3(0, 1, 0);
            LockTetromino();
        }

        lastFall = Time.time;
    }

    public void LockTetromino()
    {
        // Allow the tetromino to be scored
        isFalling = false;

        // Set this as the previous tetromino
        gm.previousTetromino = this.gameObject;

        // Score filled horizontal lines
        rowsFilled = GameManager.scoreFullRows(this.transform);

        // Detect if an in-place spin has occured
        if (!WallKickMove(1, 0, false) && !WallKickMove(-1, 0, false) && !WallKickMove(0, 1, false))
        {
            Debug.Log("In-Place spin locked! Rows filled: " + rowsFilled);
            // Score += 100, doubled for each row filled
            if (rowsFilled == 0)
                gm.AddScore(100);
            else if (rowsFilled == 1)
                gm.AddScore(200);
            else if (rowsFilled == 2)
                gm.AddScore(400);
            else if (rowsFilled == 3)
                gm.AddScore(800);
            else if (rowsFilled == 4)
                gm.AddScore(1600);
        }

        // Failsafe in case block is off screen
        foreach (Transform child in transform)
        {
            if (child.position.y >= 20)
            {
                gm.EndGame();
            }
        }

        if (!gm.isGameOver)
        {
            // Spawn next Group; if playere scored a Tetris, spawn a fully revealed Tetronimo
            if (rowsFilled < 4)
                FindObjectOfType<TetrominoSpawner>().spawnNext();
            else
                FindObjectOfType<TetrominoSpawner>().spawnNext(true);

            // Clear filled horizontal lines
            GameManager.deleteFullRows();
        }
    }
    public void SetMaximumFallDistance()
    {
        /*int fallDistance = 0;
        while (isValidGridPos())
        {
            transform.position += new Vector3(0, -1, 0);
            fallDistance++;
        }
        fallDistance--;
        // Put the tetromino back into place
        transform.position += new Vector3(0, fallDistance, 0);
        maximumFallDistance = fallDistance;

        for (int i = 0; i < GameManager.sizeX; i++)
        {
            for (int j = 0; j < GameManager.sizeY; j++)
            {
                GameManager.gameBoard[i][j] = null; // blankTile;
            }
        }*/

        int minFallDistance = 100;
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<Tile>() != null)
            {
                int fallDistance = 0;
                int coordX = child.gameObject.GetComponent<Tile>().coordX;
                int coordY = child.gameObject.GetComponent<Tile>().coordY;
                for (int i = coordY; i >= 0; i--)
                {
                    // Not inside Border?
                    if (!GameManager.insideBorder(new Vector2(coordX, i)))
                        return;
                    // Block in grid cell (and not part of same group)?
                    if (GameManager.gameBoard[coordX][i] == null)
                        fallDistance++;
                    else if (!GameManager.gameBoard[coordX][i].transform.IsChildOf(transform))
                        i = -1;                                            
                }
                //Debug.Log(fallDistance);
                if (minFallDistance > fallDistance)
                    minFallDistance = fallDistance;
            }
        }
        //Debug.Log("Final distance: " + maximumFallDistance);
        maximumFallDistance = minFallDistance;
    }

    void Rotate (int dir = -1)
    {
        Vector3 localPivot = pivotStaticBackup;
        if (pivot != null)
            localPivot = pivot.localPosition;
        
        transform.RotateAround(transform.TransformPoint(localPivot), new Vector3(0, 0, 1), 90 * dir);

        // See if valid
        if (isValidGridPos())
        {
            // It's valid. Update grid.
            UpdateGrid();

            GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            AudioSource.PlayClipAtPoint(turnSound, new Vector3(0, 0, 0), 0.75f);
        }
        else
        {
            // It's not valid. Try Wall Kick to the Right
            bool valid = WallKickMove(1);
            if (valid)
                return;
            valid = WallKickMove(2);
            if (valid)
                return;
            // It's not valid. Try Wall Kick to the Left
            valid = WallKickMove(-1);
            if (valid)
                return;
            valid = WallKickMove(-2);
            if (valid)
                return;
            //Can't find a valid position
            transform.RotateAround(transform.TransformPoint(localPivot), new Vector3(0, 0, 1), 90 * dir * -1);
            Debug.Log("Rotation Wall Kicks failed");
        }
    }

    public bool WallKickMove(float dirH, float dirV = 0, bool setPos = true) // -1 is Left, 1 is Right
    {
        transform.position += new Vector3(dirH, dirV, 0);
        if (isValidGridPos())
        {
            // It's valid. Update grid if it's allowed to do so.
            if (setPos)
            {
                UpdateGrid();

                GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                AudioSource.PlayClipAtPoint(turnSound, new Vector3(0, 0, 0), 0.75f);
            }
            else
            {
                // Reset position, but return true
                transform.position += new Vector3(dirH * -1, dirV * -1, 0);
            }

            Debug.Log("Rotation Wall Kick (" + dirH + ", " + dirV + ")");
            return true;
        }
        else
        {
            // It's not valid. Revert back to center and revert rotation.
            transform.position += new Vector3(dirH * -1, dirV * -1, 0);
            return false;
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
            UpdateGrid();

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
