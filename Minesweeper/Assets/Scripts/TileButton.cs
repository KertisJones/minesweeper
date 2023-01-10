using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TileButton : MonoBehaviour, IPointerClickHandler
{
    Tile tile;
    void Update()
    {
        tile = GetComponentInParent<Tile>();
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().isGameOver)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            tile.Reveal();
            if (Input.GetMouseButton(1))
                tile.Chord();
        }            
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            tile.QuestionToggle();
            tile.Chord();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            tile.FlagToggle();
            if (Input.GetMouseButton(0))
                tile.Chord();
        }
    }
}
