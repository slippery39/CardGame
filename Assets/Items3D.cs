using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;



public class Items3D : MonoBehaviour
{

    [SerializeField] private Card3D _cardPrefab;
    private List<Card3D> _instantiatedCards;

    [SerializeField] public int numberOfCards = 5;
    [SerializeField] int cardsPerRow = 4;
    [SerializeField] float cardSpacingY = 0.5f;
    [SerializeField] float cardSpacingX = 0.5f;
    [SerializeField] float rowOffsetX = -4f;
    [SerializeField] float rowOffsetY = -4f;

    public List<Card3D> GetCards()
    {
        return _instantiatedCards;
    }
    /// <summary>
    /// Updates the cards positions and visibility based on the number of cards to be shown.
    /// </summary>
    public void UpdateCards()
    {
        //Look for any already initialized cards
        var alreadyInitializedCards = this.GetComponentsInChildren<Card3D>(true);
        _instantiatedCards = alreadyInitializedCards.ToList();

        for (var i = 0; i < numberOfCards; i++)
        {
            Card3D card;
            if (_instantiatedCards.Count <= i)
            {
                card = PrefabUtility.InstantiatePrefab(_cardPrefab) as Card3D;
                _instantiatedCards.Add(card);
            }
            else
            {
                card = _instantiatedCards[i];
                card.gameObject.SetActive(true);
            }
            card.transform.SetParent(transform, false);
            var bounds = card.GetBounds().size;
            if (cardsPerRow == 0)
            {
                return;
            }

            var row = Mathf.Ceil(i / cardsPerRow);
            var verticalPosition = (row * (-bounds.y - cardSpacingY)) - rowOffsetY;
            var horizontalPosition = (bounds.x + cardSpacingX) * (i % cardsPerRow) - rowOffsetX;

            card.transform.localPosition = new Vector3(horizontalPosition, verticalPosition, card.transform.localPosition.z);
        }

        //Hide any additional cards
        for (var i = numberOfCards; i < _instantiatedCards.Count; i++)
        {
            _instantiatedCards[i].gameObject.SetActive(false);
        }
    }
}
