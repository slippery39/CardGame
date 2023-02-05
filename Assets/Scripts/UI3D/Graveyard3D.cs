using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Graveyard3D : MonoBehaviour
{
    [SerializeField] public Card3D _cardPrefab;
    [SerializeField] public int _numberOfCards;

    private List<Card3D> _instantiatedCards;

    /// <summary>
    /// Controls the random amount of rotation to apply via perlin noise. 
    /// Can give the look of a deck that has been slighly manipulated (i.e. cards aren't perfectly rotated in sync)
    /// </summary>
    [SerializeField] private float randomRotationModifier = 20f;

    /// <summary>
    /// Controls the random amount of position to apply via perlin noise. 
    /// Can give the look of a deck that has been slighly manipulated (i.e. cards aren't perfectly rotated in sync)
    /// </summary>
    [SerializeField] private float randomPositionModifier = 0.1f;

    private void Update()
    {
        
    }

    public List<Card3D> GetCards()
    {
        return _instantiatedCards;
    }
    public void UpdateCards()
    {
        //Look for any already initialized cards
        var alreadyInitializedCards = this.GetComponentsInChildren<Card3D>(true);
        _instantiatedCards = alreadyInitializedCards.ToList();

        for (var i = 0; i < _numberOfCards; i++)
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
            var noise = Mathf.PerlinNoise((i + 0.1f) * 0.1f, (i + 0.1f) * 0.1f);

            //Note the y value is the vertical in the world
            card.transform.localPosition = new Vector3(0 + (noise - 0.5f) * randomPositionModifier, 0.01f * i, 0 + (noise - 0.5f) * randomPositionModifier);
            card.transform.localRotation = Quaternion.Euler(90, ((noise - 0.5f) * randomRotationModifier), 0);
        }

        //Hide any additional cards
        for (var i = _numberOfCards; i < _instantiatedCards.Count; i++)
        {
            _instantiatedCards[i].gameObject.SetActive(false);
        }
    }
}
