using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightAnimation : MonoBehaviour
{
    [SerializeField] private Transform attackingCard;
    [SerializeField] private Transform thingGettingAttacked;

    [SerializeField] private float _animationTime = 0.5f;
    [SerializeField] private float _defendBounceAmount = 0.5f;

    [SerializeField] private AnimationCurve _attackAnimationCurve;
    [SerializeField] private AnimationCurve _defendAnimationCurve;

    private Sequence attackingSequence;
    private Sequence defendingSequence;

    public float AnimationTime { get => _animationTime; set => _animationTime = value; }

    public void PlayAnimation(Transform attacking, Transform defending, Action onComplete = null)
    {
        attackingCard = attacking;
        thingGettingAttacked = defending;
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
        attackingSeq.Append(attackingCard.transform.DOMove(positionTo, _animationTime)
        .SetEase(_attackAnimationCurve))
        .OnComplete(() =>
        {
            attackingSequence = null;
            attackingCard.transform.position = originalAttackPos;
        });

        attackingSequence = attackingSeq;

        var defendingSeq = DOTween.Sequence();
        defendingSeq.Append(thingGettingAttacked.transform.DOMove(originalDefPos + ((positionTo*-1).normalized * _defendBounceAmount), _animationTime)
        .SetEase(_defendAnimationCurve)
        )
        .OnComplete(() =>
        {
            defendingSequence = null;
            thingGettingAttacked.transform.position = originalDefPos;
            if (onComplete != null)
            {
                onComplete();
            }
        });

        defendingSequence = defendingSeq;
    }
}
