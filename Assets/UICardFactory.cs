using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICardFactory : MonoBehaviour
{
    [SerializeField]
    private GameObject uiCardTemplate;
    // Start is called before the first frame update


    public GameObject CreateCardWithData(BaseCardData data)
    {
        var newCard = Instantiate(uiCardTemplate);
        newCard.GetComponent<UICard>().SetFromCardData(data);
        return newCard;
    }

    void Start() 
    {
        //Create a Copy of All of our Cards in the Database.
        var CardDatabase = new CardDatabase();
        var cardsProcessed = 0;
        foreach ( var cardData in CardDatabase.GetAll())
        {
            var cardUI =  CreateCardWithData(cardData);
            var newPosition = new Vector3(cardUI.gameObject.transform.position.x + (30 * cardsProcessed), cardUI.gameObject.transform.position.y, 0);
            cardUI.gameObject.transform.position = newPosition;
            cardsProcessed++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
