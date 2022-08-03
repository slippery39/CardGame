using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICard2D : UIGameEntity, IUICard
{
    [SerializeField]
    private Image _cardFrame;

    [SerializeField]
    private Image _cardArt;

    [SerializeField]
    private TextMeshProUGUI _cardName;

    [SerializeField]
    private TextMeshProUGUI _cardManaCost;

    [SerializeField]
    private TextMeshProUGUI _cardType;

    [SerializeField]
    private TextMeshProUGUI _cardCombatStats;

    [SerializeField]
    private TextMeshProUGUI _cardRulesText;

    [SerializeField]
    private GameObject _backOfCard;

    [SerializeField]
    private GameObject _frontOfCard;

    private CardInstance _cardInstance;

    [SerializeField]
    private Sprite whiteCardFrame;
    [SerializeField]
    private Sprite blueCardFrame;
    [SerializeField]
    private Sprite greenCardFrame;
    [SerializeField]
    private Sprite redCardFrame;
    [SerializeField]
    private Sprite blackCardFrame;
    [SerializeField]
    private Sprite colorlessCardFrame;
    [SerializeField]
    private Sprite multicolorCardFrame;

    public void SetAsHiddenCard()
    {
        _frontOfCard.gameObject.SetActive(false);
        _backOfCard.gameObject.SetActive(true);
    }

    public void SetCardData(CardInstance cardInstance)
    {
        if (cardInstance.GetZone().ZoneType == ZoneType.Deck && cardInstance.RevealedToOwner == false)
        {
            SetAsHiddenCard();
            return;
        }

        _backOfCard.gameObject.SetActive(false);
        _frontOfCard.gameObject.SetActive(true);
        _cardInstance = cardInstance;

        EntityId = cardInstance.EntityId;

        var cardData = cardInstance.CurrentCardData;

        if (cardData is SpellCardData)
        {
            _cardCombatStats.gameObject.SetActive(false);
            _cardManaCost.gameObject.SetActive(true);
        }
        //in case it has already been hidden previously.
        else if (cardData is UnitCardData)
        {
            _cardCombatStats.gameObject.SetActive(true);
            _cardManaCost.gameObject.SetActive(true);
        }
        else if (cardData is ManaCardData)
        {
            _cardCombatStats.gameObject.SetActive(false);
            _cardManaCost.gameObject.SetActive(false);
        }
        else if (cardData is ItemCardData)
        {
            _cardCombatStats.gameObject.SetActive(false);
        }

        //Warning: we might want to update this to the CardInstanceAttributes..
        _cardName.text = cardData.Name;
        _cardRulesText.text = cardInstance.RulesText;

        if (cardInstance.Shields > 0)
        {
            _cardRulesText.text += $"\r\n {cardInstance.Shields} Shields";
        }

        _cardManaCost.text = cardInstance.ManaCost;
        _cardType.text = cardData.CardType;

        if (cardData is UnitCardData)
        {
            _cardCombatStats.text = cardInstance.Power + " / " + (cardInstance.Toughness - cardInstance.DamageTaken);
        }
        Sprite art = Resources.Load<Sprite>(cardData.ArtPath);
        _cardArt.sprite = art;

        SetCardFrame();
    }

    private void SetCardFrame()
    {
        if (_cardInstance == null)
        {
            _cardFrame.sprite = colorlessCardFrame;
            return;
        }
        if (_cardInstance.Colors == null || !_cardInstance.Colors.Any())
        {
            //default to colorless frame;
            _cardFrame.sprite = colorlessCardFrame;
            return;
        }

        if (_cardInstance.Colors.Count > 1)
        {
            //Do a multicolor frame.
            _cardFrame.sprite = multicolorCardFrame;
        }
        else
        {
            var color = _cardInstance.Colors.First();

            //Do a single color frame.
            switch (color)
            {
                case CardColor.White: _cardFrame.sprite = whiteCardFrame; break;
                case CardColor.Blue: _cardFrame.sprite = blueCardFrame; break;
                case CardColor.Green: _cardFrame.sprite = greenCardFrame; break;
                case CardColor.Red: _cardFrame.sprite = redCardFrame; break;
                case CardColor.Black: _cardFrame.sprite = blackCardFrame; break;
                case CardColor.Colorless: _cardFrame.sprite = colorlessCardFrame; break;
                default: _cardFrame.sprite = colorlessCardFrame; break;
            }
        }
    }

    public void SetCardData(BaseCardData cardData)
    {
        throw new System.NotImplementedException();
    }
}
