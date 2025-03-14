using System.Dynamic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.U2D;
using DG.Tweening;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.Localization.Settings;
using static GameManager;
using UnityEngine.Localization.Tables;
using GUPS.AntiCheat.Protected;
using Steamworks;
public class GameManager : MonoBehaviour
{
    private Camera mainCamera;
    [HideInInspector]
    public GameModifiers gameMods;
    [HideInInspector]
    public SoundManager soundManager;
    [HideInInspector]
    public TetrominoSpawner tetrominoSpawner;
    private DemoTitleScreen demoTitleScreen;
    ProtectedFloat startTime;
    ProtectedFloat endtime;
    [HideInInspector]
    public ProtectedFloat timeLimit = Mathf.Infinity;
    private ProtectedFloat score = 0;
    private float[] scoreByPointSourceType = new float[5];
    [SerializeField]
    public ProtectedInt32 scoreMultiplier = 0;
    public ProtectedInt32 scoreMultiplierLimit = 5000;
    public ProtectedFloat scoreMultiplierDecayPerTick = 0.1f;
    public ProtectedFloat scoreMultiplierTimer = 0f;
    private ProtectedInt32 scoreMultiplierTimerCountdown = -1;
    private ProtectedFloat scoreMultiplierAtDrain = 0;
    private ProtectedInt32 startupTimerCountdown = 1;
    private ProtectedFloat lastMultiplierTick = 0;
    public ProtectedInt32 comboLinesFilled = -1; // C=-1; +1 when mino locks & line filled; C= when mino locks & line not filled
    public bool lastFillWasDifficult = false; // Difficult fills are Tetrises or T-Spins
    public bool previousClearWasDifficultSweep = false;
    public bool perfectClearThisRound = true;
    public bool previousPCWasTetris = false;
    public ProtectedInt32 linesClearedTarget = 150;    
    public ProtectedInt32 linesCleared = 0;
    public ProtectedInt32 tetrisweepsCleared = 0;
    public ProtectedInt32 tSpinsweepsCleared = 0;
    public ProtectedInt32 revealCombo = 0;
    public ProtectedInt32 revealStreakManual = 0;
    //[HideInInspector]
    //public Tween revealComboDrainTween;    
    public ProtectedInt32 level = 1;
    public ProtectedInt32 currentMines = 0;
    public ProtectedInt32 currentFlags = 0;
    public ProtectedInt32 safeEdgeTilesGained = 0;
    public ProtectedInt32 sizeX = 10;
    public ProtectedInt32 sizeY = 24;
    public static ProtectedInt32 numMines = 5;

    //private bool minesPlaced = false;

    public static GameObject[][] gameBoard;
    List<GameObject> floorTiles;
    List<GameObject> leftBorderTiles;
    List<GameObject> rightBorderTiles;
    //public GameObject currentTetromino = null;
    public GameObject previousTetromino = null;
    
    public ProtectedBool isGameOver = false;
    public ProtectedBool isEndless = false;
    public ProtectedBool isPaused = false;
    public ProtectedBool isStarted = false;
    public ProtectedBool isTitleMenu = false;
    ProtectedBool canPause = true;
    public ProtectedBool hasQuit = false;
    public ProtectedBool lineClearInstantly = false;
    public ProtectedBool cheatGodMode = false;
    public ProtectedBool cheatAutoFlagMode = false;

    // Statistics
    public ProtectedInt32 piecesPlaced;
    public ProtectedInt32 holds;
    public ProtectedInt32 linesweepsCleared;
    public ProtectedFloat highestScoreMultiplier;
    public ProtectedInt32 minesSweeped;
    public ProtectedInt32 perfectClears;
    public ProtectedInt32 singlesFilled;
    public ProtectedInt32 doublesFilled;
    public ProtectedInt32 triplesFilled;
    public ProtectedInt32 tetrisesFilled;
    public ProtectedInt32 tSpinMiniNoLines;
    public ProtectedInt32 tSpinMiniSingle;
    public ProtectedInt32 tSpinMiniDouble;
    public ProtectedInt32 tSpinNoLines;
    public ProtectedInt32 tSpinSingle;
    public ProtectedInt32 tSpinDouble;
    public ProtectedInt32 tSpinTriple;

    public ProtectedFloat lastLineClearTime = 0;

    private bool isBestToday = false;
    private bool isHighScore = false;
    private bool isHighScoreEndless = false;
    private ProtectedFloat highScoreStarting;
    private ProtectedFloat bestTodayStarting;
    private ProtectedFloat highScoreEndlessStarting;

    // Game Mods
    [HideInInspector]
    public GameModifiers.MinesweeperTextType textType = GameModifiers.MinesweeperTextType.numbers;

    public ProtectedInt32 numBurningTiles = 0;
    public ProtectedInt32 numFrozenTiles = 0;
    public ProtectedInt32 numWetTiles = 0;

    [HideInInspector]
    public bool isHitstopPaused = false;

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
    public PieChart pieChart;
    public ProgressBar revealStreakManualProgressBar;
    public GameObject backgroundWallLeft;
    public GameObject backgroundWallRight;
    public GameObject backgroundFloor;
    public GameObject backgroundGrid;
    public GameObject guiCanvas;
    public GameObject framePreview;
    public GameObject frameHold;
    public GameObject floatingText;
    public FloatingTextQueue floatingTextQueue;

    public AudioClip lineClearSound;
    public AudioClip lineFullSound1;
    public AudioClip lineFullSound2;
    public AudioClip lineFullSound3;
    public AudioClip lineFullSound4;
    public AudioClip perfectClearSound;
    public AudioClip gameOverSound;
    public AudioClip multiplierCountdownSound;
    public AudioClip multiplierCountdownSoundFinal;
    public AudioClip startupCountdownSound;
    public AudioClip startupCountdownSoundFinal;
    public AudioClip startupCountdownSound2;
    public AudioClip startupCountdownSoundFinal2;
    public AudioClip revealStreakManual100Sound;
    public AudioClip[] fanfareSounds;

    public delegate void LineClearEvent(int lines);
    public static event LineClearEvent OnLineClearEvent;
    public delegate void HardDropEvent();
    public static event HardDropEvent OnHardDropEvent;
    public delegate void GameOverEvent();
    public static event GameOverEvent OnGameOverEvent;
    public delegate void LeftStuckEvent();
    public static event LeftStuckEvent OnLeftStuckEvent;
    public delegate void RightStuckEvent();
    public static event RightStuckEvent OnRightStuckEvent;
    public delegate void MinoLockEvent();
    public static event MinoLockEvent OnMinoLockEvent;
    public delegate void TileSolveEvent();
    public static event TileSolveEvent OnTileSolveOrLandEvent;
    public delegate void TSpinEvent(int dir);
    public static event TSpinEvent OnTSpinEvent;
    public delegate void ResetStartingPositionsEvent();
    public static event ResetStartingPositionsEvent OnResetStartingPositionsEvent;
    public delegate void KillTweenEvent();
    public static event KillTweenEvent OnKillTweenEvent;
    public delegate void NewPieceEvent();
    public static event NewPieceEvent OnNewPieceEvent;
    public delegate void MultiplierEvent();
    public static event MultiplierEvent OnMultiplierEvent;

    // Steamworks
    protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

    private void OnApplicationQuit() 
    {
        DOTween.KillAll();
    }

    #region Game Setup
    // Start is called before the first frame update
    void Awake()
    {
        DOTween.KillAll();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        inputManager = InputManager.Instance;
        soundManager = GetComponent<SoundManager>();
        gameMods = ScoreKeeper.Instance.GetComponent<GameModifiers>();
        tetrominoSpawner = GameObject.FindObjectOfType<TetrominoSpawner>();
        demoTitleScreen = GameObject.FindObjectOfType<DemoTitleScreen>();

        highScoreStarting = ScoreKeeper.Instance.bestScore;
        bestTodayStarting = ScoreKeeper.Instance.bestScoreToday;
        highScoreEndlessStarting = ScoreKeeper.Instance.bestScoreEndless;

        ScoreKeeper.Instance.runs += 1;

        textType = gameMods.minesweeperTextType;

        linesClearedTarget = gameMods.targetLines;
        timeLimit = gameMods.timeLimit;

        if (gameMods.lineClearTrigger == GameModifiers.LineClearTriggerType.clearInstantly)
            lineClearInstantly = true;
        else
            lineClearInstantly = false;

        sizeX = Mathf.Max((int)gameMods.boardSize.x, 4);
        sizeY = Mathf.Max((int)gameMods.boardSize.y + 4, 8);

        BuildGameBoard();        
        startTime = Time.time;
        if (gameMods.timeLimit == Mathf.Infinity && !gameMods.detailedTimer)
            isStarted = true;

        // Fixed Marathon: 10 per level
        while (linesCleared >= level * 10)
            level += 1;

        //PopulateMines();
    }

