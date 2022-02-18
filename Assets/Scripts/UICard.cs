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




    public void SetFromCardData(BaseCardData cardData)
    {
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

        Debug.Log(cardData.ArtPath);
        Sprite art = Resources.Load<Sprite>(cardData.ArtPath);
        Debug.Log(art);
        cardArtRenderer.sprite = art;
      

        /* TextAsset asset = Resources.Load(imageName) as TextAsset;
        byte[] data = asset.bytes;
        Texture2D texture = new Texture2D(128, 128, TextureFormat.ARGB32, false);
        texture.LoadImage(data);
        texture.name = imageName;
        Sprite icon = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        SpriteRenderer iconRenderer = GetComponent<SpriteRenderer>();
        iconRenderer.sprite = icon;*/
    }
}
