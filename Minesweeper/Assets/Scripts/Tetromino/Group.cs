using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    GameManager gm;
    private InputManager inputManager;
    HoldTetromino holdTetromino;

    public enum TetrominoType // your custom enumeration
    {
        ITetromino,
        JTetromino,
        LTetromino,
        OTetromino,
        STetromino,
        TTetromino,
        ZTetromino
    };
    public TetrominoType tetrominoType;
    public int tilesInMino = 4;
    public bool isSetupTetromino = false;

    float fallSpeed = 0.8f;
    int basicFallDistance = 1;
    float lockDelay = 0.5f;
    float lastFall = 0;
    float lastMove = 0;
    float spawnTime = 0;
    // Locking
    public bool isLocking = false;
    public int lockResets = 0;
    public int lockResetMax = 15;
    public float lockDelayTimer = 0;

    public float minePercent = 10;

    /*float screenShakeDuration = 0.1f;
    float screenShakeStrength = 0.4f;*/

    public bool isDisplay = false;
    public bool isBonus = false;
    public bool isHeld = false;
    public bool isFalling = true;
    
    [HideInInspector]
    public int rowsFilled = 0;
    bool difficultSweepScored = false;
    int bottomHeight = 99999;
    int topHeight = -99999;    
    int bottomHeightLowest = 99999;
    bool isWallKickThisTick = false;
    bool isTspin = false;
    [HideInInspector]
    public int maximumFallDistance = 0;
    bool canHardDrop = false;
    // Input
    public float dasDelay = 0.25f;
    public float autoRepeatRate = 0.05f;
    public float dasCutDelay = 0.017f;
    public float softDropFactor = 12;
    [HideInInspector]
    public bool buttonLeftHeld = false;
    bool buttonLeftHeldSecondary = false;
    float lastLeftButtonDown = 0;
    [HideInInspector]
    public bool buttonRightHeld = false;
    bool buttonRightHeldSecondary = false;
    float lastRightButtonDown = 0;
    bool buttonSoftDropHeld = false;
    float lastSoftDropDown = 0;
    float lastDASCutDelay = 0;

    public int currentRotation = 0; // 0 = spawn state, 1 = counter-clockwise rotation from spawn, 2 = 2 successive rotations from spawn, 3 = clockwise rotation from spawn
    bool lastSuccessfulMovementWasRotation = false;
    int lastRotationDir = 0;

    // Aura
    int burningTiles = 0;
    int frozenTiles = 0;
    int wetTiles = 0;

    public Transform pivot;
    public Vector3 pivotStaticBackup = new Vector3();


    public AudioClip moveSound;
    public AudioClip downSound;
    public AudioClip turnSound;
    public AudioClip fallSound;
    public AudioClip landSound;
    public AudioClip lockSound;
    public AudioClip lockFinalSound;
    public AudioClip tetrisweepSound;
    public AudioClip tSpinSound;
    public AudioClip placedAboveBoardWarningSound;    
    public AudioClip[] burningIgnitionSounds;
    public AudioClip[] frozenWindSounds;

    void Awake()
    {
        inputManager = InputManager.Instance;
    }

    #region Input
    void OnEnable()
    {
        inputManager.leftPress.started += _ => PressLeft();
        inputManager.leftPress.canceled += _ => ReleaseLeft();
        inputManager.rightPress.started += _ => PressRight();
        inputManager.rightPress.canceled += _ => ReleaseRight();
        inputManager.rotateClockwisePress.started += _ => PressRotateClockwise();
        inputManager.rotateCounterClockwisePress.started += _ => PressRotateCounterClockwise();
        inputManager.softDropPress.started += _ => PressSoftDrop();
        inputManager.softDropPress.canceled += _ => ReleaseSoftDrop();
        inputManager.hardDroptPress.started += _ => PressHardDrop();
        GameManager.OnLineClearEvent += _ => OnLineClear();
    }
    void OnDisable()
    {
        inputManager.leftPress.started -= _ => PressLeft();
        inputManager.leftPress.canceled -= _ => ReleaseLeft();
        inputManager.rightPress.started -= _ => PressRight();
        inputManager.rightPress.canceled -= _ => ReleaseRight();
        inputManager.rotateClockwisePress.started -= _ => PressRotateClockwise();
        inputManager.rotateCounterClockwisePress.started -= _ => PressRotateCounterClockwise();
        inputManager.softDropPress.started -= _ => PressSoftDrop();
        inputManager.softDropPress.canceled -= _ => ReleaseSoftDrop();
        inputManager.hardDroptPress.started -= _ => PressHardDrop();
        GameManager.OnLineClearEvent -= _ => OnLineClear();
    }
    public void PressLeft()
    {
        // Don't go any further if this shouldn't be moved 
        if (gm == null)
            return;
        if (gm.isGameOver || gm.isPaused || isDisplay || isHeld || !isFalling)
            return;
        /*if (!buttonRightHeld) // Default to the first held down
            buttonLeftHeld = true;*/
        buttonLeftHeld = true;
        buttonLeftHeldSecondary = true;
        buttonRightHeld = false;
        lastLeftButtonDown = Time.unscaledTime;
        Move(-1);
    }
    void ReleaseLeft()
    {
        buttonLeftHeld = false;
        buttonLeftHeldSecondary = false;
        if (!buttonRightHeld && buttonRightHeldSecondary)
            buttonRightHeld = true;
    }
    public void PressRight()
    {
        // Don't go any further if this shouldn't be moved 
        if (gm == null)
            return;
        if (gm.isGameOver || gm.isPaused || isDisplay || isHeld || !isFalling)
            return;
        /*if (!buttonLeftHeld) // Default to the first held down
            buttonRightHeld = true;*/
        buttonRightHeld = true;
        buttonRightHeldSecondary = true;
        buttonLeftHeld = false;
        lastRightButtonDown = Time.unscaledTime;
        Move(1);
    }
    void ReleaseRight()
    {
        buttonRightHeld = false;
        buttonRightHeldSecondary = false;
        if (!buttonLeftHeld && buttonLeftHeldSecondary)
            buttonLeftHeld = true;
    }
    void PressRotateClockwise()
    {
        // Don't go any further if this shouldn't be moved 
        if (gm == null)
            return;
        if (gm.isGameOver || gm.isPaused || isDisplay || isHeld || !isFalling)
            return;

        Rotate(-1);
    }
    void PressRotateCounterClockwise()
    {
        // Don't go any further if this shouldn't be moved 
        if (gm == null)
            return;
        if (gm.isGameOver || gm.isPaused || isDisplay || isHeld || !isFalling)
            return;

        Rotate(1);
    }
    public void PressSoftDrop()
    {
        // Don't go any further if this shouldn't be moved 
        if (gm == null)
            return;
        if (gm.isGameOver || gm.isPaused || isDisplay || isHeld || !isFalling || !gm.isStarted || gm.isHitstopPaused)
            return;

        buttonSoftDropHeld = true;
        lastSoftDropDown = Time.unscaledTime;
        
        AudioSource landSource = gm.soundManager.PlayClip(downSound, 0.9f, true);
        float pitchChange = -0.2f + (0.4f * ((float)bottomHeight / (float)gm.sizeY));
        landSource.pitch += pitchChange;

        SoftDrop();
    }
    
    void ReleaseSoftDrop()
    {
        buttonSoftDropHeld = false;
    }
    void PressHardDrop()
    {
        // Don't go any further if this shouldn't be moved 
        if (gm == null)
            return;
        if (gm.isGameOver || gm.isPaused || isDisplay || isHeld || !isFalling || !gm.isStarted)
            return;
        
        if (canHardDrop) //((Input.GetKeyDown(KeyCode.Space)  || Input.GetKeyDown(KeyCode.Keypad8)) && lastFall > 0 || Input.GetKeyDown(KeyCode.Return)))
        {            
            if (maximumFallDistance > 0)
            {
                gm.AddScore(maximumFallDistance * 2, "", 0);
                gm.SetScoreMultiplier(2, 1f);
                gm.TriggerOnHardDropEvent();
            }
            Fall(maximumFallDistance, true);
        }
    }

    public void UpdateInputValues()
    {
        //Official Guidline Gravity Curve: Time = (0.8-((Level-1)*0.007))(Level-1)
        fallSpeed = Mathf.Pow(0.8f - ((gm.level - 1) * 0.007f), (gm.level - 1));
        if (burningTiles > 0)
            fallSpeed *= 0.5f;
        if (frozenTiles > 0)
            fallSpeed *= 1.5f;

        autoRepeatRate = PlayerPrefs.GetFloat("AutoRepeatRate", autoRepeatRate * 1000) / 1000;
        dasDelay = PlayerPrefs.GetFloat("DelayedAutoShift", dasDelay * 1000) / 1000;
        dasCutDelay = PlayerPrefs.GetFloat("DASCutDelay", dasCutDelay * 1000) / 1000;
        softDropFactor = PlayerPrefs.GetFloat("SoftDropFactor", softDropFactor);
        if (softDropFactor == 41)
            softDropFactor = Mathf.Infinity;

        basicFallDistance = gm.gameMods.basicFallDistance;

        lastDASCutDelay = Time.time;
        lastFall = Time.time;
        spawnTime = Time.time;
        //Debug.Log("ARR:" + autoRepeatRate + ", DAS:" + dasDelay + ", DCD:" + dasCutDelay + ", SDF:" + softDropFactor);
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        holdTetromino = GameObject.FindGameObjectWithTag("Hold").GetComponent<HoldTetromino>();

        UpdateInputValues();        

        //if (gameMode.dynamicTime) // TODO
        //  lockDelay = 0.1f + (fallSpeed / 2);
        /*if (lockDelayBase < fallSpeed)
            lockDelay = fallSpeed;
        else
            lockDelay = lockDelayBase;*/

        if (isSetupTetromino)
        {
            isFalling = false;
            UpdateGrid();
        }   

        // Default position not valid? Then it's game over
        if (!isValidGridPos() && !isDisplay)
        {
            // Uh oh, the game's over. Check if the player could be saved by hard clearing solved rows.
            ClearRows(false);

            if (!isValidGridPos() && !isDisplay)
            {
                gm.EndGame();
            }
        }

        Tile.AuraType spawnAura = RandomSpawnAura();

        if (gm.gameMods.floorAndWallAura == Tile.AuraType.frozen && gm.gameMods.auraBurningWeight > 0 && gm.piecesPlaced == 0)
            spawnAura = Tile.AuraType.burning;

        if (spawnAura != Tile.AuraType.normal && spawnAura != Tile.AuraType.infected)
        {
            foreach (Tile child in GetChildTiles())
            {
                child.aura = spawnAura;
            }
        }
        else if (spawnAura == Tile.AuraType.infected)
        {
            List<Tile> children = GetChildTiles();
            children[Random.Range(0, children.Count)].aura = spawnAura;
        }

        if (!isHeld)
            LayMines();                 
    }

    public Tile.AuraType RandomSpawnAura()
    {
        int totalWeight = gm.gameMods.auraNormalWeight +
            gm.gameMods.auraBurningWeight +
            gm.gameMods.auraFrozenWeight +
            gm.gameMods.auraWetWeight +
            gm.gameMods.auraElectricWeight +
            gm.gameMods.auraPlantWeight +
            gm.gameMods.auraSandWeight +
            gm.gameMods.auraGlassWeight +
            gm.gameMods.auraInfectedWeight;

        int randomSelection = Random.Range(0, totalWeight);
        int weightCounter = 0;

        if (randomSelection < gm.gameMods.auraNormalWeight)
            return Tile.AuraType.normal;            
        weightCounter += gm.gameMods.auraNormalWeight;

        if (randomSelection < weightCounter + gm.gameMods.auraBurningWeight)
            return Tile.AuraType.burning;
        weightCounter += gm.gameMods.auraBurningWeight;

        if (randomSelection < weightCounter + gm.gameMods.auraFrozenWeight)
            return Tile.AuraType.frozen;
        weightCounter += gm.gameMods.auraFrozenWeight;

        if (randomSelection < weightCounter + gm.gameMods.auraWetWeight)
            return Tile.AuraType.wet;
        weightCounter += gm.gameMods.auraWetWeight;
        
        if (randomSelection < weightCounter + gm.gameMods.auraElectricWeight)
            return Tile.AuraType.electric;
        weightCounter += gm.gameMods.auraElectricWeight;

        if (randomSelection < weightCounter + gm.gameMods.auraPlantWeight)
            return Tile.AuraType.plant;
        weightCounter += gm.gameMods.auraPlantWeight;

        if (randomSelection < weightCounter + gm.gameMods.auraSandWeight)
            return Tile.AuraType.sand;
        weightCounter += gm.gameMods.auraSandWeight;

        if (randomSelection < weightCounter + gm.gameMods.auraGlassWeight)
            return Tile.AuraType.glass;
        weightCounter += gm.gameMods.auraGlassWeight;

        if (randomSelection < weightCounter + gm.gameMods.auraInfectedWeight)
            return Tile.AuraType.infected;
        weightCounter += gm.gameMods.auraInfectedWeight;

        Debug.Log("Aura out of bounds...");
        return Tile.AuraType.normal;
    }

    // Get a list of all child tiles, excluding those that will be destroyed at the end of this Update Cycle
    public List<Tile> GetChildTiles() {
        List<Tile> childTiles = new List<Tile>();
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<Tile>() != null)
                if (!child.gameObject.GetComponent<Tile>().isDestroyed)
                    childTiles.Add(child.GetComponent<Tile>());
        }
        return childTiles;
    }

    public void SpawnTetrominoOnBoard(bool firstSpawn)
    {
        List<Tile> childTiles = GetChildTiles();
        if (firstSpawn)
        {            
            foreach (Tile child in childTiles)
            {
                if (child.aura == Tile.AuraType.burning)
                    burningTiles++;
                else if (child.aura == Tile.AuraType.frozen)
                    frozenTiles++;
                else if(child.aura == Tile.AuraType.wet)
                    wetTiles++;
            }

            gm.numBurningTiles += burningTiles;
            gm.numFrozenTiles += frozenTiles;
            gm.numWetTiles += wetTiles;

            UpdateInputValues();
        }

        if (burningTiles > 0)
            gm.soundManager.PlayClip(burningIgnitionSounds[Random.Range(0, burningIgnitionSounds.Length)], 1, true);
        else if (frozenTiles > 0)
            gm.soundManager.PlayClip(frozenWindSounds[Random.Range(0, frozenWindSounds.Length)], 1, true);
        else if (wetTiles > 0)
            childTiles[0].PlaySoundBubble();
    }

    public void LayMines()
    {
        if (isDisplay && transform.position.y >= gm.sizeY - 4)
            return;

        List<Tile> childTiles = GetChildTiles();
        
        if (!isBonus)
        {
            // Populate random mines in children
            int numberOfMines = 0;
            foreach (Tile child in childTiles)
            {
                float randNum = Random.Range(1, 100);
                if (randNum <= minePercent && !child.isMine)
                {
                    child.isMine = true;
                    child.CountMine();
                    numberOfMines += 1;
                }
            }
            // I don't want big areas of nothing, so only spawn a 0-mine tile on a 'crit'
            if (numberOfMines == 0 && !isDisplay)
            {
                if (Random.Range(1,20) > 1) // 5% chance to still spawn with 0 mines
                {
                    Tile child = childTiles[Random.Range(0, 4)];
                    child.isMine = true;
                    child.CountMine();
                    numberOfMines += 1;
                }
            }
        }
        else // Bonus Tiles should be revealed
        {
            foreach (Tile child in childTiles) 
            {
                child.Reveal(true);//.isRevealed = true;
            }
        }
    }

    public bool isValidGridPos()
    {
        if (isHeld)
            return true;
        
        foreach (Tile child in GetChildTiles())
        {
            Vector2 v = GameManager.roundVec2(child.transform.position);

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


    public void UpdateGrid()
    {
        if (gm == null)
            gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        UpdateGridRemove();
        UpdateGridAdd();
        SetMaximumFallDistance();

        /*SetRandomSupporterName[] supporterTexts = GetComponentsInChildren<SetRandomSupporterName>();
        foreach (SetRandomSupporterName supporterText in supporterTexts)
        {
            supporterText.transform.rotation = Quaternion.identity;
        }     */   
    }

    public void UpdateGridRemove()
    {
        // Remove old children from grid
        for (int y = 0; y < gm.sizeY; ++y)
            for (int x = 0; x < gm.sizeX; ++x)
                if (GameManager.gameBoard[x][y] != null)
                    if (GameManager.gameBoard[x][y].transform.parent == transform)
                        GameManager.gameBoard[x][y] = null;
    }

    public void UpdateGridAdd()
    {        
        // Add new children to grid
        foreach (Tile child in GetChildTiles())
        {
            Vector2 v = GameManager.roundVec2(child.transform.position);
            child.coordX = (int)v.x;
            child.coordY = (int)v.y;
            if (!isHeld)
                GameManager.gameBoard[(int)v.x][(int)v.y] = child.gameObject;
            child.transform.rotation = Quaternion.identity;
        }    
    }

    void OnLineClear()
    {
        if (gm == null || !isFalling)
            return;
        
        // Update Speed if level has changed
        fallSpeed = Mathf.Pow(0.8f - ((gm.level - 1) * 0.007f), gm.level);
    }


    // Update is called once per frame
    void Update()
    {
        // Score tetrisweeps/tspinsweeps and delete this object if it's children tiles are gone.
        CheckForTetrisweeps();

        // Don't go any further if this shouldn't be moved 
        if (gm == null)
            return;

        if (gm.isGameOver 
            || gm.isPaused 
            || !gm.isStarted
            || isDisplay)
            return;

        if (isHeld)
        {
            bottomHeightLowest = 99999; // Reset the minimum bottom height for when this is spawned again
            return;
        }            
        if (!isFalling)
            return;
                    
        // DAS Move
        // Move Left
        if (buttonLeftHeld)
        {
            if (Time.unscaledTime - lastLeftButtonDown >= dasDelay && Time.time - lastDASCutDelay >= dasCutDelay && Time.time - lastMove >= autoRepeatRate)
            {
                Move(-1);            
                // ARR 0 moves to maximum distance instead of by framerate
                if (autoRepeatRate == 0)
                    while (WallKickMove(-1, 0, true, false)) {}
            }            
        }
            
        // Move Right
        if (buttonRightHeld)
        {
            if (Time.unscaledTime - lastRightButtonDown >= dasDelay && Time.time - lastDASCutDelay >= dasCutDelay && Time.time - lastMove >= autoRepeatRate)
            {
                Move(1);
                // ARR 0 moves to maximum distance instead of by framerate
                if (autoRepeatRate == 0)
                    while (WallKickMove(1, 0, true, false)) {}
            }
        }

        // Move Downwards and Fall
        // Soft Drop
        if (buttonSoftDropHeld && Time.unscaledTime - lastSoftDropDown >= dasDelay && Time.time - lastDASCutDelay >= dasCutDelay && Time.time - lastFall >= fallSpeed / softDropFactor)
            SoftDrop();

        if (Time.time - spawnTime >= 0.05f)
        {
            canHardDrop = true;
        }
            // Basic Fall
        if (Time.time - lastFall >= fallSpeed)// || isSoftDrop || isHardDrop)
        {
            canHardDrop = true;
            Fall(basicFallDistance);    
        }

        // Lock Delay
        if (isLocking)
        {            
            float lockPercentage = Mathf.Min(1f, Mathf.Max(0, lockDelayTimer / lockDelay));            

            if (CheckIfLockIsValid())
            {
                if (lockDelayTimer <= 0)
                {
                    LockTetromino();
                }
                else
                {
                    if (lockResets >= lockResetMax)
                        SetTileOverlayColor(new Color(1, 1, 1, Mathf.Max(0, 0.5f - (lockPercentage * 0.5f))));
                    else
                        SetTileOverlayColor(new Color(0, 0, 0, Mathf.Max(0, 0.3f - (lockPercentage * 0.3f))));
                }
            }
            else
            {
                LockDelayReset(true);
            }

            lockDelayTimer -= Time.deltaTime;
        }
    }

    public void SetTileOverlayColor(Color color)
    {
        foreach (Tile tile in GetChildTiles())
        {
            if (!tile.isRevealed)
                tile.fadeOverlay.color = color;
            else
                tile.fadeOverlay.color =  new Color(0, 0, 0, 0);
        }
    }

    public Color GetTileColor()
    {
        return GetComponentInChildren<UnityEngine.UI.Button>().image.color;
    }

    void SoftDrop()
    {
        if (gm == null)
            return;
        if (gm.isGameOver || gm.isPaused || isDisplay || isHeld || !isFalling || !gm.isStarted)
            return;

        int distToFall = basicFallDistance;
        if (softDropFactor == Mathf.Infinity)
            distToFall = Mathf.Max(maximumFallDistance, 1);

        if (maximumFallDistance > 0)
        {
            AudioSource fallSource = gm.soundManager.PlayClip(fallSound, 0.2f, true);
            float pitchChange = -0.4f + (0.8f * ((float)bottomHeight / (float)gm.sizeY));
            fallSource.pitch += pitchChange;
        }
        
        if (!isLocking && bottomHeight <= bottomHeightLowest)
            gm.AddScore(distToFall, "", 0);
        Fall(distToFall);
    }

    public void Fall(int fallDistance, bool isHardDrop = false, bool isManualFall = true)
    {
        if (!gm.isStarted || gm.isHitstopPaused)
            return;
        if (fallDistance > maximumFallDistance)
            fallDistance = Mathf.Max(maximumFallDistance, 1);
        //if (fallDistance == 0 && !isHardDrop)
            //LockTetrominoDelay();

        // Modify position
        transform.position += new Vector3(0, fallDistance * -1, 0);
        //Debug.Log(fallDistance + ", Maximum " + maximumFallDistance);
        // See if valid
        if (isValidGridPos())
        {
            if (fallDistance >= 1)
                lastSuccessfulMovementWasRotation = false;

            //gm.soundManager.PlayClip(fallSound, 0.25f, true);            

            isWallKickThisTick = false;

            if (isHardDrop)
            {
                UpdateGrid();
                LockTetromino(true);
            }
            else
            {
                //int oldbottomHeight = bottomHeight;
                if (isManualFall)
                    LockDelayReset(true);
                // It's valid. Update grid.
                UpdateGrid();

                // Step Reset for Lock Delay
                //LockDelayReset(true, oldbottomHeight);

                /*if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) //TODO
                {
                    GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                    AudioSource.PlayClipAtPoint(downSound, new Vector3(0, 0, 0), PlayerPrefs.GetFloat("SoundVolume", 0.5f));
                }          */      
            }

            // Detect the moment it lands
            DetectIfLanded(true);
        }
        else // Lock the piece in place
        {
            // It's not valid. revert.
            transform.position += new Vector3(0, 1, 0);
            LockTetrominoDelay();
        }

        if (canHardDrop)
            lastFall = Time.time;
        
    }

    void DetectIfLanded(bool rumble = false)
    {
        // Detect the moment it lands
        transform.position += new Vector3(0, -1, 0);

        if (!isValidGridPos())
        {
            LockTetrominoDelay();
            
            if (rumble)
            {
                if (burningTiles > 0)
                    GetChildTiles()[0].PlaySoundSteamHiss();
                else if (frozenTiles > 0)
                    GetChildTiles()[0].PlaySoundSnow();
                else if (wetTiles > 0)
                    GetChildTiles()[0].PlaySoundSplash();
                else
                {
                    AudioSource landSource = gm.soundManager.PlayClip(landSound, 1, true);
                    float pitchChange = -0.2f + (0.4f * ((float)bottomHeight / (float)gm.sizeY));
                    landSource.pitch += pitchChange;
                }
                    

                //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake(screenShakeDuration, screenShakeStrength);
                gm.TriggerOnTileSolveOrLandEvent();
            }
        }
        transform.position += new Vector3(0, 1, 0);
    }

    public void LockTetrominoDelay()
    {
        if (!isFalling)
            return;
        if (lockResets >= lockResetMax)
            LockTetromino();
        if (isLocking)
            return;
        
        lockDelayTimer = lockDelay;
        isLocking = true;
        
        /*yield return new WaitForSeconds(lockDelay);
        // Detect if next step will lock
        bool willLock = false;
        transform.position += new Vector3(0, -1, 0);
        if (!isValidGridPos())
        {
            willLock = true;
        }
        transform.position += new Vector3(0, 1, 0);

        // If it's at the bottom, lock it
        if (willLock)
            LockTetromino();*/
    }

    public void LockDelayReset(bool resetWithoutLimit = false)//, int bottomRow = 999999)
    {
        if (isLocking || resetWithoutLimit)
        {
            if (resetWithoutLimit) 
            {
                // This has fallen, so reset the timer without a limit
                lockDelayTimer = lockDelay;
                // If this is the farthest it has fallen, fully reset locking.
                //Debug.Log("Bottom Height: " + bottomHeight + ", lowest Row: " + bottomHeightLowest);
                if (bottomHeight <= bottomHeightLowest)
                {
                    //Debug.Log("Bottom Height: " + bottomHeight + ", lowest Row: " + bottomHeightLowest);
                    lockResets = 0;
                    isLocking = false;
                    SetTileOverlayColor(new Color(0, 0, 0, 0));
                }
                SetMaximumFallDistance();
            }
            else
            {
                if (lockResets < lockResetMax)
                {
                    lockDelayTimer = lockDelay;
                    lockResets++;
                }
            }
        }
    }

    
    public bool CheckIfLockIsValid()
    {
        // Detect if next step will lock
        bool willLock = false;
        transform.position += new Vector3(0, -1, 0);
        if (!isValidGridPos())
        {
            willLock = true;
        }
        transform.position += new Vector3(0, 1, 0);

        return willLock;
    }

    public void LockTetromino(bool isHardDrop = false)
    {
        if (!isFalling)
            return;
        
        // Allow the tetromino to be scored
        isFalling = false;
        isLocking = false;
        
        SetTileOverlayColor(new Color(0.1f, 0.1f, 0.1f, 0.1f));        

        // Score filled horizontal lines
        rowsFilled = GameManager.scoreFullRows(this.transform);
        
        switch (rowsFilled) {
            case 1:
                gm.singlesFilled++;
                break;
            case 2:
                gm.doublesFilled++;
                break;
            case 3:
                gm.triplesFilled++;
                break;
            case 4:
                gm.tetrisesFilled++;
                break;
            default:            
                break;
        }        

        // Will the next tetromino be safe?
        bool fillWasDifficult = (rowsFilled == 4);

        // Detect if an in-place spin has occured
        if (!WallKickMove(1, 0, false, false) && !WallKickMove(-1, 0, false, false) && !WallKickMove(0, 1, false, false))
        {
            //Debug.Log("In-Place spin locked! Rows filled: " + rowsFilled);
            gm.SetScoreMultiplier(5, 5);
        }

        // Detect if a T-Spin has occured
        if (tetrominoType == TetrominoType.TTetromino && lastSuccessfulMovementWasRotation)
        {
            // Three of the 4 squares diagonally adjacent to the T's center are occupied. The walls and floor surrounding the playfield are considered "occupied".
            int filledDiagonalTiles = 0;

            if (pivot.position.x == 0 || pivot.position.x == gm.sizeX - 1)
            {
                filledDiagonalTiles = 2;
            }
            if (gm.GetGameTile(Mathf.RoundToInt(pivot.position.x + 1), Mathf.RoundToInt(pivot.position.y + 1)) != null)
                filledDiagonalTiles++;
            if (gm.GetGameTile(Mathf.RoundToInt(pivot.position.x - 1), Mathf.RoundToInt(pivot.position.y - 1)) != null)
                filledDiagonalTiles++;
            if (gm.GetGameTile(Mathf.RoundToInt(pivot.position.x + 1), Mathf.RoundToInt(pivot.position.y - 1)) != null)
                filledDiagonalTiles++;
            if (gm.GetGameTile(Mathf.RoundToInt(pivot.position.x - 1), Mathf.RoundToInt(pivot.position.y + 1)) != null)
                filledDiagonalTiles++;
            
            if (filledDiagonalTiles >= 3) // It's a T-Spin!
            {
                isTspin = true;

                string scoreTranslationKey = "";

                if (rowsFilled == 0) // T-Spin no lines
                {
                    if (isWallKickThisTick) // T-Spin Mini no lines	
                    {
                        //Debug.Log("T-Spin Mini (No Lines)");
                        gm.tSpinMiniNoLines++;
                        gm.AddScore(100, "Scoring T-Spin Mini no lines", 0);
                    }
                    else // T-Spin no lines
                    {
                        //Debug.Log("T-Spin (No Lines)");
                        gm.tSpinNoLines++;
                        gm.AddScore(400, "Scoring T-Spin no lines", 0);                        
                    }
                    gm.SetScoreMultiplier(1, 5);
                }
                else if (rowsFilled == 1) // T-Spin Single
                {
                    int actionScore = 800;
                    if (isWallKickThisTick)
                    {
                        //Debug.Log("T-Spin Mini Single");
                        scoreTranslationKey = "Scoring T-Spin Mini Single";
                        gm.tSpinMiniSingle++;
                        actionScore = 200;
                    }
                    else
                    {
                        //Debug.Log("T-Spin Single");
                        scoreTranslationKey = "Scoring T-Spin Single";
                        gm.tSpinSingle++;
                    }
                    
                    if (gm.lastFillWasDifficult)
                    {
                        gm.AddScore(Mathf.RoundToInt(actionScore * 1.5f), scoreTranslationKey, 0, "Scoring Back-to-back");
                        gm.SetScoreMultiplier(10, 10);
                    }                        
                    else
                    {
                        gm.AddScore(actionScore, scoreTranslationKey, 0);
                        gm.SetScoreMultiplier(5, 10);
                    }                                          
                    fillWasDifficult = true;
                }
                else if (rowsFilled == 2) // T-Spin Double
                {
                    int actionScore = 1200;
                    if (isWallKickThisTick)
                    {
                        //Debug.Log("T-Spin Mini Double");
                        scoreTranslationKey = "Scoring T-Spin Mini Double";
                        gm.tSpinMiniDouble++;
                        actionScore = 400;
                    }
                    else
                    {
                        //Debug.Log("T-Spin Double");
                        scoreTranslationKey = "Scoring T-Spin Double";
                        gm.tSpinDouble++;
                    }

                    if (gm.lastFillWasDifficult)
                    {
                        gm.AddScore(Mathf.RoundToInt(actionScore * 1.5f), scoreTranslationKey, 0, "Scoring Back-to-back");
                        gm.SetScoreMultiplier(20, 10);
                    }                        
                    else
                    {
                        gm.AddScore(actionScore, scoreTranslationKey, 0);
                        gm.SetScoreMultiplier(10, 10);
                    }                    
                    fillWasDifficult = true;
                }
                else if (rowsFilled == 3) // T-Spin Triple
                {
                    //Debug.Log("T-Spin Triple");
                    scoreTranslationKey = "Scoring T-Spin Triple";
                    gm.tSpinTriple++;
                    int actionScore = 1600;
                    if (gm.lastFillWasDifficult)
                    {
                        gm.AddScore(Mathf.RoundToInt(actionScore * 1.5f), scoreTranslationKey, 0, "Scoring Back-to-back");
                        gm.SetScoreMultiplier(40, 10);
                    }                        
                    else
                    {
                        gm.AddScore(actionScore, scoreTranslationKey, 0);
                        gm.SetScoreMultiplier(20, 10);
                    }
                    fillWasDifficult = true;
                }

                gm.TriggerOnTSpinEvent(lastRotationDir * (rowsFilled + 1));

                if (rowsFilled > 0)
                {
                    //GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
                    gm.soundManager.PlayClip(tSpinSound, 1, true);
                }
            }            
        }

        int isTetrominoOffScreen = CheckIfTetrominoIsOffScreen(); // 0=On Screen, 1=Partially off screen, 2=Offscreen 
        // End Game if block is completely off screen
        if (isTetrominoOffScreen == 2)
        {
            // Uh oh, the game's over. Check if the player could be saved by hard clearing solved rows.
            ClearRows(false);            

            isTetrominoOffScreen = CheckIfTetrominoIsOffScreen(); // 0=On Screen, 1=Partially off screen, 2=Offscreen 
            // Check if the tetromino is still off screen. If so, the game is over.
            if (isTetrominoOffScreen == 2)
                gm.EndGame();
        }
        
        if (isTetrominoOffScreen == 0 && lockResets >= lockResetMax)
            gm.soundManager.PlayClip(lockFinalSound, 0.3f, true);
        if (isTetrominoOffScreen == 0 && !isHardDrop)
            gm.soundManager.PlayClip(lockSound, 0.3f, true);
        else if (isTetrominoOffScreen == 1)
            gm.soundManager.PlayClip(placedAboveBoardWarningSound, 0.6f, true);
            
        //GameManager.deleteFullRows();
        if (!gm.isGameOver)
        {
            // Spawn next Group; if playere scored a Tetris, spawn a fully revealed Tetronimo
            // Spawn the next mino *before* deleting rows, or else it will soft lock
            gm.tetrominoSpawner.SpawnNext(fillWasDifficult);

            // Combo Checks!
            if (rowsFilled > 0)
            {
                gm.lastFillWasDifficult = fillWasDifficult;
                gm.comboLinesFilled++;
            }
            else
                gm.comboLinesFilled = -1;            
            
            gm.piecesPlaced += 1;            
            gm.perfectClearThisRound = false;

            // Cleanse Recharge
            float adjustedTopHeight = (topHeight / 20f) * (gm.sizeY - 4);
            holdTetromino.AddToCleanseRecharge(Mathf.Max(1, Mathf.FloorToInt(adjustedTopHeight / 3f)));
            if (rowsFilled > 0)
                holdTetromino.AddToCleanseRecharge((rowsFilled + gm.comboLinesFilled) * 5);

            // Clear filled horizontal lines
            ClearRows();      
            
            // Set this as the previous tetromino
            gm.previousTetromino = this.gameObject;

            gm.TriggerOnMinoLockEvent();


            // Input DAS for next tetromino
            TransferDASToNewTetromino();                
        }
    }

    public void TransferDASToNewTetromino()
    {
        // Input DAS for next tetromino
        if (buttonLeftHeld)
        {
            gm.GetActiveTetromino().PressLeft();
            gm.GetActiveTetromino().lastLeftButtonDown = lastLeftButtonDown;
        }                
        if (buttonRightHeld)
        {
            gm.GetActiveTetromino().PressRight();
            gm.GetActiveTetromino().lastRightButtonDown = lastRightButtonDown;
            //lastRightButtonDown
        }
        if (buttonSoftDropHeld)
        {
            gm.GetActiveTetromino().buttonSoftDropHeld = buttonSoftDropHeld;
            gm.GetActiveTetromino().lastSoftDropDown = lastSoftDropDown;
        }
    }

    void ClearRows(bool getMultiplier = true)
    {
        // Clear filled horizontal lines
        int rowsCleared = GameManager.deleteFullRows(getMultiplier, rowsFilled, this.gameObject);
        if (rowsCleared > 0)
        {
            // Cascade this block down if a line cleared that would have allowed the mino to hard drop farther down.
            CascadeTetromino();
            
            // Clear filled horizontal lines again
            //rowsCleared = GameManager.deleteFullRows(getMultiplier);
            GameManager.markSolvedRows();

            // Update the fall distance for the new mino so ghost blocks don't get out of sync
            gm.tetrominoSpawner.currentTetromino.GetComponent<Group>().SetMaximumFallDistance();
        }
    }

    public void CascadeTetromino()
    {
        List<Tile> children = GetChildTiles();
        //Debug.Log(children.Count);
        if (children.Count < tilesInMino)
            return;
        //Debug.Log("Row Position before UpdateGrid(): " + transform.position.y + ", maximumFallDistance: " + maximumFallDistance);
        //UpdateGrid();
        SetMaximumFallDistance();
        //Debug.Log("Row Position after UpdateGrid(): " + transform.position.y + ", maximumFallDistance: " + maximumFallDistance);
        if (maximumFallDistance > 0)
        {
            //List<Vector2> childrenPositions = new List<Vector2>();
            foreach (Tile child in children)
            {
                int x = child.GetComponent<Tile>().coordX;
                int y = child.GetComponent<Tile>().coordY;

                // Move one towards bottom
                //GameManager.gameBoard[x][y - maximumFallDistance] = GameManager.gameBoard[x][y];
                GameManager.gameBoard[x][y] = null;

                // Update Block position
                //child.GetComponent<Tile>().coordY -= maximumFallDistance;
            }
            foreach (Tile child in children)
            {
                int x = child.GetComponent<Tile>().coordX;
                int y = child.GetComponent<Tile>().coordY;

                // Move one towards bottom
                GameManager.gameBoard[x][y - maximumFallDistance] = child.gameObject;

                // Update Block position
                child.GetComponent<Tile>().coordY -= maximumFallDistance;
            }
            
            /*Debug.Log("Current Row Position: " + transform.position.y + ", maximumFallDistance: " + maximumFallDistance);
            transform.position += new Vector3(0, maximumFallDistance * -1, 0);
            Debug.Log("New Row Position: " + transform.position.y);
            UpdateGrid();
            Debug.Log("Row Position after UpdateGrid(): " + transform.position.y);*/
            //Fall(maximumFallDistance, true);
        }        
    }

    // Failsafe in case block is completely off screen
    int CheckIfTetrominoIsOffScreen() // 0=On Screen, 1=Partially off screen, 2=Offscreen 
    {
        bool tetrominoIsOffScreen = true;
        bool tetrominoIsPartiallyOnScreen = false;
        foreach (Transform child in transform)
        {
            if (child.position.y < gm.sizeY - 4)
            {
                tetrominoIsOffScreen = false;
            }
            else
            {
                tetrominoIsPartiallyOnScreen = true;
            }
        }
        if (tetrominoIsOffScreen)
            return 2;
        if (tetrominoIsPartiallyOnScreen)
            return 1;
        return 0;
    }
    public void SetMaximumFallDistance()
    {
        int minFallDistance = 100;
        int newBottomHeight = 99999;
        int newTopHeight = -99999;

        foreach (Tile child in GetChildTiles())
        {
            int fallDistance = 0;
            int coordX = child.coordX;
            int coordY = child.coordY;
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
            
            if (newBottomHeight > coordY)
                newBottomHeight = coordY;
            if (newTopHeight < coordY)
                newTopHeight = coordY;
        }
        //Debug.Log("Final distance: " + maximumFallDistance);
        maximumFallDistance = minFallDistance;
        bottomHeight = newBottomHeight;
        if (bottomHeight < bottomHeightLowest)
            bottomHeightLowest = bottomHeight;
        topHeight = newTopHeight;
    }

    void Rotate (int dir = -1)
    {
        if (!gm.isStarted || gm.isHitstopPaused)
            return;

        Vector3 localPivot = pivotStaticBackup;
        if (pivot != null)
            localPivot = pivot.localPosition;
        
        transform.RotateAround(transform.TransformPoint(localPivot), new Vector3(0, 0, 1), 90 * dir);
        int previousRotation = currentRotation;
        currentRotation += dir;
        currentRotation = currentRotation % 4;
        if (currentRotation == -1)
            currentRotation = 3;
        
        bool lastSuccessfulMovementWasRotationTemp = lastSuccessfulMovementWasRotation;
        lastSuccessfulMovementWasRotation = true;

        int lastRotationDirTemp = lastRotationDir;
        lastRotationDir = dir;
        

        lastDASCutDelay = Time.time;

        // See if valid
        if (isValidGridPos())
        {
            // It's valid. Update grid.
            UpdateGrid();
            LockDelayReset();

            gm.soundManager.PlayClip(turnSound, 0.75f, true);
        }
        else
        {
            bool valid = false;
            // SRS Kick System: https://tetris.wiki/Super_Rotation_System
            // currentRotation key: 
            //     0 = spawn state
            //     1 = L - counter-clockwise rotation from spawn
            //     2 = 2 successive rotations from spawn
            //     3 = R - clockwise rotation from spawn

            if (tetrominoType == TetrominoType.ITetromino)
            {
                if (previousRotation == 0 && currentRotation == 3)
                {
                    // 0->R	( 0, 0)	(-2, 0)	(+1, 0)	(-2,-1)	(+1,+2)
                    valid = WallKickMove(-2, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(-2, -1);
                    if (valid)
                        return;
                    valid = WallKickMove(1, 2);
                    if (valid)
                        return;
                }
                else if (previousRotation == 3 && currentRotation == 0)
                {
                    // R->0	( 0, 0)	(+2, 0)	(-1, 0)	(+2,+1)	(-1,-2)
                    valid = WallKickMove(2, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(-1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(2, 1);
                    if (valid)
                        return;
                    valid = WallKickMove(-1, -2);
                    if (valid)
                        return;
                }
                else if (previousRotation == 3 && currentRotation == 2)
                {
                    // R->2	( 0, 0)	(-1, 0)	(+2, 0)	(-1,+2)	(+2,-1)
                    valid = WallKickMove(-1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(2, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(-1, 2);
                    if (valid)
                        return;
                    valid = WallKickMove(2, -1);
                    if (valid)
                        return;
                }
                else if (previousRotation == 2 && currentRotation == 3)
                {
                    // 2->R	( 0, 0)	(+1, 0)	(-2, 0)	(+1,-2)	(-2,+1)
                    valid = WallKickMove(1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(-2, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(1, -2);
                    if (valid)
                        return;
                    valid = WallKickMove(-2, 1);
                    if (valid)
                        return;
                }
                else if (previousRotation == 2 && currentRotation == 1)
                {
                    // 2->L	( 0, 0)	(+2, 0)	(-1, 0)	(+2,+1)	(-1,-2)
                    valid = WallKickMove(2, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(-1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(2, 1);
                    if (valid)
                        return;
                    valid = WallKickMove(-1, -2);
                    if (valid)
                        return;
                }
                else if (previousRotation == 1 && currentRotation == 2)
                {
                    // L->2	( 0, 0)	(-2, 0)	(+1, 0)	(-2,-1)	(+1,+2)
                    valid = WallKickMove(-2, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(-2, -1);
                    if (valid)
                        return;
                    valid = WallKickMove(1, 2);
                    if (valid)
                        return;
                }
                else if (previousRotation == 1 && currentRotation == 0)
                {
                    // L->0	( 0, 0)	(+1, 0)	(-2, 0)	(+1,-2)	(-2,+1)
                    valid = WallKickMove(1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(-2, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(1, -2);
                    if (valid)
                        return;
                    valid = WallKickMove(-2, 1);
                    if (valid)
                        return;
                }
                else if (previousRotation == 0 && currentRotation == 1)
                {
                    // 0->L	( 0, 0)	(-1, 0)	(+2, 0)	(-1,+2)	(+2,-1)
                    valid = WallKickMove(-1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(2, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(-1, 2);
                    if (valid)
                        return;
                    valid = WallKickMove(2, -1);
                    if (valid)
                        return;
                }
            }
            else // J, L, S, T, Z Tetrominoes; O shouldn't have made it this far
            {
                if (previousRotation == 0 && currentRotation == 3)
                {
                    // 0->R	( 0, 0)	(-1, 0)	(-1,+1)	( 0,-2)	(-1,-2)
                    valid = WallKickMove(-1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(-1, 1);
                    if (valid)
                        return;
                    valid = WallKickMove(0, -2);
                    if (valid)
                        return;
                    valid = WallKickMove(-1, -2);
                    if (valid)
                        return;
                }
                else if (previousRotation == 3 && currentRotation == 0)
                {
                    // R->0	( 0, 0)	(+1, 0)	(+1,-1)	( 0,+2)	(+1,+2)
                    valid = WallKickMove(1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(1, -1);
                    if (valid)
                        return;
                    valid = WallKickMove(0, 2);
                    if (valid)
                        return;
                    valid = WallKickMove(1, 2);
                    if (valid)
                        return;
                }
                else if (previousRotation == 3 && currentRotation == 2)
                {
                    // R->2	( 0, 0)	(+1, 0)	(+1,-1)	( 0,+2)	(+1,+2)
                    valid = WallKickMove(1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(1, -1);
                    if (valid)
                        return;
                    valid = WallKickMove(0, 2);
                    if (valid)
                        return;
                    valid = WallKickMove(1, 2);
                    if (valid)
                        return;
                }
                else if (previousRotation == 2 && currentRotation == 3)
                {
                    // 2->R	( 0, 0)	(-1, 0)	(-1,+1)	( 0,-2)	(-1,-2)
                    valid = WallKickMove(-1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(-1, 1);
                    if (valid)
                        return;
                    valid = WallKickMove(0, -2);
                    if (valid)
                        return;
                    valid = WallKickMove(-1, -2);
                    if (valid)
                        return;
                }
                else if (previousRotation == 2 && currentRotation == 1)
                {
                    // 2->L	( 0, 0)	(+1, 0)	(+1,+1)	( 0,-2)	(+1,-2)
                    valid = WallKickMove(1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(1, 1);
                    if (valid)
                        return;
                    valid = WallKickMove(0, -2);
                    if (valid)
                        return;
                    valid = WallKickMove(1, -2);
                    if (valid)
                        return;
                }
                else if (previousRotation == 1 && currentRotation == 2)
                {
                    // L->2	( 0, 0)	(-1, 0)	(-1,-1)	( 0,+2)	(-1,+2)
                    valid = WallKickMove(-1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(-1, -1);
                    if (valid)
                        return;
                    valid = WallKickMove(0, 2);
                    if (valid)
                        return;
                    valid = WallKickMove(-1, 2);
                    if (valid)
                        return;
                }
                else if (previousRotation == 1 && currentRotation == 0)
                {
                    // L->0	( 0, 0)	(-1, 0)	(-1,-1)	( 0,+2)	(-1,+2)
                    valid = WallKickMove(-1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(-1, -1);
                    if (valid)
                        return;
                    valid = WallKickMove(0, 2);
                    if (valid)
                        return;
                    valid = WallKickMove(-1, 2);
                    if (valid)
                        return;
                }
                else if (previousRotation == 0 && currentRotation == 1)
                {
                    // 0->L	( 0, 0)	(+1, 0)	(+1,+1)	( 0,-2)	(+1,-2)
                    valid = WallKickMove(1, 0);
                    if (valid)
                        return;
                    valid = WallKickMove(1, 1);
                    if (valid)
                        return;
                    valid = WallKickMove(0, -2);
                    if (valid)
                        return;
                    valid = WallKickMove(1, -2);
                    if (valid)
                        return;
                }
            }
            /*
            // Super-Wide Kick System:
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
                return;*/
            
            // Can't find a valid position - revert
            transform.RotateAround(transform.TransformPoint(localPivot), new Vector3(0, 0, 1), 90 * dir * -1);
            currentRotation = previousRotation;
            lastSuccessfulMovementWasRotation = lastSuccessfulMovementWasRotationTemp;
            lastRotationDir = lastRotationDirTemp;
            //Debug.Log("Rotation Wall Kicks failed");
        }
    }

    public bool WallKickMove(float dirH, float dirV = 0, bool setPos = true, bool playSound = true) // -1 is Left, 1 is Right
    {
        transform.position += new Vector3(dirH, dirV, 0);
        if (isValidGridPos())
        {
            // It's valid. Update grid if it's allowed to do so.
            if (setPos)
            {
                UpdateGrid();
                LockDelayReset();

                // Check if this kick causes the mino to land, to start lock timer.
                DetectIfLanded();
                
                isWallKickThisTick = true;

                if (playSound)
                {
                    gm.soundManager.PlayClip(turnSound, 0.75f, true);
                }                
            }
            else
            {
                // Reset position, but return true
                transform.position += new Vector3(dirH * -1, dirV * -1, 0);
            }
            SetMaximumFallDistance();
            //Debug.Log("Rotation Wall Kick (" + dirH + ", " + dirV + ")");
            return true;
        }
        else
        {
            // It's not valid. Revert back to center and revert rotation.
            transform.position += new Vector3(dirH * -1, dirV * -1, 0);
            SetMaximumFallDistance();
            return false;
        }
    }

    void Move(float dir = 1) // -1 is Left, 1 is Right
    {
        if (!gm.isStarted || gm.isHitstopPaused) 
            return;


        // Modify position
        transform.position += new Vector3(dir, 0, 0);

        // See if valid
        if (isValidGridPos())
        {
            // It's valid. Update grid.
            UpdateGrid();
            LockDelayReset();

            DetectIfLanded();

            AudioSource moveSource = gm.soundManager.PlayClip(moveSound, 0.75f, true);
            float posX = GetChildTiles()[0].coordX;
            float pitchChange = -0.2f + (0.4f * ((float)posX / (float)gm.sizeX));
            moveSource.pitch += pitchChange;

            lastSuccessfulMovementWasRotation = false;
        }
        else
        {
            // It's not valid. revert.
            transform.position += new Vector3(dir * -1, 0, 0);

            if (dir < 0)
                gm.TriggerOnLeftStuckEvent();
            else
                gm.TriggerOnRightStuckEvent();
        }
        lastMove = Time.time;
    }

    public (bool, bool) CheckForTetrisweeps(bool getMultiplier = true, float sweepMultiplier = 1, bool isInstantSweep = false, bool overrideIsValidPiece = false) //bool isInstantSweep = false, int highestRowSolved = -1)
    {
        if (isFalling || difficultSweepScored)
            return (false, false);            
        // The child object isn't destroyed until the next Update loop, so you should check for its destroyed tag.
        List<Tile> childrenTiles = GetChildTiles();

        bool isTetrisweep = false;
        bool isTspinsweep = false;

        int clearedTiles = 0;
            //(childrenTiles.Count == 0);
        foreach (Tile child in childrenTiles)
        {
            if (child.isMarkCleared)
                clearedTiles++;
        }

        // This tetromino has been fully cleared. Score points and delete this object.
        if (childrenTiles.Count == clearedTiles)
        {            
            // Detect if TETRISWEEP was achieved (4-row Tetris was solved with minesweeper before the next piece locks) 
            if (rowsFilled == 4 && (gm.previousTetromino == this.gameObject || gm.tetrominoSpawner.currentTetromino == this.gameObject || overrideIsValidPiece))
            {
                gm.tetrisweepsCleared += 1;
                isTetrisweep = true;
                difficultSweepScored = true;

                // Special challenge created by Random595! https://youtu.be/QR4j_RgvFsY

                if (getMultiplier)
                    gm.SetScoreMultiplier(20, 30);

                gm.soundManager.PlayClip(tetrisweepSound, 1, true);

                if (topHeight > gm.safeEdgeTilesGained - 1)
                    gm.AddSafeTileToEdges();                
            }
            // Clean up
            if (childrenTiles.Count == 0)
                Destroy(this.gameObject);
        }
        // Count as a T-spinsweep if 
        if (childrenTiles.Count < 4 || clearedTiles > 0)
        {
            if (isTspin && (gm.previousTetromino == this.gameObject || gm.tetrominoSpawner.currentTetromino == this.gameObject)) // Detect if T-Sweep was achieved
            {
                isTspinsweep = true;
                int fullTileCoordY = -100;
                foreach (Tile tile in childrenTiles)
                {
                    if (GameManager.isRowFull(tile.coordY))
                    {
                        isTspinsweep = false;
                        fullTileCoordY = tile.coordY;
                    }                        
                }
                if (isTspinsweep)
                {
                    AddTspinsweep(getMultiplier, sweepMultiplier, isInstantSweep);
                    difficultSweepScored = true;
                    /*gm.previousTetromino = null;
                    gm.tetrominoSpawner.currentTetromino = null;*/
                }                
            }
        }
        return (isTetrisweep, isTspinsweep);
    }

    void AddTspinsweep(bool getMultiplier, float sweepMultiplier = 1, bool isInstantSweep = false)
    {
        string scoreTranslationKeyPrefix1 = "";
        string scoreTranslationKeyPrefix2 = "";

        gm.tSpinsweepsCleared += 1;

        float actionScore = 595;
        actionScore *= sweepMultiplier;
        if (gm.previousClearWasDifficultSweep)
        {
            actionScore *= 1.5f;
            scoreTranslationKeyPrefix1 = "Scoring Back-to-back";
        }
        
        if (isInstantSweep)
            scoreTranslationKeyPrefix2 = "Scoring Instant";

        gm.AddScore((int)actionScore, "Scoring T-Spinsweep", 1, scoreTranslationKeyPrefix1, scoreTranslationKeyPrefix2); 
        
        if (getMultiplier)
            gm.SetScoreMultiplier(25, 30);

        gm.soundManager.PlayClip(tetrisweepSound, 1, true);

        if (topHeight > gm.safeEdgeTilesGained - 1)
            gm.AddSafeTileToEdges();
    }
}
