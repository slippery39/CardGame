using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UICard : UIGameEntity
{

    [SerializeField]
    private GameObject _frontOfCard;

    [SerializeField]
    private GameObject _backOfCard;
    //Unity Fields
    [SerializeField]
    private TextMeshPro _cardNameText;
    [SerializeField]
    private TextMeshPro _cardRulesText;
    [SerializeField]
    private TextMeshPro _cardCombatStatsText;
    [SerializeField]
    private TextMeshPro _cardManaText;
    [SerializeField]
    private TextMeshPro _cardTypeText;
    [SerializeField]
    private SpriteRenderer _cardArtRenderer;

    [SerializeField]
    private SpriteRenderer _highlight;

    [SerializeField]
    private SpriteRenderer _cardFrame;

    //Card Frame Colors
    [SerializeField]
    private Sprite colorlessCardFrame;
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
    private Sprite multiColorCardFrame;

    [SerializeField]
    private CardInstance _cardInstance;

    #region Public Properties
    public SpriteRenderer CardArtRenderer { get => _cardArtRenderer; set => _cardArtRenderer = value; }
    public TextMeshPro CardRulesText { get => _cardRulesText; set => _cardRulesText = value; }

    #endregion

    public void SetAsHiddenCard()
    {
        _backOfCard.gameObject.SetActive(true);
        _frontOfCard.gameObject.SetActive(false);
    }

    public void SetCardData(BaseCardData cardData)
    {

        if (cardData is SpellCardData)
        {
            _cardCombatStatsText.gameObject.SetActive(false);
            _cardManaText.gameObject.SetActive(true);
        }
        //in case it has already been hidden previously.
        else if (cardData is UnitCardData)
        {
            _cardCombatStatsText.gameObject.SetActive(true);
            _cardManaText.gameObject.SetActive(true);
        }
        else if (cardData is ManaCardData)
        {
            _cardCombatStatsText.gameObject.SetActive(false);
            _cardManaText.gameObject.SetActive(false);
        }
        else if (cardData is ItemCardData)
        {
            _cardCombatStatsText.gameObject.SetActive(false);
        }
        _cardNameText.text = cardData.Name;
        _cardRulesText.text = cardData.RulesText;

        _cardManaText.text = cardData.ManaCost;
        _cardTypeText.text = cardData.CardType;

        if (cardData is UnitCardData)
        {
            var unitCard = cardData as UnitCardData;
            _cardCombatStatsText.text = unitCard.Power + " / " + (unitCard.Toughness);
        }
        Sprite art = Resources.Load<Sprite>(cardData.ArtPath);
        _cardArtRenderer.sprite = art;

        SetCardFrame();
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
            _cardCombatStatsText.gameObject.SetActive(false);
            _cardManaText.gameObject.SetActive(true);
        }
        //in case it has already been hidden previously.
        else if (cardData is UnitCardData)
        {
            _cardCombatStatsText.gameObject.SetActive(true);
            _cardManaText.gameObject.SetActive(true);
        }
        else if (cardData is ManaCardData)
        {
            _cardCombatStatsText.gameObject.SetActive(false);
            _cardManaText.gameObject.SetActive(false);
        }
        else if (cardData is ItemCardData)
        {
            _cardCombatStatsText.gameObject.SetActive(false);
        }

        //Warning: we might want to update this to the CardInstanceAttributes..
        _cardNameText.text = cardData.Name;
        _cardRulesText.text = cardInstance.RulesText;

        if (cardInstance.Shields > 0)
        {
            _cardRulesText.text += $"\r\n {cardInstance.Shields} Shields";
        }

        _cardManaText.text = cardInstance.ManaCost;
        _cardTypeText.text = cardData.CardType;

        if (cardData is UnitCardData)
        {
            _cardCombatStatsText.text = cardInstance.Power + " / " + (cardInstance.Toughness - cardInstance.DamageTaken);
        }
        Sprite art = Resources.Load<Sprite>(cardData.ArtPath);
        _cardArtRenderer.sprite = art;

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
            _cardFrame.sprite = multiColorCardFrame;
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

    public override void Highlight()
    {
        _highlight.gameObject.SetActive(true);
        _highlight.color = Color.green;
    }

    public override void Highlight(Color highlightColor)
    {
        _highlight.gameObject.SetActive(true);
        _highlight.color = highlightColor;
    }

    public override void StopHighlight()
    {
        _highlight.gameObject.SetActive(false);
    }
}
