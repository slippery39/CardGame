using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
        //Look for any already initialized cards
        var alreadyInitializedCards = this.GetComponentsInChildren<Card3D>(true);
        _instantiatedCards = alreadyInitializedCards.ToList();

        for (var i = 0; i < numberOfCards; i++)
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
            card.transform.localPosition = new Vector3((bounds.x + cardOffSet) * (i), card.transform.localPosition.y, card.transform.localPosition.z);
        }

        //Hide any additional cards
        for (var i = numberOfCards; i < _instantiatedCards.Count; i++)
        {
            _instantiatedCards[i].gameObject.SetActive(false);
        }
    }
}
