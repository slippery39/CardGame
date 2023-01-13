using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[ExecuteAlways]
public class Hand3D : MonoBehaviour
{

    [SerializeField] private Card3D _cardPrefab;
    [SerializeField] private List<Card3D> _instantiatedCards;

    [SerializeField] int numberOfCards = 5;
    [SerializeField] float cardOffSet = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        //InitializeCards();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        InitializeCards();
#endif
    }

    void InitializeCards()
    {
        for (var i = 0; i < numberOfCards; i++)
        {
            Card3D card;
            if (_instantiatedCards.Count <= i)
            {
                card = Instantiate<Card3D>(_cardPrefab);
                _instantiatedCards.Add(card);
            }
            else
            {
                card = _instantiatedCards[i];
            }
            card.transform.SetParent(transform, false);
            var bounds = card.GetBounds().size;
            card.transform.localPosition = new Vector3( (bounds.x+cardOffSet) * (i), card.transform.localPosition.y, card.transform.localPosition.z);
        }
    }
}
