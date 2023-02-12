using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostTile : MonoBehaviour
{
    //public Group group;
    //public Tile tile;
    TetrominoSpawner spawner;
    public GameObject ghostTile1;
    public GameObject ghostTile2;
    public GameObject ghostTile3;
    public GameObject ghostTile4;

    // Start is called before the first frame update
    void Awake()
    {
        //group = this.GetComponentInParent<Group>();
        spawner = FindObjectOfType<TetrominoSpawner>();
        //tile = this.GetComponentInParent<Tile>();
        //this.transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        Group group = spawner.currentTetromino.GetComponent<Group>();
        List<Transform> tiles = new List<Transform>();

        foreach (Transform child in group.transform)
        {
            if (child.gameObject.GetComponent<Tile>() != null)
            {
                tiles.Add(child);
            }
        }

        int offsetDistance = group.maximumFallDistance;
        ghostTile1.transform.position = tiles[0].position + new Vector3(0, group.maximumFallDistance * -1, 2);
        ghostTile2.transform.position = tiles[1].position + new Vector3(0, group.maximumFallDistance * -1, 2);
        ghostTile3.transform.position = tiles[2].position + new Vector3(0, group.maximumFallDistance * -1, 2);
        ghostTile4.transform.position = tiles[3].position + new Vector3(0, group.maximumFallDistance * -1, 2);

        Color color = tiles[0].GetComponentInChildren<TileButton>().gameObject.GetComponent<Image>().color;
        color.a = 0.5f;
        ghostTile1.GetComponent<SpriteRenderer>().color = color;
        ghostTile2.GetComponent<SpriteRenderer>().color = color;
        ghostTile3.GetComponent<SpriteRenderer>().color = color;
        ghostTile4.GetComponent<SpriteRenderer>().color = color;

        //if (group == null)
            //return;
        
        /*if (group == null)
            return;
        if (!group.isFalling)
            Destroy(this.gameObject);
        
        if (group.gameObject != FindObjectOfType<TetrominoSpawner>().currentTetromino)
        {
            if (group.gameObject == FindObjectOfType<TetrominoSpawner>().nextTetromino)
                return;
            else
                Destroy(this.gameObject);
        }*/
            
        
        //int offsetDistance = group.GetMaximumFallDistance();
        //Debug.Log(offsetDistance);
        /*
        
        int offsetDistance = group.GetMaximumFallDistance();
        Debug.Log(offsetDistance);*/
        //this.transform.position = tile.transform.position + new Vector3(0, group.maximumFallDistance * -1, 0);
    }
}
