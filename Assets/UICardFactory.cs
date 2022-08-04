using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICardFactory : MonoBehaviour
{
    [SerializeField]
    private GameObject _cardPrefab;

    [SerializeField]
    private GameObject _cardPrefab2D;
    // Start is called before the first frame update

    public static UICardFactory Instance;

    public GameObject CardPrefab { get => _cardPrefab; set => _cardPrefab = value; }
    public GameObject CardPrefab2D { get => _cardPrefab2D; set => _cardPrefab2D = value; }

    public static GameObject CreateCard(CardInstance card)
    {
        var cardObject = GameObject.Instantiate(Instance.CardPrefab);
        cardObject.GetComponent<UICard>().SetCardData(card);
        return cardObject;
    }

    public static GameObject CreateCard2D(CardInstance card)
    {
        var cardObject = GameObject.Instantiate(Instance.CardPrefab2D);
        cardObject.GetComponent<UICard2D>().SetCardData(card);
        return cardObject;
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

        if (CardPrefab == null)
        {
            Debug.LogError("No card prefab has been set for the UICardFactory");
        }

        if (CardPrefab2D == null)
        {
            Debug.LogError("No card prefab 2D has been set for the UICardFactory");

        }
    }
}
