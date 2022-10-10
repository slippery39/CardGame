using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICardFactory : MonoBehaviour
{
    [SerializeField]
    private GameObject _cardPrefab2D;
    // Start is called before the first frame update

    public static UICardFactory Instance;

    public GameObject CardPrefab2D { get => _cardPrefab2D; set => _cardPrefab2D = value; }

    public static GameObject CreateCard2D(CardInstance card)
    {
        var cardObject = GameObject.Instantiate(Instance.CardPrefab2D);
        cardObject.GetComponent<UICard2D>().SetCardData(card);
        var uiGameEntity = cardObject.AddComponent<UIGameEntity>();
        uiGameEntity.EntityId = card.EntityId;
        return cardObject;
    }

    public static GameObject CreateCard2D(ICard card)
    {
        if (card is CardInstance)
        {
            return CreateCard2D((CardInstance)card);
        }
        else
        {
            throw new System.Exception("ERROR : Create Card 2D is not properly handling the creation of card. Possibly an unsupported class type");
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Can only instantiate one UICardFactory... you should check the scene for duplicates and remove them");
        }

        //Sanity Checks
        if (CardPrefab2D == null)
        {
            Debug.LogError("No card prefab 2D has been set for the UICardFactory");
        }
    }
}
