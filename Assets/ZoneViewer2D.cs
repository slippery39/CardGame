using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ZoneViewer2D : MonoBehaviour, IZoneViewer
{
    [Header("Zone Viewer Properties")]
    [SerializeField]
    private bool _showExitButton;
    [SerializeField]
    private bool _showBackGround;
    [SerializeField]
    private bool _showZoneName;

    [Header("Game Object References")]

    [SerializeField]
    private GameObject _viewport;

    [SerializeField]
    private GameObject _background;

    [SerializeField]
    private Scrollbar _scrollbar;

    [SerializeField]
    private GameObject _cardsContainer;

    [SerializeField]
    private TextMeshProUGUI _nameOfZone;

    [SerializeField]
    private Button _exitButton;

    [Header("Card Sizing")]
    [SerializeField]
    private float _cardWidth = 375f;

    [SerializeField]
    private float _cardScaling = 0.5f;

    private IZone _zone;

    public void SetZone(IZone zone, bool setReverse = false)
    {
        _zone = zone;
        SetNameOfZoneText(zone);
        SetCardsInZone(zone, setReverse);
        SetContainerSize();
        HideScrollbar();
    }

    void Awake()
    {
        _scrollbar.onValueChanged.AddListener(BindViewToScrollBar);
        _exitButton.onClick.AddListener(Exit);
    }

    void Update()
    {
        _exitButton.gameObject.SetActive(_showExitButton);
        _background.gameObject.SetActive(_showBackGround);
        _nameOfZone.gameObject.SetActive(_showZoneName);

#if UNITY_EDITOR
        SetContainerSize();
        HideScrollbar();
        SetCardSizes();
#endif
    }

    private void SetContainerSize()
    {
        var _cardsViewRect = _cardsContainer.GetComponent<RectTransform>();
        //Setting the size of the "container" based on how many cards there are
        //225 is the width of the UI card (350) * its scaling (0.5) + a little padding (25)
        _cardsViewRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _cardsContainer.GetComponentsInChildren<IUICard>().Count() * (_cardWidth * _cardScaling));
    }


    private void Exit()
    {
        this.gameObject.SetActive(false);
    }

    private void BindViewToScrollBar(float value)
    {
        //Scrollbar Max Scroll should be the width of the container - the width of the canvas

        var widthOfViewPort = _viewport.transform.GetComponent<RectTransform>().rect.width;
        var _cardsViewRect = _cardsContainer.GetComponent<RectTransform>();

        //X Position should be a Lerp of the scrollbar value and 

        var maxPositionVal = (_cardsViewRect.rect.width - widthOfViewPort);
        var xPosition = Mathf.Lerp(0, -1 * maxPositionVal, value);
        _cardsViewRect.anchoredPosition = new Vector3(xPosition, _cardsViewRect.localPosition.y, _cardsViewRect.localPosition.z);
    }

    private void HideScrollbar()
    {
        var widthOfCanvas = transform.GetComponentInParent<Canvas>().pixelRect.width;
        var _cardsViewRect = _cardsContainer.GetComponent<RectTransform>();

        if (_cardsViewRect.rect.width <= widthOfCanvas)
        {
            _scrollbar.gameObject.SetActive(false);
            return;
        }
        else
        {
            _scrollbar.enabled = true;
            _scrollbar.gameObject.SetActive(true);
        }
    }

    private void SetNameOfZoneText(IZone zone)
    {
        _nameOfZone.SetText($"Viewing {zone.Name}");
    }

    /// <summary>
    /// Used for editor mode only currently
    /// </summary>
    private void SetCardSizes()
    {
        var alreadyMadeUICards = _cardsContainer.GetComponentsInChildren<IUICard>();
        var scalingVector = new Vector3(_cardScaling, _cardScaling, _cardScaling);

        for (var i = 0; i < alreadyMadeUICards.Count(); i++)
        {
            var rect = ((MonoBehaviour)(alreadyMadeUICards[i])).GetComponent<RectTransform>();
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _cardWidth);
            rect.localScale = scalingVector;
        }
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

        var scalingVector = new Vector3(_cardScaling, _cardScaling, _cardScaling);

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

                var rect = cardGameObject.GetComponent<RectTransform>();

                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _cardWidth);
                rect.localScale = scalingVector;
            }
            else
            {
                alreadyMadeUICards[i].SetCardData(card);
                alreadyMadeUICards[i].SetActive(true);

                var rect = ((MonoBehaviour)(alreadyMadeUICards[i])).GetComponent<RectTransform>();
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _cardWidth);
                rect.localScale = scalingVector;
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
