using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TileButton : MonoBehaviour, IPointerClickHandler
{
    void Update()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().isGameOver)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GetComponentInParent<Tile>().Reveal();            
        }            
        else if (eventData.button == PointerEventData.InputButton.Middle)
            GetComponentInParent<Tile>().QuestionToggle();
        else if (eventData.button == PointerEventData.InputButton.Right)
            GetComponentInParent<Tile>().FlagToggle();
    }
}
