using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System;

public class Card3D : MonoBehaviour, IHighlightable
{
    [SerializeField] private GameObject _card3D; 
    [SerializeField] private GameObject _cardMesh;
    [SerializeField] private Renderer _cardRenderer;
    [SerializeField] private GameObject _unknownCard;

    [SerializeField] private TextMeshPro _name;
    [SerializeField] private TextMeshPro _combatStats;
    [SerializeField] private TextMeshPro _manaCost;
    [SerializeField] private TextMeshPro _cardType;
    [SerializeField] private TextMeshPro _rulesText;
    [SerializeField] private GameObject _cardTextContainer;
    [SerializeField] private Material _cardArtMaterial;

    [SerializeField] private GameObject _highlightObject;

    [SerializeField] private CardFrameTextures _cardFrameTextures;

    [Header("Materials For Animation")]
    [SerializeField] private Card3DMaterialSet _dissolveMaterials;
    [SerializeField] private Card3DMaterialSet _summonMaterials;

    [SerializeField] [ColorUsage(true, true)] private Color _defaultHighlightColor = new Color(1 * 3, 1 * 3, 1);
    [SerializeField] [ColorUsage(true, true)] private Color _greenHighlightColor = new Color(1 * 3, 1 * 3, 1);
    [SerializeField] [ColorUsage(true, true)] private Color _redHighlightColor = new Color(1 * 3, 1 * 3, 1);


