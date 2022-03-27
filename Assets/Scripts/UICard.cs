using System.Collections;
using System.Collections.Generic;
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
        }
        //in case it has already been hidden previously.
        else if (cardData is UnitCardData)
        {
            _cardCombatStatsText.gameObject.SetActive(true);

        }

        _cardNameText.text = cardData.Name;
        _cardRulesText.text = cardData.RulesText;
        _cardManaText.text = cardData.ManaCost;
        _cardTypeText.text = cardData.CardType;

        if (cardData is UnitCardData)
        {
            UnitCardData unitCardData = (UnitCardData)cardData;
            _cardCombatStatsText.text = unitCardData.Power + " / " + unitCardData.Toughness;
        }
        Sprite art = Resources.Load<Sprite>(cardData.ArtPath);
        _cardArtRenderer.sprite = art;
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
