using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZoneViewer2D : MonoBehaviour, IZoneViewer
{
    [SerializeField]
    private Scrollbar _scrollbar;

    [SerializeField]
    private GameObject _cardsContainer;

    [SerializeField]
    private TextMeshProUGUI _nameOfZone;

    [SerializeField]
    private Button _exitButton;

    private IZone _zone;

    public void SetZone(IZone zone, bool setReverse = false)
    {
        _zone = zone;
        SetNameOfZoneText(zone);
        SetCardsInZone(zone, setReverse);
        SetContainerSize();
    }

    void Awake()
    {
        _scrollbar.onValueChanged.AddListener(BindViewToScrollBar);
        _exitButton.onClick.AddListener(Exit);
    }

    private void SetContainerSize()
    {
        var _cardsViewRect = _cardsContainer.GetComponent<RectTransform>();
        //Setting the size of the "container" based on how many cards there are
        //225 is the width of the UI card (350) * its scaling (0.5) + a little padding (25)
        _cardsViewRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _cardsContainer.GetComponentsInChildren<IUICard>().Count() * (375f * 0.5f));
    }

    private void Exit()
    {
        this.gameObject.SetActive(false);
    }

    private void BindViewToScrollBar(float value)
    {
        var _cardsViewRect = _cardsContainer.GetComponent<RectTransform>();
        _cardsViewRect.anchoredPosition = new Vector3(value * -1 * _cardsContainer.GetComponent<RectTransform>().rect.width, _cardsViewRect.localPosition.y, _cardsViewRect.localPosition.z);
    }

    private void SetNameOfZoneText(IZone zone)
    {
        _nameOfZone.SetText($"Viewing {zone.Name}");
    }

    private void SetCardsInZone(IZone zone, bool setReverse = false)
    {
        //Get any already made ui cards;
        var alreadyMadeUICards = _cardsContainer.GetComponentsInChildren<IUICard>();

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
                var cardGameObject = UICardFactory.CreateCard2D(card);
                cardGameObject.AddComponent<LayoutElement>();
                cardGameObject.transform.SetParent(_cardsContainer.transform, false);
                cardGameObject.transform.localPosition = new Vector3(0, 0, 0); //actual position will be handled by the layout group.
                cardGameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 375);
            }
            else
            {
                alreadyMadeUICards[i].SetCardData(card);
                alreadyMadeUICards[i].SetActive(true);
                ((MonoBehaviour)(alreadyMadeUICards[i])).GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 375);
            }
        }

        if (alreadyMadeUICards.Length > cardsToSet.Count)
        {
            for (var i = zone.Cards.Count; i < alreadyMadeUICards.Length; i++)
            {
                alreadyMadeUICards[i].SetActive(false);
            }
        }
    }
}
