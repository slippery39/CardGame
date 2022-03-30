using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// This MonoBehaviour should be inherited from any UI entity that corresponds to 
/// an entity in the actual game that can be interacted with.
/// 
/// Will be used by our targetting system to highlight which things can be targeted by certain spells.
/// 
/// Examples: Cards, Players, Lanes, etc...
/// </summary>
public class UIGameEntity : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    int _entityId;
    public int EntityId { get => _entityId; set => _entityId = value; }

    //Override these as necessary
    public virtual void Highlight()
    {
        Debug.Log($@"{EntityId} should be getting highlighted!");
    }

    public virtual void StopHighlight()
    {
        Debug.Log($@"{EntityId} should stop being highlighted!");
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        UIGameController.Instance.HandleClick(new UIGameControllerClickEvent { EntityId = this.EntityId });
        //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        Debug.Log(name + " Game Object Clicked!");
    }
}
