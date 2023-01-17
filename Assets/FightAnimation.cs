using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightAnimation : MonoBehaviour
{
    [SerializeField] private Transform attackingCard;
    [SerializeField] private Transform thingGettingAttacked;


    public void PlayAnimation()
    {
        //The attacking card will "wind up" which means it goes slightly back, then it attacks the other card by moving to the other cards transform
        //at the same time it will knock the other card back a little bit.
        //Afterwards both cards will return to their starting points. 

        var sequence = DOTween.Sequence();

        var originalPosition = attackingCard.transform.position;

        sequence.Append(attackingCard.transform.DOMove(thingGettingAttacked.transform.position, 0.15f));
        sequence.Append(attackingCard.transform.DOMove(originalPosition, 0.15f));

        sequence.SetEase(Ease.InBack);
    }
}