    private void Awake()
    {
        _cardRenderer = _cardMesh.GetComponent<MeshRenderer>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(DissolveCo());
        }
    }
    public Bounds GetBounds()
    {
        return _cardMesh.GetComponent<MeshRenderer>().bounds;
    }

    public void SetCardInfo(CardGameAction action)
    {
        var cardOptions = new Card3DOptions
        {
            CardName = action.SourceCard?.Name,
            ManaCost = action.ManaCost, //TODO - actions currently don't have a mana cost?
            CardType = action.ActionType,
            RulesText = action.ToUIString(),
            CastShadows = false,
            ArtTexture = Resources.Load<Texture2D>(action.SourceCard.ArtPath),
            CardFrameTexture = GetCardFrameTexture(action.SourceCard.Colors),
        };

        if (action is PlayUnitAction)
        {
            cardOptions.CombatStats = action.SourceCard.Power + "/" + action.SourceCard.Toughness;
        }

        //TODO - need to manually figure out the mana cost and combat stats

        SetCardInfo(cardOptions);
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
            ArtTexture = Resources.Load<Texture2D>(card.ArtPath),
            CardFrameTexture = GetCardFrameTexture(card.Colors),
        };

        var unitCard = card as UnitCardData;
        if (unitCard != null)
        {
            cardOptions.CombatStats = unitCard.Power + "/" + unitCard.Toughness;
        }
        else
        {
            cardOptions.CombatStats = "";
        }

        SetCardInfo(cardOptions);
    }

    public void SetCardInfo(CardInstance card, bool castShadows = true, bool addEntityComponent = true)
    {
        var cardOptions = new Card3DOptions
        {
            CardName = card.Name,
            ManaCost = card.ManaCost,
            CardType = card.CardType,
            RulesText = card.RulesText,
            CombatStats = card.Power + " / " + (card.Toughness - card.DamageTaken),
            CastShadows = castShadows,
            ArtTexture = Resources.Load<Texture2D>(card.ArtPath),
            CardFrameTexture = GetCardFrameTexture(card.Colors)
        };

        if (card.CurrentCardData is not UnitCardData)
        {
            cardOptions.CombatStats = "";
        }

        if (addEntityComponent)
        {
            var entityComp = UIGameEntity3D.AddToCard3D(this, card);
            if (entityComp.EntityId == -1)
            {
                SetAsUnknown();
                return;
            }
        }

        SetCardInfo(cardOptions);
    }

    private void SetAsUnknown()
    {
        _unknownCard.SetActive(true);
        _card3D.SetActive(false);
    }

    private Texture2D GetCardFrameTexture(List<CardColor> colors)
    {
        if (colors.IsNullOrEmpty())
        {
            return _cardFrameTextures.Colorless;
        }
        else if (colors.Count > 1)
        {
            //Do a multicolor frame.
            return _cardFrameTextures.MultiColor;
        }
        else
        {
            var color = colors[0];

            //Do a single color frame.
            switch (color)
            {
                case CardColor.White: return _cardFrameTextures.White;
                case CardColor.Blue: return _cardFrameTextures.Blue;
                case CardColor.Green: return _cardFrameTextures.Green;
                case CardColor.Red: return _cardFrameTextures.Red;
                case CardColor.Black: return _cardFrameTextures.Black;
                case CardColor.Colorless: return _cardFrameTextures.Colorless;
                default: return _cardFrameTextures.Colorless;
            }
        }
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
            CardFrameTexture = (Texture2D)otherCard._cardRenderer.materials[0].GetTexture("_Albedo"),
            ArtTexture = (Texture2D)otherCard._cardRenderer.materials[3].GetTexture("_Albedo"),
            CastShadows = castShadows
        };

        SetCardInfo(cardOptions);
    }

    public void SetCardInfo(Card3DOptions cardOptions)
    {
        _unknownCard.SetActive(false);
        _card3D.SetActive(true);
        //Show anything that was hidden before
        this._rulesText.gameObject.SetActive(true);
        this._manaCost.gameObject.SetActive(true);
        this._name.gameObject.SetActive(true);
        this._cardType.gameObject.SetActive(true);
        this._combatStats.gameObject.SetActive(true);
        this._cardTextContainer.gameObject.SetActive(true);

        //Reset any Dissolve Amounts
        for (var i = 0; i < _cardRenderer.materials.Length; i++)
        {
            var material = _cardRenderer.materials[i];
            //Reset the dissolve amounts
            material.SetFloat("_DissolveAmount", 0);
        }

        _name.text = cardOptions.CardName;
        _manaCost.text = cardOptions.ManaCost;
        _cardType.text = cardOptions.CardType;
        _rulesText.text = cardOptions.RulesText;

        if (cardOptions.CombatStats == null || cardOptions.CombatStats.Trim() == "")
        {
            _combatStats.gameObject.SetActive(false);
        }
        else
        {
            _combatStats.gameObject.SetActive(true);
        }
        _combatStats.text = cardOptions.CombatStats;

        SetCardFrame(cardOptions.CardFrameTexture);
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
        _cardRenderer.materials[3].SetTexture("_Albedo", artTexture);
    }
    private void SetCardFrame(Texture2D cardFrameTexture)
    {
        _cardRenderer.materials[0].SetTexture("_Albedo", cardFrameTexture);
    }

    public void SetMaterials(Card3DMaterialSet materials)
    {
        var cardFrameTexture = (Texture2D)_cardRenderer.materials[0].GetTexture("_Albedo");
        var artTexture = (Texture2D)_cardRenderer.materials[3].GetTexture("_Albedo");
        _cardRenderer.SetMaterials(materials.ToList());
        SetArt(artTexture);
        SetCardFrame(cardFrameTexture);
    }

    public void PlaySummon(Action onComplete = null)
    {
        SetMaterials(_summonMaterials);
        StartCoroutine(SummonCo(onComplete));
    }
    private IEnumerator SummonCo(Action onComplete = null)
    {
        //Hide Everything to run the dissolve animation
        _rulesText.gameObject.SetActive(false);
        _manaCost.gameObject.SetActive(false);
        _name.gameObject.SetActive(false);
        _combatStats.gameObject.SetActive(false);
        _cardType.gameObject.SetActive(false);
        _cardTextContainer.gameObject.SetActive(false);

        for (var t = 0f; t < 1.01; t += 0.05f)
        {
            for (var i = 0; i < _cardRenderer.materials.Length; i++)
            {
                var material = _cardRenderer.materials[i];
                material.SetFloat("_DissolveAmount", 1 - t);
            }
            yield return new WaitForSeconds(0.01f);
        }

        Debug.Log("Dissolve Completed!");
        if (onComplete != null)
        {
            onComplete();
        }
    }

    public void PlayDissolve(Action onComplete = null)
    {
        SetMaterials(_dissolveMaterials);
        StartCoroutine(DissolveCo(onComplete));
    }

    private IEnumerator DissolveCo(Action onComplete = null)
    {
        //Hide Everything to run the dissolve animation
        _rulesText.gameObject.SetActive(false);
        _manaCost.gameObject.SetActive(false);
        _name.gameObject.SetActive(false);
        _combatStats.gameObject.SetActive(false);
        _cardType.gameObject.SetActive(false);
        _cardTextContainer.gameObject.SetActive(false);

        for (var t = 0f; t < 1.01; t += 0.05f)
        {
            for (var i = 0; i < _cardRenderer.materials.Length; i++)
            {
                var material = _cardRenderer.materials[i];
                material.SetFloat("_DissolveAmount", t);
            }
            yield return new WaitForSeconds(0.01f);
        }

        Debug.Log("Dissolve Completed!");
        if (onComplete != null)
        {
            onComplete();
        }
    }

    public void Highlight()
    {
        this._highlightObject.SetActive(true);
        var materials = this._highlightObject.GetComponent<Renderer>().materials;
        materials.Last().SetColor("_Color", _defaultHighlightColor);
    }

    public void Highlight(Color highlightColor)
    {
        this._highlightObject.SetActive(true);
        var materials = this._highlightObject.GetComponent<Renderer>().materials;
        materials.Last().SetColor("_Color", highlightColor.ToHDR(6));
    }

    public void StopHighlight()
    {
        this._highlightObject.SetActive(false);
    }
}

public class Card3DOptions
{
    public string ManaCost { get; set; }
    public string CardName { get; set; }
    public string CardType { get; set; }
    public string RulesText { get; set; }
    public string CombatStats { get; set; }
    public Texture2D CardFrameTexture { get; set; }
    public Texture2D ArtTexture { get; set; }
    public bool CastShadows { get; set; } = true;
}


[System.Serializable]
public class CardFrameTextures
{
    public Texture2D White;
    public Texture2D Blue;
    public Texture2D Black;
    public Texture2D Red;
    public Texture2D Green;
    public Texture2D Colorless;
    public Texture2D MultiColor;
}

[System.Serializable]
public class Card3DMaterialSet
{
    public Material art;
    public Material cardFrame;
    public Material cardSides;
    public Material cardBack;

    public List<Material> ToList()
    {
        return new List<Material> { cardFrame, cardBack, cardSides, art };
    }
}
