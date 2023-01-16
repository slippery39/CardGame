using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class Graveyard3D : MonoBehaviour
{
    [SerializeField] public Card3D _cardPrefab;
    [SerializeField] public int _numberOfCards;

    private List<Card3D> _instantiatedCards;

    private void Update()
    {
        InitializeCards();
    }
    void InitializeCards()
    {
        //Look for any already initialized cards
        var alreadyInitializedCards = this.GetComponentsInChildren<Card3D>(true);
        _instantiatedCards = alreadyInitializedCards.ToList();

        for (var i = 0; i < _numberOfCards; i++)
        {
            Card3D card;
            if (_instantiatedCards.Count <= i)
            {
#if UNITY_EDITOR
                card = PrefabUtility.InstantiatePrefab(_cardPrefab) as Card3D;
#else

                card = Instantiate<Card3D>(_cardPrefab);

#endif


                _instantiatedCards.Add(card);

            }
            else
            {
                card = _instantiatedCards[i];
                card.gameObject.SetActive(true);
            }
            card.transform.SetParent(transform, false);
            var bounds = card.GetBounds().size;
            card.transform.localPosition = new Vector3(0, 0.01f * i, 0);
            card.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }

        //Hide any additional cards
        for (var i = _numberOfCards; i < _instantiatedCards.Count; i++)
        {
            _instantiatedCards[i].gameObject.SetActive(false);
        }
    }
}
