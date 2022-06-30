using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICardFactory : MonoBehaviour
{
    [SerializeField]
    private GameObject _cardPrefab;
    // Start is called before the first frame update

    public static UICardFactory Instance;

    public GameObject CardPrefab { get => _cardPrefab; set => _cardPrefab = value; }

    public static GameObject CreateCard(CardInstance card)
    {
        var cardObject = GameObject.Instantiate(Instance.CardPrefab);
        cardObject.GetComponent<UICard>().SetCardData(card);
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
    }
}
