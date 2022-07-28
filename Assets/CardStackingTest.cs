using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardStackingTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var db = new CardDatabase();

        db.GetCardData("Lightning Bolt");
        db.GetCardData("Snapcaster Mage");
        db.GetCardData("Forest");

        var cardData = new List<BaseCardData>() {
            db.GetCardData("Lightning Bolt"),
            db.GetCardData("Snapcaster Mage"),
            db.GetCardData("Forest")
        };

        var i = 0;
        GetComponentsInChildren<UICard>().ToList().ForEach(c =>
        {
            c.SetCardData(cardData[i]);
            i++;
        });


    }

    // Update is called once per frame
    void Update()
    {

    }
}
