using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane3D : MonoBehaviour, IHighlightable
{
    [SerializeField] GameObject _laneSprite;
    [SerializeField] Card3D _card;

    public void Highlight()
    {
        _laneSprite.SetActive(true);
    }

    public void Highlight(Color highlightColor)
    {
        _laneSprite.SetActive(true);
    }

    public void SetUnitInLane(CardInstance card)
    {
        if (card == null)
        {
            _card.gameObject.SetActive(false);
        }
        else
        {
            _card.gameObject.SetActive(true);
            _card.SetCardInfo(card);
            UIGameEntity3D.AddToCard3D(_card, card);
        }
    }

    public void StopHighlight()
    {
        _laneSprite.SetActive(false);
    }
}
