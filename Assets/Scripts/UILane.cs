using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UILane : UIGameEntity, IPointerClickHandler
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private UICard _uiCard;

    private void Awake()
    {
        _uiCard = GetComponentInChildren<UICard>();
    }

    public void SetCard(CardInstance cardInstance)
    {
        if (!_uiCard.gameObject.activeSelf)
        {
            _uiCard.gameObject.SetActive(true);
        }
        _uiCard.SetCardData(cardInstance);
    }

    public void SetEmpty()
    {
        _uiCard.EntityId = -1;
        _uiCard.gameObject.SetActive(false);
    }

    public override void Highlight()
    {
        _spriteRenderer.gameObject.SetActive(true);
    }

    public override void StopHighlight()
    {
        _spriteRenderer.gameObject.SetActive(false);
    }

    public new void OnPointerClick(PointerEventData pointerEventData)
    {

        base.OnPointerClick(pointerEventData);
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        //Only going to propagate for the next available gameobject hit for now... perhaps it should be for all available game objects..
        if (raycastResults.Count > 1)
        {
            var newTarget = raycastResults[1].gameObject;

            //Allow the click event to proprgate through to children that are covered by the parent.
            if (newTarget.GetComponent<UIGameEntity>())
            {
                var gameEntity = newTarget.GetComponent<UIGameEntity>();
                Debug.Log(newTarget.name + "has received the propagated click event");
                gameEntity.OnPointerClick(pointerEventData);
            }
        }
    }
}
