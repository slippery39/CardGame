using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane3D : MonoBehaviour
{
    [SerializeField] Card3D _card;
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
}
