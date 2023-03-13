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

    public float AnimationTime { get => _animationTime; set => _animationTime = value; }

    public void PlayAnimation(Deck3D deck, Hand3D hand, CardInstance cardDrawn, Action onComplete = null)
    {
        //Grab the last card3D gameObject from the deck.
        //de parent the card
        _deck = deck;
        _hand = hand;

        var cardToMove = _deck.GetComponentsInChildren<Card3D>().ToList().LastOrDefault();
        //cardToMove.transform.SetParent(null);
        cardToMove.SetCardInfo(cardDrawn);

        if (cardToMove == null)
        {
            Debug.LogWarning("No card available to move");
            return;
        }

        //Update the cards in the deck.
        _deck._numberOfCards--;

        //Afterwards, move the card into the players hand at the next available spot. 
        //We need to find out where the next transform is in the hand
        //Ways of doing this : add 1 to the hands number of cards, scale that card down, then use that transform to move our own card.
        _hand.numberOfCards++;
        _hand.UpdateCards();

        var lastCard = _hand.GetComponentsInChildren<Card3D>().LastOrDefault();
        //Hide the card, while we run the animation.

        lastCard.gameObject.SetActive(false);

        if (lastCard == null)
        {
            Debug.LogWarning("Last Card in hand is null");
            return;
        }

        var position = lastCard.transform.position;
        var scaling = lastCard.transform.localScale;
        var rotation = lastCard.transform.rotation;

        //Move it to the draw card point,
        //At the same time as moving, we want to rotate it so that the card information is facing the player
        cardToMove.transform.DOMove(position, _animationTime).SetEase(Ease.Linear);
        cardToMove.transform.DORotateQuaternion(rotation, _animationTime);
        cardToMove.transform.DOScale(scaling, _animationTime)
            .OnComplete(() =>
            {
                Destroy(cardToMove.gameObject);
                lastCard.gameObject.SetActive(true);
                lastCard.SetCardInfo(cardToMove); //need this just in case we haven't gotten an updated game state yet from the game.
                if (onComplete != null)
                {
                    onComplete();
                }
            });
    }
}
