using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System;

public class DrawCardAnimation : MonoBehaviour
{
    [SerializeField] private Deck3D _deck;
    [SerializeField] private Hand3D _hand;
    [SerializeField] private float _animationTime = 0.3f;

    Sequence currentSequence;

    public float AnimationTime { get => _animationTime; set => _animationTime = value; }

    public void PlayAnimation(Deck3D deck, Hand3D hand, CardInstance cardDrawn, Action onComplete = null)
    {
        //Note we added this in in order to try and solve a bug we were having, i'm not entirely sure if 
        //thse actually do anything at the moment, but it might be nice to have.

        if (currentSequence != null)
        {
            if (currentSequence.IsComplete() == false)
            {
                currentSequence.Complete();
                currentSequence.Kill();
                currentSequence = null;
            }
        }

        //Grab the last card3D gameObject from the deck.
        //de parent the card
        _deck = deck;
        _hand = hand;

        var lastCardInDeck = _deck.GetComponentsInChildren<Card3D>().ToList().LastOrDefault();
        var cardToMove = Instantiate(lastCardInDeck);
        cardToMove.transform.position = lastCardInDeck.transform.position;
        
        //cardToMove.transform.SetParent(null);
        cardToMove.SetCardInfo(cardDrawn, true, false);

        //Update the cards in the deck.
        _deck._numberOfCards--;

        //Afterwards, move the card nto the players hand at the next available spot. 
        //We need to find out where the next transform is in the hand
        //Ways of doing this : add 1 to the hands number of cards, scale that card down, then use that transform to move our own card.
        _hand.numberOfCards++;
        _hand.UpdateCards();

        var lastCard = _hand.GetComponentsInChildren<Card3D>().LastOrDefault();
        //Hide the card, while we run the animation.

        lastCard.gameObject.SetActive(false);

        var position = lastCard.transform.position;
        var scaling = lastCard.transform.localScale;
        var rotation = lastCard.transform.rotation;

        cardToMove.name = "Card (Draw Animation)";

        //Move it to the draw card point,
        //At the same time as moving, we want to rotate it so that the card information is facing the player

        var drawCardSequence = DOTween.Sequence();

        drawCardSequence.Append(cardToMove.transform.DOMove(position, _animationTime).SetEase(Ease.Linear));
        drawCardSequence.Join(cardToMove.transform.DORotateQuaternion(rotation, _animationTime));
        drawCardSequence.Join(cardToMove.transform.DOScale(scaling, _animationTime + 0.1f));
        drawCardSequence.OnComplete(() =>
        {
            lastCard.gameObject.SetActive(true);
            lastCard.SetCardInfo(cardToMove); //need this just in case we haven't gotten an updated game state yet from the game.
            Destroy(cardToMove.gameObject);
            if (onComplete != null)
            {
                onComplete();
            }
            Destroy(cardToMove.gameObject);
            currentSequence = null;
        });
    }
}
