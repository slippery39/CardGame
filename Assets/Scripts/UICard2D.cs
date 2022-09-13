using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UICard2D : UIGameEntity, IUICard
{
    [Header("Debugging")]
    [SerializeField]
    private bool _revealedToOwner;

    [Header("Random")]
    private bool _showAsUnknown = false;

    [SerializeField]
    private GameObject _highlight;

    [Header("Card Properties")]
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

    [Header("Card Frame References")]


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

    public void SetAsUnknownCard()
    {
        _showAsUnknown = true;
        EntityId = -1;
        _frontOfCard.gameObject.SetActive(false);
        _backOfCard.gameObject.SetActive(true);
    }

    private void SetCardDataInternal(ICard cardData)
    {
        if (cardData is SpellCardData)
        {
            _cardCombatStats.gameObject.SetActive(false);
            _cardManaCost.gameObject.SetActive(true);
        }
        //in case it has already been hidden previously.
        else if (cardData is UnitCardData)
        {
            var unitCard = cardData as UnitCardData;
            _cardCombatStats.gameObject.SetActive(true);
            _cardManaCost.gameObject.SetActive(true);
            _cardCombatStats.text = unitCard.Power + " / " + (unitCard.Toughness);
        }
        else if (cardData is ManaCardData)
        {
            _cardCombatStats.gameObject.SetActive(false);
            _cardManaCost.gameObject.SetActive(false);
            _cardType.text += " - " + (cardData as ManaCardData).ManaAdded;
        }
        else if (cardData is ItemCardData)
        {
            _cardCombatStats.gameObject.SetActive(false);
        }

        _cardName.text = cardData.Name;
        _cardRulesText.text = cardData.RulesText;
        _cardManaCost.text = cardData.ManaCost;
        _cardType.text = cardData.CardType;
        Sprite art = Resources.Load<Sprite>(cardData.ArtPath);

        //If failed look up the name of the card
        if (art == null)
        {
            art = Resources.Load<Sprite>(cardData.Name);
        }

        _cardArt.sprite = art;

        if (art == null)
        {
            _cardArt.color = Color.black;
        }
        else
        {
            _cardArt.color = Color.white;
        }

        SetCardFrame(cardData);
    }

    /// <summary>
    /// Set the UICard as if it is a display of an action choice.
    /// For example, it could be an action to activate an ability, in which case it should have the rules text for that specific ability.
    /// The rules text will be defined by the action type itself and not by this.
    /// </summary>
    /// <param name="action"></param>
    public void SetFromAction(CardGameAction action)
    {
        SetCardData(action.SourceCard);
        _cardRulesText.text = action.ToUIString();
    }

    public void SetCardData(CardInstance cardInstance)
    {

        if (cardInstance == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }

        _revealedToOwner = cardInstance.RevealedToOwner;

        //if revealed to all && zonetype is in the hand then show it
        var isInDeck = cardInstance.GetZone().ZoneType == ZoneType.Deck;
        var isInHand = cardInstance.GetZone().ZoneType == ZoneType.Hand;
        var isVisible = new List<ZoneType> { ZoneType.InPlay,ZoneType.Stack,ZoneType.Discard,ZoneType.Exile }.Contains(cardInstance.GetZone().ZoneType);
        var isOwnTurn = cardInstance.CardGame.ActivePlayer == cardInstance.GetOwner();

        var shouldSeeCard = isVisible || cardInstance.RevealedToAll || (isInDeck && _revealedToOwner && isOwnTurn) || (isInHand  && isOwnTurn);

        //Cards that are revealed to owner
        if (!shouldSeeCard)
        {
            _showAsUnknown = true;
            SetAsUnknownCard();
            return;
        }

        _showAsUnknown = false;
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
            _cardCombatStats.text = cardInstance.Power + " / " + (cardInstance.Toughness - cardInstance.DamageTaken);
        }
        else if (cardData is ManaCardData)
        {
            _cardCombatStats.gameObject.SetActive(false);
            _cardManaCost.gameObject.SetActive(false);
            _cardType.text += " - " + (cardData as ManaCardData).ManaAdded;
        }
        else if (cardData is ItemCardData)
        {
            _cardCombatStats.gameObject.SetActive(false);
        }

        _cardName.text = cardData.Name;
        _cardRulesText.text = cardInstance.RulesText;
        _cardManaCost.text = cardInstance.ManaCost;
        _cardType.text = cardData.CardType;

        Sprite art = Resources.Load<Sprite>(cardData.ArtPath);
        _cardArt.sprite = art;

        if (art == null)
        {
            _cardArt.color = Color.black;
        }
        else
        {
            _cardArt.color = Color.white;
        }

        SetCardFrame();
    }

    private void SetCardFrame(ICard cardData)
    {
        if (cardData.Colors == null || !cardData.Colors.Any())
        {
            //default to colorless frame;
            _cardFrame.sprite = colorlessCardFrame;
            return;
        }

        if (cardData.Colors.Count > 1)
        {
            //Do a multicolor frame.
            _cardFrame.sprite = multicolorCardFrame;
        }
        else
        {
            var color = cardData.Colors.First();

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

    public void SetCardData(ICard card)
    {
        //temporary hack while we figure out the best way to properly handle non CardInstance cards (i.e. like ones we would view in deck building)
        if (card is CardInstance)
        {
            SetCardData(card as CardInstance);
        }
        else
        {
            SetCardDataInternal(card);
        }
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public override void Highlight()
    {
        _highlight.gameObject.SetActive(true);
        _highlight.GetComponent<Image>().color = Color.green;
    }

    public override void Highlight(Color highlightColor)
    {
        _highlight.gameObject.SetActive(true);
        _highlight.GetComponent<Image>().color = highlightColor;
    }

    public override void StopHighlight()
    {
        _highlight.gameObject.SetActive(false);
    }
}
