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
    //public GameObject nextTetromino;

    public List<GameObject> tetrominoPreviewList = new List<GameObject>();
    public List<GameObject> groupStack = new List<GameObject>();
    //ArrayList groupStack = new ArrayList();

    public int previewCount = 5;
    int nextIndex;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        previewCount = gm.gameMods.previewCount;

        this.transform.position = new Vector3(Mathf.Ceil((gm.sizeX / 2f) - 1), gm.sizeY - 3, 0); // Default is 4,20
        SpawnPreview();
        SpawnNext();        
    }

    private void SpawnPreview(bool overridePreviewCount = false)
    {
        if (groupStack.Count == 0)
            groupStack = GenerateNewStack();

        if (tetrominoPreviewList.Count >= previewCount && !overridePreviewCount)
            return;

        // Random Index
        //int i = Random.Range(0, groupStack.Count);
        //int newIndex = tetrominoPreviewList.Count;

        // Spawn Group at current Position
        GameObject newTetromino = Instantiate(groupStack[0], GetPreviewPosition(previewTarget.position, groupStack[0].GetComponent<Group>().tetrominoType, tetrominoPreviewList.Count, previewCount), Quaternion.identity, this.transform);
        newTetromino.GetComponent<Group>().isHeld = true;
        //mainCamera.DOColor(nextTetromino.GetComponentInChildren<UnityEngine.UI.Button>().image.color * new Color(0.725f, 0.725f, 0.725f), 0.5f);

        groupStack.RemoveAt(0);//.Remove(groupStack[0]);
        tetrominoPreviewList.Add(newTetromino);

        if (tetrominoPreviewList.Count < previewCount)
            SpawnPreview();
    }

    public void SpawnNext(bool bonusTile = false)
    {
        // Spawn Group at current Position
        currentTetromino = GetNextTetromino();
        currentTetromino.transform.localPosition = Vector3.zero;// = this.transform.position; 

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

        gm.TriggerOnNewPieceEvent();

        SpawnPreview();
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

    GameObject GetNextTetromino()
    {
        if (previewCount <= 0)
            SpawnPreview(true);

        GameObject nextTetromino = tetrominoPreviewList[0];
        tetrominoPreviewList.RemoveAt(0);
        
        for (int i = 0; i < tetrominoPreviewList.Count; i++)
        {
            tetrominoPreviewList[i].transform.position = GetPreviewPosition(previewTarget.position, tetrominoPreviewList[i].GetComponent<Group>().tetrominoType, i, previewCount);
            tetrominoPreviewList[i].GetComponent<Group>().UpdateGrid();
        }

        SpawnPreview();

        return nextTetromino;
    }


    List<GameObject> GenerateNewStack()
    {
        List<GameObject> tempStack = new List<GameObject>();
        List<GameObject> newStack = new List<GameObject>();

        tempStack.AddRange(groups);
        
        while (tempStack.Count > 0)
        {
            int i = Random.Range(0, tempStack.Count - 1);

            newStack.Add(tempStack[i]);
            tempStack.Remove(tempStack[i]);
        }

        return newStack;
    }

    public static Vector3 GetPreviewPosition(Vector3 targetPos, Group.TetrominoType tetrominoType, int index = 0, int totalCount = 1)
    {
        Vector3 spawnPos = targetPos;

        if (totalCount > 2 && index > 0)
        {
            spawnPos.y -= 1;
            float topHeight = (float)targetPos.y - 1.5f;
            float totalHeight = targetPos.y - ((totalCount - 1 ) * 3);

            float totalDistance = totalHeight - topHeight;
            int totalObjectsToCenter = totalCount - 1;
            float distancePerIndex = totalDistance / totalObjectsToCenter;
            Debug.Log(totalDistance + ", " + distancePerIndex);
            spawnPos.y += index * distancePerIndex;
        }
        else
        {
            spawnPos.y -= index * 3;
        }

        if (tetrominoType == Group.TetrominoType.ITetromino)
        {
            spawnPos.y += 0.5f;
        }
        else if (tetrominoType != Group.TetrominoType.OTetromino)
        {
            spawnPos.x += 0.5f;
        }

        

        return spawnPos;

        /*if (index == 0)
            return spawnPos;

        spawnPos.y -= 1;
        spawnPos.y -= index * 2;*/

        //return spawnPos;
    }
}
