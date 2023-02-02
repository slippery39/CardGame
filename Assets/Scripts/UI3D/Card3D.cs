using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Card3D : MonoBehaviour
{
    [SerializeField] private GameObject _cardMesh;
    [SerializeField] private Renderer _cardRenderer;

    [SerializeField] private TextMeshPro _name;
    [SerializeField] private TextMeshPro _combatStats;
    [SerializeField] private TextMeshPro _manaCost;
    [SerializeField] private TextMeshPro _cardType;
    [SerializeField] private TextMeshPro _rulesText;
    [SerializeField] private Material _cardArtMaterial;


    private void Awake()
    {
        _cardRenderer = _cardMesh.GetComponent<MeshRenderer>();
    }

    public Bounds GetBounds()
    {
        return _cardMesh.GetComponent<MeshRenderer>().bounds;
    }

    public void SetCardInfo(BaseCardData card, bool castShadows = true)
    {
        var cardOptions = new Card3DOptions
        {
            CardName = card.Name,
            ManaCost = card.ManaCost,
            CardType = card.CardType,
            RulesText = card.RulesText,
            CastShadows = castShadows,
            ArtTexture = Resources.Load<Texture2D>(card.ArtPath)
        };

        var unitCard = card as UnitCardData;
        if (unitCard != null)
        {
            cardOptions.CombatStats = unitCard.Power + "/" + unitCard.Toughness;
        }

        SetCardInfo(cardOptions);
    }

    public void SetCardInfo(CardInstance card, bool castShadows = true)
    {
        var cardOptions = new Card3DOptions
        {
            CardName = card.Name,
            ManaCost = card.ManaCost,
            CardType = card.CardType,
            RulesText = card.RulesText,
            CombatStats = card.Power + " / " + card.Toughness,
            CastShadows = castShadows,
            ArtTexture = Resources.Load<Texture2D>(card.ArtPath)
        };

        SetCardInfo(cardOptions);
    }

    public void SetCardInfo(Card3D otherCard, bool castShadows = true)
    {
        var cardOptions = new Card3DOptions
        {
            CardName = otherCard._name.text,
            ManaCost = otherCard._manaCost.text,
            CardType = otherCard._cardType.text,
            RulesText = otherCard._rulesText.text,
            CombatStats = otherCard._combatStats.text,
            ArtTexture = (Texture2D)otherCard._cardRenderer.materials[3].GetTexture("_BaseMap"),
            CastShadows = castShadows
        };

        SetCardInfo(cardOptions);
    }

    public void SetCardInfo(Card3DOptions cardOptions)
    {
        _name.text = cardOptions.CardName;
        _manaCost.text = cardOptions.ManaCost;
        _cardType.text = cardOptions.CardType;
        _rulesText.text = cardOptions.RulesText;
        _combatStats.text = cardOptions.CombatStats;

        SetArt(cardOptions.ArtTexture);

        if (cardOptions.CastShadows)
        {
            _cardMesh.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        }
        else
        {
            _cardMesh.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    private void SetArt(Texture2D artTexture)
    {        
        _cardRenderer.materials[3].SetTexture("_BaseMap", artTexture);
    }
}

public class Card3DOptions
{
    public string ManaCost { get; set; }
    public string CardName { get; set; }
    public string CardType { get; set; }
    public string RulesText { get; set; }
    public string CombatStats { get; set; }
    public Texture2D ArtTexture{ get; set; }
    public bool CastShadows { get; set; } = true;
}
