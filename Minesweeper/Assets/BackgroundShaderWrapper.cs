using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
public class BackgroundShaderWrapper : MonoBehaviour
{
    private Material material;
    private GameManager gm;
    private float levelScaledTime = 0;
    private Color gradiantTempTop = Color.black;
    private Color gradiantTempBottom = Color.black;
    private Color colorMultiplier = new Color(0.725f, 0.725f, 0.725f); //828282
    public bool updateColorsWithNext = false;
    void OnEnable()
    {
        //GameManager.OnLineClearEvent += _ => LineClear(_);
        GameManager.OnNewPieceEvent += NewPiece;
        GameManager.OnMinoLockEvent += LockPiece;
        //GameManager.OnMultiplierEvent += Multiplier;
    }
    void OnDisable()
    {
        //GameManager.OnLineClearEvent -= _ => LineClear(_);
        GameManager.OnNewPieceEvent -= NewPiece;
        GameManager.OnMinoLockEvent -= LockPiece;
        //GameManager.OnMultiplierEvent -= Multiplier;
    }

    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        NewPiece();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpeed();
    }

    public void UpdateSpeed()
    {
        float maxSpeed = 3f;
        float speedIncreasePerLevel = maxSpeed / 19f;
        float currentSpeed = 1 + ((gm.level - 1) * speedIncreasePerLevel);
        if (gm.level >= 20)
            currentSpeed = 1 + maxSpeed;


        levelScaledTime += Time.unscaledDeltaTime * currentSpeed;
        material.SetFloat("_UnscaledTime", levelScaledTime);
    }

    void NewPiece()
    {
        if (gm != null && updateColorsWithNext)
        {
            if (gm.tetrominoSpawner.currentTetromino != null)
                material.DOColor(gm.tetrominoSpawner.currentTetromino.GetComponent<Group>().GetTileColor() * colorMultiplier, "_GradiantColor01", 2.5f);
        }
    }

    void LockPiece()
    {
        if (gm != null && updateColorsWithNext)
        {
            if (gm.previousTetromino != null)
                material.DOColor(gm.previousTetromino.GetComponent<Group>().GetTileColor() * colorMultiplier, "_GradiantColor02", 2.5f);
        }
    }
    /*void Multiplier()
    {
        if (gm != null)
        {
            if (gm.scoreMultiplierLimit >= gm.scoreMultiplierLimit)
                material.DOColor(Color.white, "_OverlayColor", 1f);
        }
    }*/
}
