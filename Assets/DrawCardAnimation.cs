using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class DrawCardAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _drawCardTransform1;
    [SerializeField] private Deck3D _deck;
    [SerializeField] private Hand3D _hand;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void PlayAnimation()
    {
        Debug.Log("playing animation");
        //Grab the last card3D gameObject from the deck.
        //de parent the card

        var cardToMove = _deck.GetComponentsInChildren<Card3D>().ToList().LastOrDefault();
        cardToMove.transform.SetParent(null);

        if (cardToMove == null)
        {
            Debug.LogWarning("No card available to move");
            return;
        }

        //Update the cards in the deck.
        _deck._numberOfCards--;

        //Move it to the draw card point,
        //At the same time as moving, we want to rotate it so that the card information is facing the player
        cardToMove.transform.DOMove(_drawCardTransform1.transform.position, 0.5f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                //Afterwards, move the card into the players hand at the next available spot. 
                //We need to find out where the next transform is in the hand

                //Ways of doing this : add 1 to the hands number of cards, scale that card down, then use that transform to move our own card.
                _hand.numberOfCards++;
                _hand.InitializeCards();

                var lastCard = _hand.GetComponentsInChildren<Card3D>().LastOrDefault();

                if (lastCard == null)
                {
                    Debug.LogWarning("Last Card in hand is null");
                    return;
                }

                var position = lastCard.transform.position;
                var scaling = lastCard.transform.localScale;
                var rotation = lastCard.transform.rotation;
                //Hide the card, while we run the animation.
                lastCard.transform.localScale = new Vector3(0, 0, 0);
                cardToMove.transform.DOMove(position, 0.5f);
                cardToMove.transform.DORotateQuaternion(rotation, 0.5f);
                cardToMove.transform.DOScale(scaling, 0.5f)
                .OnComplete(() =>
                {
                    Destroy(cardToMove.gameObject);
                    lastCard.transform.localScale = scaling;
                });
            });
        cardToMove.transform.DOLocalRotate(new Vector3(90, 0, 0), 0.25f).SetEase(Ease.Linear);
    }
}
