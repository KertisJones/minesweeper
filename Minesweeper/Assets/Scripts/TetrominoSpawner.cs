using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TetrominoSpawner : MonoBehaviour
{
    private Camera mainCamera;
    private GameManager gm;
    public Transform previewTarget;
    public GameObject[] groups;

    public GameObject currentTetromino;
    public GameObject nextTetromino;

    ArrayList groupStack = new ArrayList();

    int nextIndex;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();


        this.transform.position = new Vector3(Mathf.Ceil((gm.sizeX / 2f) - 1), gm.sizeY - 3, 0); // Default is 4,20
        spawnPreview();
        spawnNext();
    }

    private void spawnPreview()
    {
        if (groupStack.Count == 0)
        {
            groupStack = GenerateNewStack();
        }

        // Random Index
        int i = Random.Range(0, groupStack.Count);

        // Spawn Group at current Position
        nextTetromino = Instantiate((GameObject)groupStack[i], previewTarget.position, Quaternion.identity, this.transform);
        nextTetromino.GetComponent<Group>().isHeld = true;
        //mainCamera.DOColor(nextTetromino.GetComponentInChildren<UnityEngine.UI.Button>().image.color * new Color(0.725f, 0.725f, 0.725f), 0.5f);

        groupStack.Remove(groupStack[i]);
    }

    public void spawnNext(bool bonusTile = false)
    {
        // Spawn Group at current Position
        currentTetromino = nextTetromino;
        currentTetromino.transform.position = this.transform.position; 
        currentTetromino.GetComponent<Group>().isHeld = false;
        if (!currentTetromino.GetComponent<Group>().isValidGridPos() && !currentTetromino.GetComponent<Group>().isDisplay)
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().EndGame();
        
        currentTetromino.GetComponent<Group>().UpdateGrid();
        currentTetromino.GetComponent<Group>().SpawnTetrominoOnBoard(true);
        // If the previous score was a Tetris (4 rows), spawn a bonus tetromino with no mines!
        if (bonusTile)
        {
            currentTetromino.GetComponent<Group>().isBonus = true;
        }

        currentTetromino.GetComponent<Group>().LayMines();
        currentTetromino.GetComponent<Group>().UpdateInputValues();

        spawnPreview();
    }

    /*public void spawnNext(bool bonusTile = false)
    {        
        if (groupStack.Count == 0)
        {
            groupStack = GenerateNewStack();
        }

        // Random Index
        int i = Random.Range(0, groupStack.Count);

        // Spawn Group at current Position
        currentTetromino = Instantiate((GameObject)groupStack[i], this.transform.position, Quaternion.identity);

        // If the previous score was a Tetris (4 rows), spawn a bonus tetromino with no mines!
        if (bonusTile)
        {
            currentTetromino.GetComponent<Group>().isBonus = true;            
        }

        groupStack.Remove(groupStack[i]);
    }*/




    ArrayList GenerateNewStack()
    {
        ArrayList tempStack = new ArrayList();
        ArrayList newStack = new ArrayList();

        tempStack.AddRange(groups);
        
        while (tempStack.Count > 0)
        {
            int i = Random.Range(0, tempStack.Count - 1);

            newStack.Add(tempStack[i]);
            tempStack.Remove(tempStack[i]);
        }

        return newStack;
    }


}
