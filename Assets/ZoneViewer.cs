using System.Collections;
using System.Collections.Generic;
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

    public void SetZone(IZone zone)
    {
        //Get any already made ui cards;
        var alreadyMadeUICards = _cards.GetComponentsInChildren<UICard>(true);

        _zone = zone;

        for (var i = 0; i < zone.Cards.Count; i++)
        {
            var card = zone.Cards[i];
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

        if (alreadyMadeUICards.Length > zone.Cards.Count)
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
