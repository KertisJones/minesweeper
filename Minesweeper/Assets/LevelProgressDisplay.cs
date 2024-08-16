using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressDisplay : MonoBehaviour
{
    GameManager gm;
    ProgressBar progressBar;
    //Image progressBarBack;
    Image progressBarFill;

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

        //progressBarBack = progressBar.GetComponent<Image>();
        progressBarFill = progressBar.fill;
        progressBarFill.material = new Material(progressBarFill.material);

        progressBar.maximum = 100;
        progressBar.current = gm.linesCleared * 10;

        progressBarFill.material.SetFloat("_OutlineWidth", 0);
        progressBarFill.material.SetFloat("_OutlineDistortAmount", 0);
    }

    void LineClear(int lines)
    {        
        progressBar.current = gm.linesCleared * 10;
        
        bool levelUp = false;
        if (progressBar.current > progressBar.maximum)
        {
            levelUp = true;
            progressBar.maximum = gm.level * 100;
            progressBar.minimum = (gm.level - 1) * 100;
        }

        UpdateOutline(levelUp);
    }

    void NewPiece()
    {
        if (gm != null)
        {
            Color color = gm.tetrominoSpawner.currentTetromino.GetComponentInChildren<Button>().image.color;            
            progressBarFill.material = new Material(progressBarFill.material);
            progressBarFill.material.SetColor("_OutlineColor", color);

            if (progressBar.current > 0)
            {
                progressBar.ChangeColor(color);
            }
            else
            {
                progressBar.fill.color = color;                
            }
        }            
    }

    void UpdateOutline(bool isLevelUp)
    {
        progressBarFill.material = new Material(progressBarFill.material);
        
        float linesPercent = Mathf.Min(1, (float)gm.linesCleared / (float)gm.linesClearedTarget);

        float glowPercent = Mathf.Min(1,
            ((float)gm.linesCleared - (float)gm.linesClearedTarget * 0.25f) /
            ((float)gm.linesClearedTarget * 0.75f));

        //progressBarFill.material.SetColor("_GlowColor", color);
        
        progressBarFill.material.SetFloat("_OutlineGlow", 1 + 6.5f * glowPercent);

        if (linesPercent >= 0.25f && (isLevelUp || gm.linesCleared % 10 == 0))
        {
            progressBarFill.material.SetFloat("_OutlineWidth", 0.004f);
            progressBarFill.material.SetFloat("_OutlineDistortAmount", 0.2f);
        }
    }
}
