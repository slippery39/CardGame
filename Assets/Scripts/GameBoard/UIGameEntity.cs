using System;
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
/// 
//TODO - This should be a component, not a class that gets inherited from.
public class UIGameEntity : MonoBehaviour, IUIGameEntity, IPointerClickHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    bool clickEventsEnabled = true;

    [SerializeField]
    int _entityId;
    public int EntityId { get => _entityId; set => _entityId = value; }

    //Set in Awake.
    public Action<UIGameControllerClickEvent> OnClickHandler;

    public void Awake()
    {
        OnClickHandler = HandleOnClick;
    }

    //Override these as necessary
    public virtual void Highlight()
    {
        var highlightable = GetComponent<IHighlightable>();
        if (highlightable != null)
        {
            highlightable.Highlight();
        }
        else
        {
            Debug.Log($@"{EntityId} should be getting highlighted!");
        }
    }

    public virtual void Highlight(Color highlightColor)
    {
        var highlightable = GetComponent<IHighlightable>();
        if (highlightable != null)
        {
            highlightable.Highlight(highlightColor);
        }
        else
        {
            Debug.Log($@"{EntityId} should be getting highlighted with a specific color!");

        }
    }

    public virtual void StopHighlight()
    {
        var highlightable = GetComponent<IHighlightable>();
        if (highlightable != null)
        {
            highlightable.StopHighlight();
        }
        else
        {
            //Debug.Log($@"{EntityId} should stop being highlighted");
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log($"Pointer has been clicked for {name} -EntityID: {EntityId}");
        if (clickEventsEnabled)
        {
            OnClickHandler(new UIGameControllerClickEvent { EntityId = EntityId });
        }
    }

    //Our default handle click method.. can be overriden if necessary.
    private void HandleOnClick(UIGameControllerClickEvent eventData)
    {
        UIGameController.Instance.HandleClick(new UIGameControllerClickEvent { EntityId = EntityId });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       // Debug.Log($"Pointer has been entered for {name} - EntityID:  {EntityId} ");
        UIGameController.Instance.HandlePointerEnter(EntityId);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
