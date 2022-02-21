using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICard : MonoBehaviour
{
    //Unity Fields
    [SerializeField]
    private TextMeshPro cardNameText;
    [SerializeField]
    private TextMeshPro cardRulesText;
    [SerializeField]
    private TextMeshPro cardCombatStatsText;
    [SerializeField]
    private TextMeshPro cardManaText;
    [SerializeField]
    private TextMeshPro cardTypeText;
    [SerializeField]
    private SpriteRenderer cardArtRenderer;

    private BaseCardData _cardData;
    #region Public Properties
    public SpriteRenderer CardArtRenderer { get => cardArtRenderer; set => cardArtRenderer = value; }

    #endregion



    //TODO - could also have a SetFromCardInstance method which could do fancier stuff?
    public void SetFromCardData(BaseCardData cardData)
    {

        if (cardData is SpellCardData)
        {
            cardCombatStatsText.gameObject.SetActive(false);
        }

        _cardData = cardData;

        cardNameText.text = cardData.Name;
        cardRulesText.text = cardData.RulesText;
        cardManaText.text = cardData.ManaCost;
        cardTypeText.text = cardData.CardType;

        if (cardData is UnitCardData)
        {
            UnitCardData unitCardData = (UnitCardData)cardData;
            cardCombatStatsText.text = unitCardData.Power + " / " + unitCardData.Toughness;
        }
        Sprite art = Resources.Load<Sprite>(cardData.ArtPath);
        cardArtRenderer.sprite = art;
    }
}
