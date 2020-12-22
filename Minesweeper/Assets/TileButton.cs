using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TileButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GetComponentInParent<Tile>().Reveal();
            GetComponent<Button>().interactable = false;
        }            
        else if (eventData.button == PointerEventData.InputButton.Middle)
            GetComponentInParent<Tile>().QuestionToggle();
        else if (eventData.button == PointerEventData.InputButton.Right)
            GetComponentInParent<Tile>().FlagToggle();
    }
}