    public IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;
        //Debug.Log(LocalizationSettings.SelectedLocale);
    }

    #region Input
    void OnEnable()
    {
        inputManager.escapePress.started += _ => PressEscape();
        inputManager.restartPress.started += _ => PressRestart();
        inputManager.hardClearPress.started += _ => PressHardClear();

        if (SteamManager.Initialized)
        {
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
        }

    }
    void OnDisable()
    {
        inputManager.escapePress.started -= _ => PressEscape();
        inputManager.restartPress.started -= _ => PressRestart();
        inputManager.hardClearPress.started -= _ => PressHardClear();
    }

    void OnDestroy()
    {
        OnLineClearEvent = null;
        OnHardDropEvent = null;
        OnGameOverEvent = null;
        OnLeftStuckEvent = null;
        OnRightStuckEvent = null;
        OnMinoLockEvent = null;
        OnTileSolveOrLandEvent = null;
        OnTSpinEvent = null;
        OnResetStartingPositionsEvent = null;
        OnKillTweenEvent = null;
        OnNewPieceEvent = null;
        OnMultiplierEvent = null;
        transform.DOKill();
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
        //currentTetromino = null;
    }
    private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
    {
        if (pCallback.m_bActive != 0)
        {
            //Debug.Log("Steam Overlay has been activated");
            if (!isPaused && !isTitleMenu)
                Pause(true);
        }
        /*else
        {
            Debug.Log("Steam Overlay has been closed");
        }*/
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        if (!isStarted)
        {
            GetComponent<Light2D>().intensity = ((float)startupTimerCountdown / 4) * 0.6f;
            if (Time.time - startTime >= (float)startupTimerCountdown - 0.5f)
            {
                GameObject floater = Instantiate(floatingText, new Vector3((sizeX / 2f) - 0.5f, (sizeY - 4) / 2, 0), Quaternion.identity, guiCanvas.transform);
                //Debug.Log(startupTimerCountdown);
                switch (startupTimerCountdown)
                {                    
                    case 4:                        
                        floater.GetComponent<TextMeshProUGUI>().text = GetTranslation("UIText", "GUI Go");

                        soundManager.PlayClip(startupCountdownSoundFinal, 1, false);
                        soundManager.PlayClip(startupCountdownSoundFinal2, 1, false);

                        GetComponent<Light2D>().intensity = 1;
                        isStarted = true;
                        startTime = Time.time;
                        break;
                    default:
                        floater.GetComponent<TextMeshProUGUI>().text = (4 - startupTimerCountdown).ToString();

                        soundManager.PlayClip(startupCountdownSound, 1, false);
                        soundManager.PlayClip(startupCountdownSound2, 1, false);
                        break;
                }
                startupTimerCountdown++;
            }
            
        }
        


        //Debug.Log((Mathf.Floor(0.04f * 100) / 100) + 0.01f);
        if (scoreMultiplierTimer > 0 && !isGameOver)
            scoreMultiplierTimer -= Time.deltaTime;
        if (scoreMultiplierTimer > -1 && GetScoreMultiplier() > 0)
        {
            if (scoreMultiplierTimer <= (float)scoreMultiplierTimerCountdown) 
            {
                //Debug.Log(scoreMultiplierTimerCountdown);
                switch (scoreMultiplierTimerCountdown)
                {
                    case 0:
                        soundManager.PlayClip(multiplierCountdownSoundFinal, 0.5f, true);
                        break;
                    default:
                        soundManager.PlayClip(multiplierCountdownSound, 1, false);
                        break;                    
                }
                scoreMultiplierTimerCountdown--;
            }
        }
        if (scoreMultiplierTimer <= 0 && GetScoreMultiplier() > 0)
        {
            soundManager.PlayMultiplierDrain();

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

            SetMultiplierBackground();

            if (Time.time - lastMultiplierTick >= tickTime && !isGameOver)
            {
                if (GetScoreMultiplier() > 0)
                {
                    // Get the current level, but cap it at 30: endless mode should not be affected.
                    float tempLevel = level;
                    if (tempLevel > 30)
                        tempLevel = 30;
                    // Multiplier drain per tick will be half of the level
                    int multiplierDrain = Mathf.CeilToInt(tempLevel / 4);
                    // If the level is a multiple of 5, increase it by 1 so that the display doesn't flash between .5 and no decimal; the true cap on marathon games is 16 at level 29.
                    if (multiplierDrain % 5 == 0)
                        multiplierDrain += 1;
                    // Drain the multiplier...
                    SetScoreMultiplier(-1 * multiplierDrain, 0);
                    // If the multiplier is gone, turn off the background animation.
                    if (GetScoreMultiplier() <= 0)
                    {
                        backgroundAnimated.SetActive(false);
                        soundManager.StopMultiplierDrain();
                    }                        
                }
            }
            scoreMultiplierTimer = 0;
        }

        if (timeLimit < Mathf.Infinity)
            if (GetTime() >= timeLimit)
                Pause(true, true);
        
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
        for (int i = 0; i < sizeX; i++)
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
        if (gameMods.wallType == GameModifiers.WallType.playable)
        {
            for (int i = -1; i < sizeY; i++)
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
        else if (gameMods.wallType == GameModifiers.WallType.unlock)
        {
            safeEdgeTilesGained = -1;
            AddSafeTileToEdges();            
        }

        SetCameraScale();
        /*float backgroundHeight = sizeY + 1;
        if (cameraSize > cameraSizeYprefer)
            backgroundHeight = sizeY - 3;*/

        if (!isTitleMenu)
        {
            framePreview.transform.position = new Vector3(sizeX + 4, sizeY - 8);
            frameHold.transform.position = new Vector3(-10, sizeY - 8);
        }

        float backgroundHeight = sizeY - 3;
        backgroundWallRight.transform.position = new Vector3(sizeX - 0.5f, -1.5f, 1);

        backgroundWallLeft.GetComponent<SpriteRenderer>().size = new Vector2(1, backgroundHeight);
        backgroundWallRight.GetComponent<SpriteRenderer>().size = new Vector2(1, backgroundHeight);
        backgroundFloor.GetComponent<SpriteRenderer>().size = new Vector2(sizeX, 1);
        backgroundGrid.GetComponent<SpriteRenderer>().size = new Vector2(sizeX, sizeY);
        SetGridOpacity();
        backgroundAnimated.GetComponent<SpriteRenderer>().size = new Vector2(sizeX, backgroundHeight - 1); 
    }

    public void SetCameraScale(bool isStartup = true)
    {
        if (!isStartup && isTitleMenu)
            return;
        // Get the two points that need to be visible
        bool previewBuffer = (PlayerPrefs.GetInt("PreviewSpaceAboveBoardEnabled", 0) != 0);
        float guiBuffer = 12f;
        float borderBufferX = 0f;
        float borderBufferY = 0f;
        Vector3 bottomLeft = new Vector3(-1.5f - guiBuffer - borderBufferX, -1.5f - borderBufferY);
        Vector3 topRight = new Vector3(sizeX + 0.5f + guiBuffer + borderBufferX, sizeY - 4.5f + borderBufferY);
        if (previewBuffer)
            topRight.y += 4;
                
        Transform buttonsBoundTransform = null;
        if (isTitleMenu)
        {
            /*cameraSizeXprefer = (sizeX * 2 + 18) * 0.5f * ((float)mainCamera.pixelHeight / mainCamera.pixelWidth);

            cameraSizeXprefer = Mathf.Max(cameraSizeXprefer, 10.5f * ((float)mainCamera.pixelHeight / mainCamera.pixelWidth));
            cameraSizeYprefer = Mathf.Max(cameraSizeYprefer, 10.5f);*/
            buttonsBoundTransform = demoTitleScreen.topLeftBounds;

            bottomLeft = new Vector3(Mathf.Min((sizeX + 1.5f) * -1, buttonsBoundTransform.position.x), -1.5f);
            topRight = new Vector3(sizeX + 1.5f, Mathf.Max(sizeY - 4.5f, buttonsBoundTransform.position.y));
        }

        Vector3 centerPoint = (bottomLeft + topRight) / 2f;
        float distance = Vector3.Distance(bottomLeft, topRight);
        float horizontalDistance = Mathf.Abs(bottomLeft.x - topRight.x);
        float verticalDistance = Mathf.Abs(bottomLeft.y - topRight.y);
        //float greaterDistance = Mathf.Max(horizontalDistance, verticalDistance);


        float aspectRatio = mainCamera.aspect;
        float orthographicSize = verticalDistance / 2f;

        // Adjust orthographic size based on the aspect ratio
        if (horizontalDistance / aspectRatio > verticalDistance) // Landscape
        {
            orthographicSize = horizontalDistance / (2f * aspectRatio);
        }
        else // Portrait
        {
            orthographicSize = verticalDistance / 2f;
        }

        float sizeModifier = orthographicSize / mainCamera.orthographicSize;
        mainCamera.transform.position = new Vector3(centerPoint.x, centerPoint.y, mainCamera.transform.position.z);
        mainCamera.orthographicSize = orthographicSize;

        if (isTitleMenu)
        {
            Transform buttonsTransform = demoTitleScreen.gameObject.transform;
            buttonsTransform.localScale = new Vector3(sizeModifier, sizeModifier, sizeModifier);
            buttonsTransform.localPosition = new Vector3(bottomLeft.x + horizontalDistance / 4, mainCamera.ScreenToWorldPoint(new Vector3(0, (float)Screen.height / 2, 0)).y);
            demoTitleScreen.bouncyLogo.transform.position = new Vector3((sizeX / 2f) - 0.5f, topRight.y);
            demoTitleScreen.ResetStartingPositionsInChildren(sizeModifier);
        }

        /*float cameraSizeYprefer = ((sizeY - 4) / 2f) + 0.5f; //10.5f; Y Bounds
        float cameraSizeXprefer = (sizeX + 28) * 0.5f * ((float)mainCamera.pixelHeight / mainCamera.pixelWidth); //10f; X Bounds

        if (isTitleMenu)
        {
            cameraSizeXprefer = (sizeX * 2 + 18) * 0.5f * ((float)mainCamera.pixelHeight / mainCamera.pixelWidth);

            cameraSizeXprefer = Mathf.Max(cameraSizeXprefer, 10.5f * ((float)mainCamera.pixelHeight / mainCamera.pixelWidth));
            cameraSizeYprefer = Mathf.Max(cameraSizeYprefer, 10.5f);
        }

        float cameraSize = Mathf.Max(cameraSizeXprefer, cameraSizeYprefer);
        if (sizeX == 10 && sizeY == 24 && (Mathf.Floor(((float)mainCamera.pixelWidth / mainCamera.pixelHeight) * 100) / 100) == Mathf.Floor((16f / 9f) * 100) / 100)
            cameraSize = cameraSizeYprefer;
        //Debug.Log(((float)mainCamera.pixelWidth / mainCamera.pixelHeight) + ", " + (16f/9f));

        float cameraX = (sizeX / 2f) - 0.5f; //4.5f;
        float cameraY = ((sizeY - 4) / 2f) - 1f;//cameraSize - 1.5f; //9

        if (isTitleMenu)
        {
            // cameraSize = ((sizeY - 4) / 2f) + 0.5f;
            // cameraSize - .5f = (sizeY - 4) / 2f
            // (cameraSize - .5f) * 2 = sizeY - 4
            // ((cameraSize - .5f) * 2) + 4 = sizeY
            float yscaleModifier = (cameraSize - 0.5f);
            float sizeModifier = Mathf.Max((cameraSize / 10.5f), 1);

            //cameraX = (cameraSize / 4) - (sizeX * sizeModifier) / 4 - 1; // Mathf.Max(cameraX - sizeX - 1, -4.5f);
            //cameraX = (cameraSize / ((float)mainCamera.pixelHeight / mainCamera.pixelWidth)) / -4;
            float camWithCoord00AtBottomLeft = ((float)mainCamera.pixelWidth / mainCamera.pixelHeight) * cameraSize;
            cameraX = camWithCoord00AtBottomLeft - (sizeX + 1.5f) * 1.5f;// (camWithCoord00AtBottomLeft * -1) + sizeX * 1.5f;// - 1.5f;

            cameraY = yscaleModifier - sizeModifier;
            cameraY = Mathf.Max(cameraY, 9);

            Transform buttonsTransform = GameObject.FindObjectOfType<DemoTitleScreen>().gameObject.transform;
            //float sizeModifier = Mathf.Max(cameraX / (10.5f * ((float)mainCamera.pixelHeight / mainCamera.pixelWidth)),
                cameraY / (10.5f * ((float)mainCamera.pixelHeight / mainCamera.pixelWidth)),
                1);
            
            

            sizeModifier = Mathf.Max(yscaleModifier / 10.5f, 1);

            

            buttonsTransform.localScale = new Vector3(sizeModifier, sizeModifier, sizeModifier);
            //buttonsTransform.localPosition = new Vector3(Mathf.Min(-7.5f, sizeX / -4), buttonsTransform.localPosition.y);
            buttonsTransform.localPosition = new Vector3(mainCamera.ScreenToWorldPoint(new Vector3((float)Screen.width / 4, 0, 0)).x * sizeModifier, buttonsTransform.localPosition.y);
            // buttonsTransform.localPosition = new Vector3(mainCamera.ScreenToWorldPoint(new Vector3((float)mainCamera.pixelWidth / 4,0,0)).x, buttonsTransform.localPosition.y);//Mathf.Min(-7.5f, sizeX / -4)

        }

        mainCamera.transform.position = new Vector3(cameraX, cameraY, -10);
        mainCamera.orthographicSize = cameraSize;
        */

        float canvasHeight = (450 / 10.5f) * mainCamera.orthographicSize;
        if (guiCanvas != null)
        {
            guiCanvas.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);
            guiCanvas.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(canvasHeight * aspectRatio, canvasHeight);
        }               

        TriggerOnResetStartingPositionsEvent();
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
        public ArrayList GetNeighborTiles(int x, int y, bool getDiagonals = true)
    {
        ArrayList neighbors = new ArrayList();

        if (GetGameTile(x - 1, y) != null)
            neighbors.Add(GetGameTile(x - 1, y));        
        if (GetGameTile(x + 1, y) != null)
            neighbors.Add(GetGameTile(x + 1, y));        
        if (GetGameTile(x, y - 1) != null)
            neighbors.Add(GetGameTile(x, y - 1));
        if (GetGameTile(x, y + 1) != null)
            neighbors.Add(GetGameTile(x, y + 1));

        if (getDiagonals)
        {
            if (GetGameTile(x - 1, y - 1) != null)
                neighbors.Add(GetGameTile(x - 1, y - 1));
            if (GetGameTile(x - 1, y + 1) != null)
                neighbors.Add(GetGameTile(x - 1, y + 1));
            if (GetGameTile(x + 1, y - 1) != null)
                neighbors.Add(GetGameTile(x + 1, y - 1));
            if (GetGameTile(x + 1, y + 1) != null)
                neighbors.Add(GetGameTile(x + 1, y + 1));
        }
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
        else if (x == -1 && y >= -1 && y < leftBorderTiles.Count-1)
        {
            return leftBorderTiles[y+1].GetComponentInChildren<Tile>();
        }
        else if (x == sizeX && y >= -1 && y < rightBorderTiles.Count-1)
        {
            return rightBorderTiles[y+1].GetComponentInChildren<Tile>();
        }
        else if (x >= 0 && x < sizeX && y == -1)
        {
            return floorTiles[x].GetComponent<Tile>();
        }
        //Debug.Log("Failed to find tile " + x + ", " + y);
        return null;
    }   

    public static bool insideBorder(Vector2 pos)
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        return ((int)pos.x >= 0 &&
                (int)pos.x < gm.sizeX &&
                (int)pos.y >= 0 &&
                (int)pos.y < gm.sizeY);
    }
    #endregion
    #region Tetris Logic
    public static void deleteRow(int y)
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        for (int x = 0; x < gm.sizeX; ++x)
        {
            Tile tile = gameBoard[x][y].gameObject.GetComponent<Tile>();
            if (tile.aura == Tile.AuraType.frozen)
            {
                tile.PlaySoundIceBreak();
                tile.SetAura(Tile.AuraType.wet);
            }
            else
            {
                tile.isDestroyed = true;
                Destroy(gameBoard[x][y].gameObject);
                gameBoard[x][y] = null;
            }
        }
    }

    public static bool decreaseRow(int y) // returns true if the falling piece clips into the falling row
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        Group activeTetrominoInRow = null;
        bool currentTetrominoClipped = false;
        for (int x = 0; x < gm.sizeX; ++x)
        {
            if (gameBoard[x][y] != null)
            {
                if (gameBoard[x][y].GetComponentInParent<Group>().isFalling) // Active tetromino should not be moved directly, or it will cause a rotation error
                {
                    activeTetrominoInRow = gameBoard[x][y].GetComponentInParent<Group>();
                }
                else // All static tiles should be moved down
                {
                    bool dropDown = false;
                    // ...unless it has a tile there already (caused by ice or falling piece that didn't kick correctly)
                    if (gameBoard[x][y - 1] == null)
                    {
                        dropDown = true;
                    }
                    else if (gameBoard[x][y - 1].GetComponentInParent<Group>().isFalling) // Handle falling minos in the wrong space
                    {
                        Group activeMino = gameBoard[x][y - 1].GetComponentInParent<Group>();
                        int i = 1;
                        while (!activeMino.WallKickMove(0, i, true, false)) 
                        { 
                            i++;
                        }
                        currentTetrominoClipped = true;
                        dropDown = true;
                    }

                    if (dropDown)
                    {
                        // Move one towards bottom
                        gameBoard[x][y - 1] = gameBoard[x][y];
                        gameBoard[x][y] = null;

                        // Update Block position
                        gameBoard[x][y - 1].GetComponent<Tile>().coordY -= 1;
                    }
                    
                }                
            }
        }

        if (activeTetrominoInRow != null) // Move the active tetromino after the rest of the row has finished
        {
            // The active tetromino should not change position, unless it needs to fall in order for blocks above it to have a place to land
            activeTetrominoInRow.WallKickMove(0, 1); // Attempt to move the tetromino up. This will do nothing if the space is blocked
            activeTetrominoInRow.Fall(1, false, false); // Move the tetromino down. This will put it back in the same place if it was moved up, otherwise it will get the tetromino out of the way
        }

        return currentTetrominoClipped;
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
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        bool currentTetrominoClipped = false;
        for (int i = y; i < gm.sizeY; ++i)
            currentTetrominoClipped = currentTetrominoClipped || decreaseRow(i);

        if (currentTetrominoClipped)
            gm.tetrominoSpawner.currentTetromino.GetComponent<Group>().Fall(1, false, false);
    }

    public static bool isRowFull(int y)
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        for (int x = 0; x < gm.sizeX; ++x)
            if (gameBoard[x][y] == null)
                return false;
            else if (gameBoard[x][y].GetComponentInParent<Group>().isFalling)
                return false;
        return true;
    }

    public static bool isRowEmpty(int y)
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        for (int x = 0; x < gm.sizeX; ++x)
            if (gameBoard[x][y] != null)
                if (!gameBoard[x][y].GetComponentInParent<Group>().isFalling)
                    return false;
        return true;
    }

    // Checks whether a perfect clear is possible if all currently solved lines were cleared, then clears those lines if it would create a PC.
    public static void CheckForPossiblePerfectClear()
    {        
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        for (int y = 0; y < gm.sizeY; ++y)
        {
            if (!isRowEmpty(y))
                if (!isRowSolved(y))
                    return;
        }
        int rowsFilled = 0;
        if (gm.previousTetromino != null)
            rowsFilled = gm.previousTetromino.GetComponent<Group>().rowsFilled;
        deleteFullRows(true, rowsFilled);
    }

    public void CheckForPerfectClear(int rowsCleared, int rowsFilled, bool isInstantPC)
    {
        //Debug.Log(rowsCleared + ", " + rowsFilled + ", " + isInstantPC);
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
        string scoreTranslationKey = "";
        string scoreTranslationKeyPrefix1 = "";
        string scoreTranslationKeyPrefix2 = "";
        // Give rewards for a perfect clear!
        if (rowsActuallyFilled == 0 || rowsActuallyFilled == 1)
        {
            pcScore = 800 + (100 * (rowsCleared - 1));        
            SetScoreMultiplier(10, 30);
            previousPCWasTetris = false;
            scoreTranslationKey = "Scoring Perfect clear 1";
        }
        else if (rowsActuallyFilled == 2)
        {
            pcScore = 1200 + (150 * (rowsCleared - 2));        
            SetScoreMultiplier(25, 30);
            previousPCWasTetris = false;
            scoreTranslationKey = "Scoring Perfect clear 2";
        }
        else if (rowsActuallyFilled == 3)
        {
            pcScore = 1800 + (200 * (rowsCleared - 3));        
            SetScoreMultiplier(50, 30);
            previousPCWasTetris = false;
            scoreTranslationKey = "Scoring Perfect clear 3";
        }
        else if (rowsActuallyFilled >= 4)
        {
            pcScore = 500 * rowsCleared;
            scoreTranslationKey = "Scoring Perfect clear 4";
            if (previousPCWasTetris)
            {
                pcScore = pcScore * 1.6f;
                scoreTranslationKeyPrefix1 = "Scoring Back-to-back";
            }                
            SetScoreMultiplier(100, 30);
            previousPCWasTetris = true;
        }

        if (isInstantPC)
        {
            AddSafeTileToEdges();
            pcScore = pcScore * 1.5f;
            scoreTranslationKeyPrefix2 = "Scoring Instant";
        }

        AddScore(((int)pcScore), scoreTranslationKey, 0, scoreTranslationKeyPrefix1, scoreTranslationKeyPrefix2);

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

        soundManager.PlayClip(perfectClearSound, 1, true);
    }
    #endregion
    #region Tetrisweeper Solved Logic
    public static int deleteFullRows(bool getMultiplier = true, int rowsFilled = 0, GameObject tetrominoThatJustLocked = null) //, bool isTriggeredByLock = false)
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (gm.isGameOver || gm.isHitstopPaused)
            return 0;
        
        int rowsCleared = 0;
        int delayedLinesweepsCleared = 0;
        int instantLinesweepsCleared = 0;
        int highestRowSolved = -1;

        float burningMultiplier = 1;
        bool containsFrozenTile = false;

        // Score all of the solved rows
        for (int y = 0; y < gm.sizeY; ++y)
        {
            if (isRowSolved(y))
            {
                highestRowSolved = y;

                if (!getMultiplier)
                {
                    gm.ResetScoreMultiplier();
                }

                (bool isDelayedLinesweep, bool isInstantLinesweep, float lineBurningMultiplier, bool lineContainsFrozenTile) = scoreSolvedRow(y, getMultiplier, tetrominoThatJustLocked);

                gm.linesCleared++;
                gm.lastLineClearTime = Time.time;
                rowsCleared++;
                
                if (isInstantLinesweep)
                {
                    instantLinesweepsCleared++;
                    gm.linesweepsCleared += 1;
                }
                else if (isDelayedLinesweep)
                {
                    delayedLinesweepsCleared++;
                    gm.linesweepsCleared += 1;
                }

                burningMultiplier += lineBurningMultiplier;
                containsFrozenTile = containsFrozenTile || lineContainsFrozenTile;
            }
        }

        // Hitstop pause
        if (rowsCleared > 0)
        {
            if (!gm.isHitstopPaused)
            {
                Time.timeScale = 0f;
                gm.isHitstopPaused = true;
            }
            //FADEOUT TODO
            float delayTime = 0.15f;
            if (PlayerPrefs.GetInt("LineClearDelayEnabled", 1) == 0) // If Line Clear Delay is disabled, set delay time to 0.
                delayTime = 0;
            gm.StartCoroutine(gm.DeleteFullRowsDelayedHelper(delayTime, rowsCleared, rowsFilled, instantLinesweepsCleared, tetrominoThatJustLocked));
        }
        /*// Delete the finished Rows
        for (int y = 0; y < gm.sizeY; ++y)
        {
            if (isRowSolved(y))
            {
                deleteRow(y);                
                decreaseRowsAbove(y + 1);
                --y;
            }
        }       */ 

        if (rowsCleared > 0)
        {
            gm.soundManager.PlayClip(gm.lineClearSound, 0.5f, true);

            // Fixed Marathon: 10 per level
            while (gm.linesCleared >= gm.level * 10)
                gm.level += 1;

            if (OnLineClearEvent != null)
                OnLineClearEvent(rowsCleared);

            string scoreTranslationKeyLineClear = "Scoring Lines Cleared";
            if (rowsCleared == 1)
                scoreTranslationKeyLineClear = "Scoring Line Cleared";
            
            // Calculate Multipliers
            float lineClearMultiplier = 1;

            lineClearMultiplier *= gm.GetRowHeightPointModifier(highestRowSolved);
            lineClearMultiplier *= burningMultiplier;
            if (containsFrozenTile)
                lineClearMultiplier *= 0.5f;            
                                                  
            if (getMultiplier)
                gm.SetScoreMultiplier(rowsCleared * rowsCleared, rowsCleared * 2);
            
            if (rowsCleared > gm.safeEdgeTilesGained)
                gm.AddSafeTileToEdges();
            
            int normalRowsCleared = (rowsCleared - delayedLinesweepsCleared) - instantLinesweepsCleared;
            bool isDifficultSweep = false;

            //Debug.Log("Instant: " + instantLinesweepsCleared + ", Delayed: " + delayedLinesweepsCleared + ", Normal: " + normalRowsCleared + "... " + tetrominoThatJustLocked);

            // Linesweep: Row was solved before the next tetromino was placed
            if (instantLinesweepsCleared > 0)
            {
                // tetrominoThatJustLocked should be gm.tetrominoSpawner.currentTetromino
                isDifficultSweep = gm.ScoreLinesweepHelper(instantLinesweepsCleared, tetrominoThatJustLocked, null, true, lineClearMultiplier, getMultiplier) || isDifficultSweep;
            }
            if (delayedLinesweepsCleared > 0)
            {
                isDifficultSweep = gm.ScoreLinesweepHelper(delayedLinesweepsCleared, gm.previousTetromino, gm.tetrominoSpawner.currentTetromino, false, lineClearMultiplier, getMultiplier) || isDifficultSweep;
            }            
            if (normalRowsCleared > 0)
            {
                // Lines Cleared Points
                float lineClearScore = 100 * normalRowsCleared;
                lineClearScore *= lineClearMultiplier;

                gm.AddScore((int)lineClearScore, scoreTranslationKeyLineClear, 1, "", "", "", true, true, false, rowsCleared); // TODO high clear
            }

            //Debug.Log("Is Difficult sweep? " + isDifficultSweep);
            gm.previousClearWasDifficultSweep = isDifficultSweep;
        }
        
        // Did you win?
        if (gm.linesCleared >= gm.linesClearedTarget && gm.isEndless == false) 
            gm.Pause(true, true);
        return rowsCleared;
    }

    IEnumerator DeleteFullRowsDelayedHelper(float duration, int rowsCleared, int rowsFilled, int instantLinesweepsCleared, GameObject tetrominoThatJustLocked)
    {
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        isHitstopPaused = false;
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
        if (tetrominoThatJustLocked != null)
            tetrominoThatJustLocked.GetComponent<Group>().CascadeTetromino();
        CheckForPerfectClear(rowsCleared, rowsFilled, instantLinesweepsCleared > 0);
        GameManager.markSolvedRows();
    }


    private bool ScoreLinesweepHelper(int linesweepsCleared, GameObject tetrominoToCheckForSweeps1, GameObject tetrominoToCheckForSweeps2, bool isInstant, float lineClearMultiplier, bool getMultiplier)
    {
        string scoreTranslationKeyLinesweep = "";
        string scoreTranslationKeyPrefix1 = "";
        string scoreTranslationKeyPrefix2 = "";

        float sweepMultiplier = lineClearMultiplier;
        if (isInstant) // Instant Sweep multiplier
        {
            sweepMultiplier *= 1.5f;
            scoreTranslationKeyPrefix2 = "Scoring Instant";
        }

        // Check for difficult sweeps, to find back-to-back. The below back-to-back bonus will also need to be applied inside the Tspinsweep separately.
        bool isTetrisweep = false;
        bool isTspinsweep = false;
        bool isTetrisweep2 = false;
        bool isTspinsweep2 = false;

        if (tetrominoToCheckForSweeps1 != null)
            (isTetrisweep, isTspinsweep) = tetrominoToCheckForSweeps1.GetComponent<Group>().CheckForTetrisweeps(getMultiplier, sweepMultiplier, isInstant, true);
        if (tetrominoToCheckForSweeps2 != null)
            (isTetrisweep2, isTspinsweep2) = tetrominoToCheckForSweeps2.GetComponent<Group>().CheckForTetrisweeps(getMultiplier, sweepMultiplier, isInstant, true);

        //Debug.Log("Lines: " + linesweepsCleared + " isInstant: " + isInstant + "! ..... Previous (or Just Locked) Tetrisweep: " + isTetrisweep + ", Previous (or Just Locked) Tspinsweep: " + isTspinsweep + ", Current Tetrisweep: " + isTetrisweep2 + ", Current Tspinsweep: " + isTspinsweep2);

        // Get Translation key...
        switch (linesweepsCleared)
        {
            case 1:
                scoreTranslationKeyLinesweep = "Scoring Linesweep";
                break;
            case 2:
                scoreTranslationKeyLinesweep = "Scoring Duosweep";
                break;
            case 3:
                scoreTranslationKeyLinesweep = "Scoring Triosweep";
                break;
            case 4:
                //isTetrisweep = true;
                scoreTranslationKeyLinesweep = "Scoring Tetrisweep";
                break;
            default: // Pentominos go here...                
                scoreTranslationKeyLinesweep = "Scoring Tetrisweep";
                break;
        }

        bool isDifficultSweep = isTetrisweep || isTspinsweep || isTetrisweep2 || isTspinsweep2;
        if (isDifficultSweep && previousClearWasDifficultSweep)
        {
            sweepMultiplier *= 1.5f;
            scoreTranslationKeyPrefix1 = "Scoring Back-to-back";
        }

        // Linesweep Score...
        float linesweepScore = 100 * (linesweepsCleared * linesweepsCleared);
        linesweepScore *= sweepMultiplier;
        // Add normal line clear score...
        float lineClearScore = 100 * linesweepsCleared;
        lineClearScore *= lineClearMultiplier;

        AddScore(Mathf.FloorToInt(linesweepScore) + (int)lineClearScore, scoreTranslationKeyLinesweep, 1, scoreTranslationKeyPrefix1, scoreTranslationKeyPrefix2); // TODO high clear

        if (getMultiplier)
            SetScoreMultiplier(10 * linesweepsCleared, 10f);

        // Tetrisweep Staggerred... Triggered when part of a Tetrisweep was scored as an instant sweep
        if (isTetrisweep && linesweepsCleared < 4)
        {
            float tetrisweepStaggeredScore = 595 * sweepMultiplier;
            AddScore((int)tetrisweepStaggeredScore, "Scoring Tetrisweep Staggered", 1, scoreTranslationKeyPrefix1, scoreTranslationKeyPrefix2); // TODO high clear
        }

        return isDifficultSweep;
    }

    public static void markSolvedRows()
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (gm.isGameOver)
            return;
        
        for (int y = 0; y < gm.sizeY; ++y)
        {
            if (isRowSolved(y))
            {
                for (int x = 0; x < gm.sizeX; ++x)
                {
                    gm.GetGameTile(x, y).isRowSolved = true;
                }
                if (gm.GetGameTile(-1, y) != null)
                    gm.GetGameTile(-1, y).isRowSolved = true;
                if (gm.GetGameTile(gm.sizeX, y) != null)
                    gm.GetGameTile(gm.sizeX, y).isRowSolved = true;
            }
            else
            {
                for (int x = 0; x < gm.sizeX; ++x)
                {
                    if (gm.GetGameTile(x, y) != null)
                    {
                        gm.GetGameTile(x, y).isRowSolved = false;
                    }
                }
                if (gm.GetGameTile(-1, y) != null)
                    gm.GetGameTile(-1, y).isRowSolved = false;
                if (gm.GetGameTile(gm.sizeX, y) != null)
                    gm.GetGameTile(gm.sizeX, y).isRowSolved = false;
            }
        }
    }

    public static bool isRowSolved(int y)
    {
        if (!isRowFull(y))
            return false;
        
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        bool isSolved = true;
        for (int x = 0; x < gm.sizeX; ++x)
        {
            if (gameBoard[x][y] != null)
            {
                Tile tile = gameBoard[x][y].GetComponent<Tile>();
                if (!(tile.isRevealed && !tile.isMine)
                    && !(!tile.isRevealed && tile.isMine && tile.isFlagged))
                {
                    if (tile.aura != Tile.AuraType.frozen
                        && !tile.burnoutInvisible)
                        isSolved = false;
                }                    
            }
        }
        return isSolved;
    }

    public void AddSafeTileToEdges() 
    {
        if (gameMods.wallType == GameModifiers.WallType.unlock)
        {
            // Place display tiles at bottom
            GameObject newTile = Instantiate(tile, new Vector3(-1, leftBorderTiles.Count-1, 0), new Quaternion(0, 0, 0, 0), this.gameObject.transform) as GameObject;
            newTile.name = "Tile (" + -1 + ", " + (leftBorderTiles.Count-1) + ")";
            newTile.GetComponent<Tile>().coordX = -1;
            newTile.GetComponent<Tile>().coordY = leftBorderTiles.Count - 1;
            newTile.GetComponent<Tile>().isRevealed = true;
            newTile.GetComponent<Tile>().isDisplay = true;
            if (safeEdgeTilesGained != -1)
                newTile.GetComponentInChildren<Tile>().shimmerOverlay.gameObject.SetActive(true);
            leftBorderTiles.Add(newTile); 

            newTile = Instantiate(tile, new Vector3(sizeX, rightBorderTiles.Count-1, 0), new Quaternion(0, 0, 0, 0), this.gameObject.transform) as GameObject;
            newTile.name = "Tile (" + sizeX + ", " + (rightBorderTiles.Count-1) + ")";
            newTile.GetComponent<Tile>().coordX = sizeX;
            newTile.GetComponent<Tile>().coordY = rightBorderTiles.Count - 1;
            newTile.GetComponent<Tile>().isRevealed = true;
            newTile.GetComponent<Tile>().isDisplay = true;
            if (safeEdgeTilesGained != -1)
                newTile.GetComponentInChildren<Tile>().shimmerOverlay.gameObject.SetActive(true);
            rightBorderTiles.Add(newTile); 

            if (safeEdgeTilesGained == 0)
            {
                GameObject.Find("Tile (-1, -1)").GetComponent<Tile>().shimmerOverlay.gameObject.SetActive(true);
                GameObject.Find("Tile (" + sizeX + ", -1)").GetComponent<Tile>().shimmerOverlay.gameObject.SetActive(true);
            }

            safeEdgeTilesGained++;
        }        
    }

    /*public void RemoveSafeTileFromEdges() 
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
    }*/
    #endregion
    #region Gamestate Logic
    public void EndGame(string deathType = "")
    {
        if (cheatGodMode)
            return;
        if (isGameOver)
            return;
        
        endtime = GetTime();
        isGameOver = true;        

        if (!canPause)
            TriggerOnResetStartingPositionsEvent();

        if (!isTitleMenu)
            ScoreKeeper.Instance.SaveCurrentGame();
        
        soundManager.DisablePauseFilter();
        soundManager.StopMultiplierDrain();

        if (OnGameOverEvent != null)
            OnGameOverEvent();

        // Reveal all tiles!
        /*for (int i = -1; i <= sizeX; i++)
        {
            for (int j = -1; j < sizeY + 4; j++)
            {
                if (GetGameTile(i, j) != null)
                {
                    //GetGameTile(i, j).isFlagged = false;
                    GetGameTile(i, j).Reveal(true);
                }
            }
        }*/

        if (!isTitleMenu)
            GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>().Stop();

        soundManager.PlayClip(gameOverSound, 0.35f, true);

        StartCoroutine(GameOver(deathType));        
    }

    IEnumerator GameOver(string deathType = "") // "" = normal, "Burn"
    {
        yield return new WaitForSeconds(1f);
        if (!isTitleMenu)
        {
            if (deathType == "")
                gameOverMenu.GetComponent<GameOverMenu>().SetDeathNormal();
            else if (deathType.ToLower() == "burn")
                gameOverMenu.GetComponent<GameOverMenu>().SetDeathBurn();

            gameOverMenu.SetActive(true);
        }
            
    }
    #endregion
    #region Scoring
    public void AddScore(int newScore, string scoreSourceName, int scoreSourceType, string translationKeyPrefix1 = "", string translationKeyPrefix2 = "", string translationKeySuffix = "", bool levelMultiplierActive = true, bool scoreMultiplierActive = true, bool combineExistingScoreFloatingText = false, int comboCountOverride = 1)
    {
        float tempScore = newScore;
        if (scoreMultiplierActive)
            if (GetScoreMultiplier() > 0)
                tempScore = tempScore * (1 + GetScoreMultiplier());
        if (levelMultiplierActive)
            tempScore = tempScore * level;      

        tempScore = Mathf.Floor(tempScore);

        scoreByPointSourceType[scoreSourceType] += tempScore; // 0=Block Placing, 1=Line Clearing, 2=Minesweeping, 3=Misc.
        if (pieChart != null)
            pieChart.SetValues(scoreByPointSourceType);
        
        score += tempScore;

        //floater.GetComponent<TextMeshProUGUI>().text = "+" + tempScore.ToString("#,#")

        Transform scoreFloaterSpawnPoint = transform;
        //GameObject floater = Instantiate(floatingText, new Vector3((sizeX / 2f) - 0.5f, (sizeY - 4) / 1.5f, 0), Quaternion.identity, guiCanvas.transform);
        //floater.GetComponent<TextMeshProUGUI>().text = "+" + tempScore.ToString("#,#") + " " + "Placeholder Text";//GetTranslation("UIText", scoreSourceName);

        if (floatingTextQueue != null && scoreSourceName != "")
        {
            if (scoreSourceName == "Scoring Tile Revealed")
                floatingTextQueue.SpawnText(tempScore, scoreSourceName, combineExistingScoreFloatingText, translationKeyPrefix1, translationKeyPrefix2, translationKeySuffix, comboCountOverride, 10);
            //floatingTextQueue.UpdateSingularFloater(newScore, scoreSourceName, translationKeyPrefix1, translationKeyPrefix2, translationKeySuffix, comboCountOverride);
            else
                floatingTextQueue.SpawnText(tempScore, scoreSourceName, combineExistingScoreFloatingText, translationKeyPrefix1, translationKeyPrefix2, translationKeySuffix, comboCountOverride);
        }
            

        if (score >= bestTodayStarting && !isTitleMenu && !gameMods.detailedTimer) // Normal score mode
        {
            if (!isHighScore && score >= highScoreStarting  && highScoreStarting > 0 && !isEndless)
            {
                GameObject highScoreFloater = Instantiate(floatingText, new Vector3((sizeX / 2f) - 0.5f, (sizeY - 4) / 1.5f, 0), Quaternion.identity, guiCanvas.transform);
                highScoreFloater.GetComponent<TextMeshProUGUI>().text = GetTranslation("UIText", "GUI HighScore") + "!";

                soundManager.PlayClip(fanfareSounds[Random.Range(0, fanfareSounds.Length)], 1, true);
                isHighScore = true;
                isBestToday = true;
            }

            if (!isBestToday && bestTodayStarting > 0 && !isEndless)
            {
                GameObject highScoreFloater = Instantiate(floatingText, new Vector3((sizeX / 2f) - 0.5f, (sizeY - 4) / 1.5f, 0), Quaternion.identity, guiCanvas.transform);
                highScoreFloater.GetComponent<TextMeshProUGUI>().text = GetTranslation("UIText", "GUI HighScoreBestToday") + "!";
                soundManager.PlayClip(fanfareSounds[Random.Range(0, fanfareSounds.Length)], 1, true);
                isBestToday = true;
            }

            if (!isHighScoreEndless && score >= highScoreEndlessStarting && highScoreEndlessStarting > highScoreStarting && isEndless)
            {
                GameObject highScoreFloater = Instantiate(floatingText, new Vector3((sizeX / 2f) - 0.5f, (sizeY - 4) / 1.5f, 0), Quaternion.identity, guiCanvas.transform);
                highScoreFloater.GetComponent<TextMeshProUGUI>().text = GetTranslation("UIText", "GUI HighScore") + "!";
                soundManager.PlayClip(fanfareSounds[Random.Range(0, fanfareSounds.Length)], 1, true);
                isHighScoreEndless = true;
            }
        }        
    }

    public void SetScoreMultiplier(int mult, float duration, bool isSweep = false)
    {
        float sm = GetScoreMultiplier();
        scoreMultiplier += mult;
        if (scoreMultiplier < 0)
        {
            scoreMultiplier = 0;
            soundManager.StopMultiplierDrain();
        }            
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
            
            if (Mathf.FloorToInt(scoreMultiplierTimer) > scoreMultiplierTimerCountdown && Mathf.FloorToInt(scoreMultiplierTimer) >= 1)
                scoreMultiplierTimerCountdown = Mathf.Min(Mathf.FloorToInt(scoreMultiplierTimer), 3);
            soundManager.StopMultiplierDrain();
            scoreMultiplierAtDrain = GetScoreMultiplier();
            SetMultiplierBackground();
        }

        if (GetScoreMultiplier() > 0)
            backgroundAnimated.SetActive(true);
        if (GetScoreMultiplier() > highestScoreMultiplier)
            highestScoreMultiplier = GetScoreMultiplier();
        
        lastMultiplierTick = Time.time;
        TriggerOnMultiplierEvent();
    }

    public void ResetScoreMultiplier()
    {
        //scoreMultiplier = 0;
        scoreMultiplierTimer = 0;      
        if (scoreMultiplierTimerCountdown > 0) 
            scoreMultiplierTimerCountdown = 0; 
    }

    public float GetScoreMultiplier()
    {
        //scoreMultiplier = Mathf.Floor(scoreMultiplier * 100) / 100;
        return scoreMultiplier / 100f;
    }

    private void SetMultiplierBackground()
    {
        //Debug.Log(GetScoreMultiplier() + " / " + scoreMultiplierAtDrain + " = " + Mathf.Clamp(GetScoreMultiplier() / scoreMultiplierAtDrain, 0, 1));
        float tweenedAlpha = Mathf.SmoothStep(0.25f, 1, GetScoreMultiplier() / scoreMultiplierAtDrain);
        backgroundAnimated.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, tweenedAlpha);

        foreach (SpriteRenderer bg in backgroundAnimated.GetComponentsInChildren<SpriteRenderer>())
        {
            bg.color = new Color(1, 1, 1, tweenedAlpha);
        }
    }

    // Returns true if this row was a linesweep
    public static (bool, bool, float, bool) scoreSolvedRow(int y, bool getMultiplier, GameObject tetrominoThatJustLocked)
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //int rowScore = 0;
        int minesFlagged = 0;
        bool isDelayedLinesweep = false;
        bool isInstantLinesweep = false;
        float burningMultiplier = 0f;
        bool containsFrozenTile = false;
        for (int x = 0; x < gm.sizeX; ++x)
        {
            if (gameBoard[x][y] != null)
            {
                gameBoard[x][y].GetComponent<ButtonJiggle>().ShrinkToZero(false);


                if (gameBoard[x][y].GetComponent<Tile>().isMine)
                {
                    //rowScore += 50;
                    minesFlagged += 1;
                }
                /*else
                {
                    rowScore += gameBoard[x][y].GetComponent<Tile>().nearbyMines;
                }*/
                if (gameBoard[x][y].GetComponent<Tile>().aura == Tile.AuraType.burning)
                    burningMultiplier += 0.25f;
                if (gameBoard[x][y].GetComponent<Tile>().aura == Tile.AuraType.frozen)
                    containsFrozenTile = true;

                if (gameBoard[x][y].GetComponentInParent<Group>().gameObject == gm.previousTetromino)
                    isDelayedLinesweep = true;
                if (gameBoard[x][y].GetComponentInParent<Group>().gameObject == tetrominoThatJustLocked)
                {
                    isInstantLinesweep = true;
                    isDelayedLinesweep = false; // These shouldn't actually both happen at the same time anyways...
                }
                    
            }
        }
        //gm.AddScore(50 * (y + 1));
        //gm.SetScoreMultiplier(0.2f * (y + 1), 2f);
        //if (getMultiplier)
            //gm.SetScoreMultiplier(0.5f, 2f);
        
        gm.currentMines -= minesFlagged;
        gm.currentFlags -= minesFlagged;
        gm.minesSweeped += minesFlagged;
        return (isDelayedLinesweep, isInstantLinesweep, burningMultiplier, containsFrozenTile);
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
                    gm.AddScore(100, "Scoring Line-Fill", 0);
                    gm.SetScoreMultiplier(3, 5f);
                    break;
                case 2:
                    clipToPlay = gm.lineFullSound2;
                    gm.AddScore(300, "Scoring Double-Fill", 0);
                    gm.SetScoreMultiplier(5, 5f);
                    break;
                case 3:
                    clipToPlay = gm.lineFullSound3;
                    gm.AddScore(500, "Scoring Triple-Fill", 0);
                    gm.SetScoreMultiplier(8, 5f);
                    break;
                default:
                    clipToPlay = gm.lineFullSound4;
                    int actionScore = 800;
                    if (gm.lastFillWasDifficult)
                    {
                        gm.AddScore(Mathf.RoundToInt(actionScore * 1.5f), "Scoring Tetri-Fill", 0, "Scoring Back-to-back");
                        gm.SetScoreMultiplier(20, 5f);
                    }                        
                    else
                    {
                        gm.AddScore(actionScore, "Scoring Tetri-Fill", 0);
                        gm.SetScoreMultiplier(10, 5f);
                    }
                    break;
            }
            // C-c-c-Combo!
            if (gm.comboLinesFilled > 0)
                gm.AddScore(50 * gm.comboLinesFilled, "Scoring Combo: Line Filled", 0, "", "", "", true, true, true, gm.comboLinesFilled + 1);

            gm.soundManager.PlayClip(clipToPlay, 1, true);
        }        
        return fullRows;
    }

    public void AddRevealCombo(bool isAutomatic, bool isFailedChord, int rowHeight)
    {
        //ResetRevealComboDrain();
        
        revealCombo++;
        float revealScore = revealCombo;
        if (isAutomatic)
            revealScore *= 0.1f;
        if (isFailedChord)
            revealScore *= 0.1f;
        
        revealScore *= GetRowHeightPointModifier(rowHeight);
        
        AddScore(Mathf.FloorToInt(10 * revealScore), "Scoring Tile Revealed", 2, "", "", "", true, true, true, revealCombo); // TODO automatic, failed, high reveal...
    }

    public void ResetRevealCombo()
    {
        /*if (revealComboDrainTween != null)
            if (revealComboDrainTween.IsActive())
                if (revealComboDrainTween.IsPlaying())
                    return;                      
        revealComboDrainTween = DOTween.To(()=>revealCombo, x=> revealCombo = x, 0, 31f);*/
        revealCombo = 0;
        if (floatingTextQueue != null)
            floatingTextQueue.DisableCombineExistingScoresOnFloater("Scoring Tile Revealed");
    }

    public void AddRevealStreakManual()
    {
        revealStreakManual++;

        if (revealStreakManual >= 50)
        {
            AddScore(2500, "Scoring Manual Reveal Streak (50 tiles)", 2);
            SetScoreMultiplier(10, 10, true);

            soundManager.PlayClip(revealStreakManual100Sound, 1, true);
            
            ResetRevealStreakManual();
        }
        if (revealStreakManualProgressBar != null)
            revealStreakManualProgressBar.current = revealStreakManual;
    }

    public void ResetRevealStreakManual()
    {
        revealStreakManual = 0;
        if (revealStreakManualProgressBar != null)
            revealStreakManualProgressBar.current = 0;
    }

    /*public void ResetRevealComboDrain()
    {
        if (revealComboDrainTween != null)
            if (revealComboDrainTween.IsActive())
                if (revealComboDrainTween.IsPlaying())
                    {
                        revealComboDrainTween.Kill();
                        revealComboDrainTween = null;
                    }
    }*/
    #endregion
    #region Helper Functions
    public void ReloadScene()
    {
        Time.timeScale = 1;
        DOTween.Clear(true);
        //DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitToTitleMenu () 
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Title");
    }
    public void Quit()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            Application.Quit();
        hasQuit = true;    
    }
    public void PauseFromButton(bool pause) 
    {
        Pause(pause, false, true);
    }
    public void Pause(bool pause, bool isMarathonOverPause = false, bool bypassTitlePause = false)
    {
        if (isGameOver && !isTitleMenu)
            return;
        /*if (isTitleMenu && !bypassTitlePause)
            return;*/
        if (pause && !canPause)
            return;

        if (pause)
        {
            Time.timeScale = 0;
            isPaused = true;
            if (!isTitleMenu)
                canPause = false;
            TriggerOnKillTweenEvent();
            if (isMarathonOverPause)
            {
                GameWin();                
            }
            else
            {
                if (isTitleMenu)
                {
                    settingsMenu.SetActive(true);
                    demoTitleScreen.gameObject.SetActive(false);
                }
                else
                    pauseMenu.SetActive(true);
                soundManager.EnablePauseFilter();
            }
        }
        else
        {
            settingsMenu.SetActive(false);
            if (isTitleMenu)
                demoTitleScreen.gameObject.SetActive(true);
            if (!isGameOver)
            {
                Time.timeScale = 1;
                isPaused = false;
                pauseMenu.SetActive(false);
                soundManager.DisablePauseFilter();
                marathonOverMenu.SetActive(false);
                /*if (!bypassTitlePause)
                    settingsMenu.SetActive(false);*/
                StartCoroutine(ResetPause());
            }
        }
    }
    
    private void GameWin()
    {
        isEndless = true;
        marathonOverMenu.SetActive(true);
        if (!isTitleMenu)
        {
            ScoreKeeper.Instance.SaveCurrentGame();

            if (gameMods.detailedTimer)
            {
                Debug.Log(GetTime() + ": ...best? " + ScoreKeeper.Instance.bestTimeToday + " isEndless=" + isEndless);
                if (ScoreKeeper.Instance.bestTime == GetTime())
                {
                    Debug.Log("Best Time! " + GetTime());
                    GameObject floater = Instantiate(floatingText, new Vector3((sizeX / 2f) - 0.5f, (sizeY - 4) / 1.4f, 0), Quaternion.identity, guiCanvas.transform);
                    floater.GetComponent<TextMeshProUGUI>().text = GetTranslation("UIText", "GUI HighScore") + "!";
                    floater.GetComponent<FloatingText>().duration = 3f;
                    soundManager.PlayClip(fanfareSounds[Random.Range(0, fanfareSounds.Length)], 1, true);
                }
                else if (ScoreKeeper.Instance.bestTimeToday == GetTime() && ScoreKeeper.Instance.runs > 1)
                {
                    Debug.Log("Best Time Today! " + GetTime());
                    soundManager.PlayClip(fanfareSounds[Random.Range(0, fanfareSounds.Length)], 1, true);
                    GameObject floater = Instantiate(floatingText, new Vector3((sizeX / 2f) - 0.5f, (sizeY - 4) / 1.4f, 0), Quaternion.identity, guiCanvas.transform);
                    floater.GetComponent<TextMeshProUGUI>().text = GetTranslation("UIText", "GUI HighScoreBestToday") + "!";
                    floater.GetComponent<FloatingText>().duration = 3f;                    
                }
            }
        }
    }
    IEnumerator ResetPause()
    {
        yield return new WaitForSeconds(0.4f);
        TriggerOnResetStartingPositionsEvent();
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
        if (!isStarted)
            return 0;
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

    public static string GetTranslation(string tableName, string entryName, int depth = 0)
    {
        StringTable table = LocalizationSettings.StringDatabase.GetTableAsync(tableName).Result;
        
        if (table == null)
        {
            //Debug.Log(entryName + ", " + depth);
            if (depth > 10)
                return entryName;
            return GetTranslation(tableName, entryName, depth + 1);
        }
        else
            if (table.GetEntry(entryName) != null)
                return table.GetEntry(entryName).GetLocalizedString();
        return "";
            //[entryName].LocalizedValue;
/*#if !UNITY_EDITOR && UNITY_WEBGL
            return LocalizationSettings.StringDatabase.GetTable(tableName)[entryName].LocalizedValue;
#endif
        return LocalizationSettings.StringDatabase.GetLocalizedString(tableName, entryName);*/
    }
    #endregion
    #region Extra Helpers
    public void SetGridOpacity()
    {
        Color gridColor = backgroundGrid.GetComponent<SpriteRenderer>().color;
        backgroundGrid.GetComponent<SpriteRenderer>().color = new Color(gridColor.r, gridColor.g, gridColor.b, PlayerPrefs.GetFloat("GridOpacity", 0));
    }
    #endregion
    #region Event Triggers
    public void TriggerOnHardDropEvent()
    {
        if (OnHardDropEvent != null)
            OnHardDropEvent();
    }

    public void TriggerOnLeftStuckEvent()
    {
        if (OnLeftStuckEvent != null)
            OnLeftStuckEvent();
    }

    public void TriggerOnRightStuckEvent()
    {
        if (OnRightStuckEvent != null)
            OnRightStuckEvent();
    }

    public void TriggerOnMinoLockEvent()
    {
        if (OnMinoLockEvent != null)
            OnMinoLockEvent();
    }

    public void TriggerOnTileSolveOrLandEvent()
    {
        if (OnTileSolveOrLandEvent != null)
            OnTileSolveOrLandEvent();
    }

    public void TriggerOnTSpinEvent(int dir)
    {        
        if (OnTSpinEvent != null)
            OnTSpinEvent(dir);
    }

    public void TriggerOnResetStartingPositionsEvent()
    {
        if (OnResetStartingPositionsEvent != null)
            OnResetStartingPositionsEvent();
    }

    public void TriggerOnKillTweenEvent()
    {
        if (OnKillTweenEvent != null)
            OnKillTweenEvent();
    }

    public void TriggerOnNewPieceEvent()
    {
        if (OnNewPieceEvent != null)
            OnNewPieceEvent();
    }

    public void TriggerOnMultiplierEvent()
    {
        if (OnMultiplierEvent != null)
            OnMultiplierEvent();
    }

    public float GetRowHeightPointModifier(int rowHeight)
    {
        float adjustedHeight = (rowHeight / 20f) * (sizeY - 4);

        if (adjustedHeight > 16)
            return 1.75f;
        else if (adjustedHeight > 11)
            return 1.5f;
        else if (adjustedHeight > 6)
            return 1.25f;
        
        return 1;
    }
    #endregion
}
