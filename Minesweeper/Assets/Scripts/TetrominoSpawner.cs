using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoSpawner : MonoBehaviour
{
    public GameObject[] groups;

    ArrayList groupStack = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {
        spawnNext();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnNext()
    {
        if (groupStack.Count == 0)
        {
            groupStack = GenerateNewStack();
        }

        // Random Index
        int i = Random.Range(0, groupStack.Count);

        // Spawn Group at current Position
        Instantiate((GameObject)groupStack[i], this.transform.position, Quaternion.identity);

        groupStack.Remove(groupStack[i]);
    }


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
