using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stack3D : MonoBehaviour
{
    [SerializeField] Card3D _cardPrefab;
    List<Card3D> _cards; 
    
    public void SetCards(List<CardInstance> cards)
    {
        DestroyChildren();
        foreach (var card in cards)
        {
            var cardUI = Instantiate(_cardPrefab);
            cardUI.transform.SetParent(transform, true);
            cardUI.SetCardInfo(card);
            UIGameEntity3D.AddToGameObject(cardUI.gameObject, card);
            //The stack is here more as a hacky solution
            //in case we need to find game objects in the UI, it shouldn't really show otherwise. 
            cardUI.gameObject.SetActive(false);
        }
    }

    private void DestroyChildren()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
