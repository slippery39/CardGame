using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneViewer : MonoBehaviour
{
    private IZone _zone;

    [SerializeField]
    private GameObject _cards;
    public static GameObject Create(IZone zoneToView)
    {
        var zoneViewer = new GameObject($"Zone Viewer ({zoneToView.Name})");
        var component = zoneViewer.AddComponent<ZoneViewer>();

        component.SetZone(zoneToView);

        return zoneViewer;
    }

    public void SetCards(List<CardInstance> cards, bool setReverse = false)
    {

    }

    public void SetZone(IZone zone, bool setReverse = false)
    {
        //Get any already made ui cards;
        var alreadyMadeUICards = _cards.GetComponentsInChildren<UICard>(true);

        _zone = zone;

        var cardsToSet = zone.Cards.ToList();

        if (setReverse)
        {
            cardsToSet.Reverse();
        }

        for (var i = 0; i < cardsToSet.Count; i++)
        {
            cardsToSet = cardsToSet.ToList();
            var card = cardsToSet[i];
            if (i >= alreadyMadeUICards.Length)
            {
                var cardGameObject = UICardFactory.CreateCard(card);
                cardGameObject.transform.SetParent(_cards.transform, false);
            }
            else
            {
                alreadyMadeUICards[i].SetCardData(card);
                alreadyMadeUICards[i].gameObject.SetActive(true);
            }
        }

        if (alreadyMadeUICards.Length > cardsToSet.Count)
        {
            for (var i = zone.Cards.Count; i < alreadyMadeUICards.Length; i++)
            {
                alreadyMadeUICards[i].gameObject.SetActive(false);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
