using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightAnimation : MonoBehaviour
{
    [SerializeField] private Transform attackingCard;
    [SerializeField] private Transform thingGettingAttacked;


    [SerializeField] private Ease attackingEasingMethod = Ease.InBack;
    [SerializeField] private Ease defendingEasingMethod = Ease.OutCirc;

    private Sequence attackingSequence;
    private Sequence defendingSequence;

    public void PlayAnimation()
    {
        //We do not want to run an animation if one is already playing
        if (
            (attackingSequence != null && attackingSequence.IsPlaying())
            || (defendingSequence != null && defendingSequence.IsPlaying())
            )
        {
            return;
        }
        //The attacking card will "wind up" which means it goes slightly back, then it attacks the other card by moving to the other cards transform
        //at the same time it will knock the other card back a little bit.
        //Afterwards both cards will return to their starting points. 

        var originalAttackPos = attackingCard.transform.position;
        var originalDefPos = thingGettingAttacked.transform.position;

        var thingGettingAttackedBounds = thingGettingAttacked.GetComponent<Collider>().bounds.min.z;
        var positionTo = new Vector3(
            thingGettingAttacked.position.x,
            thingGettingAttacked.position.y,
            thingGettingAttackedBounds);

        var attackingSeq = DOTween.Sequence();
        attackingSeq.Append(attackingCard.transform.DOMove(positionTo, 0.3f)
        .SetEase(attackingEasingMethod))
        .Append(attackingCard.transform.DOMove(originalAttackPos, 0.1f))
        .OnComplete(() =>
        {
            attackingSequence = null;
            attackingCard.transform.position = originalAttackPos;
        });

        attackingSequence = attackingSeq;

        var defendingSeq = DOTween.Sequence();
        defendingSeq.Append(thingGettingAttacked.transform.DOMove(originalDefPos + new Vector3(0, 0, 1), 0.2f)
        .SetDelay(0.25f)
        .SetEase(defendingEasingMethod)
        )
        .Append(thingGettingAttacked.transform.DOMove(originalDefPos, 0.1f))
        .OnComplete(() =>
        {
            defendingSequence = null;
            thingGettingAttacked.transform.position = originalDefPos;
        });

        defendingSequence = defendingSeq;
    }
}
