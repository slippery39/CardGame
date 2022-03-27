using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILane : UIGameEntity
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
}
