using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoSpawner : MonoBehaviour
{
    public Transform previewTarget;
    public GameObject[] groups;

    public GameObject currentTetromino;
    public GameObject nextTetromino;

    ArrayList groupStack = new ArrayList();

    int nextIndex;

    // Start is called before the first frame update
    void Start()
    {
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
        nextTetromino = Instantiate((GameObject)groupStack[i], previewTarget.position, Quaternion.identity);
        nextTetromino.GetComponent<Group>().isHeld = true;

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

        // If the previous score was a Tetris (4 rows), spawn a bonus tetromino with no mines!
        if (bonusTile)
        {
            currentTetromino.GetComponent<Group>().isBonus = true;
        }

        currentTetromino.GetComponent<Group>().LayMines();

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
