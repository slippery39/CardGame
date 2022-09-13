using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UICard2D))]
public class UIActionChoice : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private int _choiceIndex;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"UI Cast Mode Choice Clicked for : {_choiceIndex}");
        UIGameController.Instance.HandleCastChoice(_choiceIndex); 
    }

    public void SetAction(CardGameAction action)
    {
        var uiCard = GetComponent<UICard2D>();
        uiCard.SetFromAction(action);
    }
}
