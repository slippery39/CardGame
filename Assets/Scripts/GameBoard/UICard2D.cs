using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UICard2D : MonoBehaviour, IUICard, IHighlightable
{
    [Header("Debugging")]
    [SerializeField]
    private bool _revealedToOwner;

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

    //TODO - This should not be in here... the UICard should be a dumb container.
    //private CardInstance _cardInstance;

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
        //TODO - How are we going to set this?
        // EntityId = -1;
        _frontOfCard.gameObject.SetActive(false);
        _backOfCard.gameObject.SetActive(true);

        var uiGameEntity = GetComponent<UIGameEntity>();
        if (uiGameEntity != null)
        {
            uiGameEntity.EntityId =-1;
        }
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
        SetArt(cardData.ArtPath);
        SetCardFrame(cardData.Colors);
    }

    private void SetArt(string artPath)
    {
        Sprite art = Resources.Load<Sprite>(artPath);
        _cardArt.sprite = art;

        if (art == null)
        {
            _cardArt.color = Color.black;
        }
        else
        {
            _cardArt.color = Color.white;
        }
    }

    /// <summary>
    /// Set the UICard as if it is a display of an action choice.
    /// For example, it could be an action to activate an ability, in which case it should have the rules text for that specific ability.
    /// The rules text will be defined by the action type itself and not by this.
    /// </summary>
    /// <param name="action"></param>
    public void SetFromAction(CardGameAction action)
    {
        //SetCardData(action.SourceCard);
        if (action.CardToPlay != null)
        {
            //TODO - need a way to clone the card?
            var cardForAction = action.CardToPlay.ShallowClone();
            cardForAction.Abilities = cardForAction.Abilities.Where(
                ab =>
                {
                    var actAb = ab as ActivatedAbility;
                    if (actAb != null)
                    {
                        if (actAb.ActivationZone == cardForAction.GetZone().ZoneType)
                        {
                            return false;
                        }
                    }
                    return true;
                })
                .ToList();
            //This will not work for multiple cast modifiers.
            if (action.CastModifiers.IsNullOrEmpty())
            {
                cardForAction.Abilities = cardForAction.Abilities.Where(ab => !(ab is ICastModifier)).ToList();
            }

            SetCardData(cardForAction);
            //TODO - we need to hide all cast zone related activated abilities 
            // i.e. cycling should not show on the card if its being cast normally
            
            //TODO - hide all CastModifier related abilities.
        }
        else //A non casting action, i.e. activated ability or something else.
        {
            //Hide all unrelated objects
            _cardType.gameObject.SetActive(false);
            _cardCombatStats.gameObject.SetActive(false);
            _cardName.gameObject.SetActive(false);

            SetArt(action.SourceCard.ArtPath);
            _cardRulesText.text = action.ToUIString();
        }
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
            if (cardInstance.GetZone().ZoneType == ZoneType.InPlay)
            {
                var debug = 0;
            }
            SetAsUnknownCard();
            return;
        }

        _backOfCard.gameObject.SetActive(false);
        _frontOfCard.gameObject.SetActive(true);

        //TODO - Refaftor - EntityID will not be stored here anymore, figure out where to put it.
        //EntityId = cardInstance.EntityId;

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

        SetArt(cardInstance.ArtPath);
        SetCardFrame(cardInstance.Colors);

        //Set the EntityId if available

        var entityComponent = GetComponent<UIGameEntity>();
        if (entityComponent != null)
        {
            entityComponent.EntityId= cardInstance.EntityId;
        }

    }

    private void SetCardFrame(List<CardColor> colors)
    {
        if (colors.IsNullOrEmpty())
        {
            _cardFrame.sprite = colorlessCardFrame;
            return;
        }
        else if (colors.Count > 1)
        {
            //Do a multicolor frame.
            _cardFrame.sprite = multicolorCardFrame;
        }
        else
        {
            var color = colors.First();

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

    public void Highlight()
    {
        _highlight.gameObject.SetActive(true);
        _highlight.GetComponent<Image>().color = Color.green;
    }

    public void Highlight(Color highlightColor)
    {
        _highlight.gameObject.SetActive(true);
        _highlight.GetComponent<Image>().color = highlightColor;
    }

    public void StopHighlight()
    {
        _highlight.gameObject.SetActive(false);
    }
}
