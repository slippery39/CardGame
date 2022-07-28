using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class CardLayout : MonoBehaviour
{
    [SerializeField]
    private int _cardsPerRow = 3;

    [SerializeField]
    private float paddingRight = 0.25f;

    [SerializeField]
    private float paddingBottom = 0.3f;

    [SerializeField]
    private float cardScaling = 1;

    [SerializeField]
    private bool _stackCards = false;

    void Update()
    {

        var cards = GetComponentsInChildren<UICard>();

        if (_stackCards)
        {
            StackCards(cards);
        }
        else
        {
            LayoutCardsInRow(cards);
        }
    }

    private void StackCards(UICard[] cards)
    {

        if (!cards.Any())
        {
            return;
        }

        for (var i = 0; i < cards.Length; i++)
        {
            cards[i].transform.localPosition = new Vector3(0, 0, 0);
            cards[i].transform.localScale = new Vector3(cardScaling, cardScaling, cardScaling);
            cards[i].GetComponent<SortingGroup>().sortingOrder = cards.Length - i;
        }
    }

    private void LayoutCardsInRow(UICard[] cards)
    {
        for (var i = 0; i < cards.Length; i++)
        {
            var width = cards[i].GetComponent<Collider>().bounds.size.x;
            var height = cards[i].GetComponent<Collider>().bounds.size.y;

            var x = (i % _cardsPerRow) * (width + paddingRight);
            var y = Mathf.Floor(i / _cardsPerRow) * (-height - paddingBottom);

            cards[i].transform.localPosition = new Vector3(x, y, 0f);
            cards[i].transform.localScale = new Vector3(cardScaling, cardScaling, cardScaling);
            cards[i].GetComponentsInChildren<Renderer>().ToList().ForEach(x => x.sortingOrder = 0);
            cards[i].GetComponentsInChildren<TextMeshPro>().ToList().ForEach(x => x.sortingOrder = 0);
            cards[i].CardArtRenderer.sortingOrder = 1;
        }
    }
}
