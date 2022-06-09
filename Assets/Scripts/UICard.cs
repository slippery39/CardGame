using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UICard : UIGameEntity
{
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

    #endregion

    public void SetCardData(CardInstance cardInstance)
    {
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

        //Warning: we might want to update this to the CardInstanceAttributes..
        _cardNameText.text = cardData.Name;
        _cardRulesText.text = cardData.RulesText;
        _cardManaText.text = cardInstance.ManaCost;
        _cardTypeText.text = cardData.CardType;

        if (cardData is UnitCardData)
        {
            _cardCombatStatsText.text = cardInstance.Power + " / " + cardInstance.Toughness;
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
                default: _cardFrame.sprite = colorlessCardFrame;break;
            }
            
        }
    }

    public override void Highlight()
    {
        _highlight.gameObject.SetActive(true);
    }

    public override void StopHighlight()
    {
        _highlight.gameObject.SetActive(false);
    }
}
