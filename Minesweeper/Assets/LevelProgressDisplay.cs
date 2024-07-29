using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressDisplay : MonoBehaviour
{
    GameManager gm;
    ProgressBar progressBar;

    void OnEnable()
    {
        GameManager.OnLineClearEvent += _ => LineClear(_);
        GameManager.OnNewPieceEvent += NewPiece;
    }
    void OnDisable()
    {
        GameManager.OnLineClearEvent -= _ => LineClear(_);
        GameManager.OnNewPieceEvent -= NewPiece;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        progressBar = GetComponent<ProgressBar>();
    }

    void LineClear(int lines)
    {
        progressBar.maximum = gm.level * 100;
        progressBar.minimum = (gm.level - 1) * 100;
        progressBar.current = gm.linesCleared * 10;
    }

    void NewPiece()
    {
        if (gm != null)
        {
            if (progressBar.current > 0)
                progressBar.ChangeColor(gm.tetrominoSpawner.currentTetromino.GetComponentInChildren<Button>().image.color);
            else
                progressBar.fill.color = gm.tetrominoSpawner.currentTetromino.GetComponentInChildren<Button>().image.color;
        }            
    }
}
