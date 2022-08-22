using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICastModeChoice : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private int _choiceIndex;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"UI Cast Mode Choice Clicked for : {_choiceIndex}");
        UIGameController.Instance.HandleCastChoice(_choiceIndex); 
    }
}
