﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This class represents a card as it exists inside the game state.
//It is essentially just a wrapper class around an existing card data.
public class CardInstance : CardGameEntity, ICard
{
    private BaseCardData _originalCardData;
    private BaseCardData _currentCardData;
    private int _ownerId;
    private bool _isSummoningSick = true;
    private bool _isExhausted = false; //exhausted will be our term for tapped
    private CardGame _cardGame;
    public IZone GetZone()
    {
        return _cardGame.GetZoneOfCard(this);
    }

    public Player GetOwner()
    {
        return _cardGame.GetOwnerOfCard(this);
    }

    #region Public Properties
    public BaseCardData CurrentCardData
    {
        get => _currentCardData;
        set
        {
            _currentCardData = value;

            if (_currentCardData is UnitCardData)
            {
                _powerWithoutMods = ((UnitCardData)_currentCardData).Power;
                _toughnessWithoutMods = ((UnitCardData)_currentCardData).Toughness;
            }
        }
    }

    public int OwnerId { get => _ownerId; set => _ownerId = value; }
    public override string Name { get => _currentCardData.Name; set => _currentCardData.Name = value; }

    public List<Effect> Effects { get => ((SpellCardData)_currentCardData).Effects; }

    public string RulesText
    {
        get
        {
            var str = "";
            if (_isExhausted && !(_currentCardData is SpellCardData))
            {
                str += "EXHAUSTED \r\n";
            }

            str += string.Join("\r\n", Abilities.Select(ab => ab.RulesText));



            if (_currentCardData is SpellCardData)
            {
                var additionalCostText = AdditionalCost != null ? $"Additional Cost : {AdditionalCost.RulesText}\r\n" : "";
                var abilitiesText = string.Join("\r\n", Abilities.Select(ab => ab.RulesText));
                var effectsText = string.Join("\r\n", Effects.Select(ef => ef.RulesText));
                str = additionalCostText + "\r\n" + abilitiesText + "\r\n" + effectsText;
            }

            if (Shields > 0)
            {
                str += $"\r\n {Shields} Shields";
            }

            str = str.Replace("#this", Name);

            return str;
        }
    }


    //NOTE - we do not have a rules text property here... seems weird why we don't.
    public List<CardColor> Colors { get => _currentCardData.Colors; }
    public string CreatureType
    {
        get
        {
            if (_currentCardData is UnitCardData)
            {
                return (_currentCardData as UnitCardData).CreatureType;
            }
            else
            {
                return "";
            }
        }
        set
        {
            if (_currentCardData is UnitCardData)
            {
                (_currentCardData as UnitCardData).CreatureType = value;
            }
        }

    }

    public string ManaCost
    {
        get
        {
            var modifiedCost = _currentCardData.ManaCost;

            var allCostModifiers = GetMods<IModifyManaCost>();

            foreach (var costModifier in allCostModifiers)
            {
                modifiedCost = costModifier.ModifyManaCost(_cardGame, this, modifiedCost);
            }
            return modifiedCost;
        }
    }

    public AdditionalCost AdditionalCost
    {
        get
        {
            var originalCost = CurrentCardData.AdditionalCost;
            var allCostModifiers = GetMods<IModifyAdditionalCost>();

            foreach (var costModifier in allCostModifiers)
            {
                originalCost = costModifier.ModifyAdditionalCost(_cardGame, this, originalCost);
            }
            return originalCost;
        }
    }

    public string Subtype => _currentCardData.Subtype;

    public string CardType { get => _currentCardData.CardType; }

    public bool IsSummoningSick { get => _isSummoningSick; set => _isSummoningSick = value; }
    public List<CardAbility> Abilities { get; set; } = new List<CardAbility>();

    //Hard coding in shield counters for now...
    public int Shields { get; set; }

    //Controls whether or not the card is revealed to the player. Only factors for cards in hidden information zones (i.e. the deck).
    public bool RevealedToOwner { get; set; } = false;
    public bool RevealedToAll { get; set; } = false;

    public List<Counter> Counters { get; set; } = new List<Counter>();
    public bool IsOfType<T>()
    {
        return CurrentCardData is T;
    }

    public bool IsOfType(string cardType)
    {
        return CurrentCardData.CardType.ToLower() == cardType.ToLower();
    }

    private int _powerWithoutMods;
    private int _toughnessWithoutMods;

    //Calculates the power, we also use this for switching power / toughness effects
    public int CalculatePower(bool applyPowerToughnessSwitchEffects)
    {
        if (_currentCardData is UnitCardData)
        {
            //return - power without mods, + mods power;
            int calculatedPower = _powerWithoutMods;

            var powerModifications = GetMods<IModifyPower>();

            //We have to ignore any switching power and toughness modifications or we could cause an infinite loop.
            if (!applyPowerToughnessSwitchEffects)
            {
                powerModifications = powerModifications.Where(mod => !(mod is ModSwitchPowerandToughness)).ToList();
            }

            foreach (var modification in powerModifications)
            {
                calculatedPower = modification.ModifyPower(_cardGame, this, calculatedPower);
            }

            return calculatedPower;
        }
        else
        {
            throw new Exception("Card Instance is not a creature, cannot access the power property");
        }
    }

    public int CalculateToughness(bool applyPowerToughnessSwitchEffects)
    {
        if (_currentCardData is UnitCardData)
        {
            //return - power without mods, + mods power;
            int calculatedToughness = _toughnessWithoutMods;

            //Add any modifications to the unit as well.
            var toughnessModifications = GetMods<IModifyToughness>();

            //We have to ignore any switching power and toughness modifications or we could cause an infinite loop.
            if (!applyPowerToughnessSwitchEffects)
            {
                toughnessModifications = toughnessModifications.Where(mod => !(mod is ModSwitchPowerandToughness)).ToList();
            }

            foreach (var modification in toughnessModifications)
            {
                calculatedToughness = modification.ModifyToughness(_cardGame, this, calculatedToughness);
            }
            return calculatedToughness;
        }
        else
        {
            throw new Exception("Card Instance is not a creature, cannot access the power property");
        }
    }

    //Temporary sort of unsafe properties for accessing Unit Power and Toughness,
    //While I figure out how I actually want to do this properly in a more type safe way.
    //I was casting CardInstance.CurrentCardData all over my code base anyways,
    //at least this keeps it in one place.
    public int Power
    {
        get
        {
            return CalculatePower(true);
        }
        set
        {
            if (_currentCardData is UnitCardData)
            {
                _powerWithoutMods = value;
            }
        }
    }

    public int Toughness
    {
        get
        {
            if (_currentCardData is UnitCardData)
            {
                return CalculateToughness(true);
            }
            else
            {
                throw new Exception("Card Instance is not a creature, cannot access the power property");
            }
        }
    }

    public int DamageTaken { get; set; }
    public int BaseToughness
    {
        get { return _toughnessWithoutMods; }
        set { _toughnessWithoutMods = value; }
    }

    public string ArtPath => _currentCardData.ArtPath;

    public CardGame CardGame { get => _cardGame; set => _cardGame = value; }
    public bool IsExhausted { get => _isExhausted; set => _isExhausted = value; }

    /// <summary>
    /// Variables for Double Sided Cards.
    /// </summary>
    public CardInstance BackCard { get; set; }
    public CardInstance FrontCard { get; set; }

    #endregion

    public CardInstance(CardGame cardGame, BaseCardData cardData)
    {
        _cardGame = cardGame;
        ContinuousEffects = new List<ContinuousEffect>();
        _originalCardData = cardData;
        _currentCardData = cardData.Clone();
        Abilities = _currentCardData.Abilities.ToList();

        //WIP - not finalized yet
        //We may need to treat split cards and double faced cards as instanced cards for them to properly work with the other systems.

        if (_currentCardData.BackCard != null)
        {
            BackCard = new CardInstance(cardGame, _currentCardData.BackCard);
            BackCard.FrontCard = this;
        }

        if (_currentCardData is UnitCardData)
        {
            var unitCardData = (UnitCardData)_currentCardData;
            _powerWithoutMods = unitCardData.Power;
            _toughnessWithoutMods = unitCardData.Toughness;
        }
    }

    public void TransformToCardData(BaseCardData cardData)
    {
        //We reset any continuous effects and abilities?
        ContinuousEffects = new List<ContinuousEffect>();
        Abilities = _currentCardData.Abilities.ToList();

        _currentCardData = cardData;

        //reset the back card if necessary.
        _currentCardData.BackCard = null;

        if (cardData is UnitCardData)
        {
            var unitCardData = (UnitCardData)_currentCardData;
            _powerWithoutMods = unitCardData.Power;
            _toughnessWithoutMods = unitCardData.Toughness;
        }
    }

    #region Public Methods

    public void SetCardData(BaseCardData cardData)
    {
        _currentCardData = cardData.Clone();
        Abilities = _currentCardData.Abilities.ToList();
        if (_currentCardData is UnitCardData)
        {
            var unitCardData = (UnitCardData)_currentCardData;
            _powerWithoutMods = unitCardData.Power;
            _toughnessWithoutMods = unitCardData.Toughness;
        }
    }

    public List<T> GetAbilitiesAndComponents<T>()
    {

        var abilities = Abilities.Where(a => a is T).Cast<T>().ToList();

        //also look for ability components

        var abilityComponents = Abilities.SelectMany(a => a.Components).Where(c => c is T).Cast<T>().ToList();

        return abilities.Union(abilityComponents).ToList();
    }
    /// <summary>
    /// Returns anything on the card instance which could modify a property of the card.
    /// Could be an ability, an ability component or a Modification
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public List<T> GetMods<T>()
    {
        return GetAbilitiesAndComponents<T>().Union(Modifications.GetOfType<T>()).Union(Counters.GetOfType<T>()).ToList();
    }

    /// <summary>
    /// Gets all available things that can be done with the card
    /// For example, is it castable, can we activate an ability on it?
    /// </summary>
    /// <returns></returns>
    public List<CardGameAction> GetAvailableActions()
    {
        var actions = new List<CardGameAction>();

        if (CardGame.CanPlayCard(this))
        {
            //Need to check the type of card it is and create an associated action
            actions.Add(CardGameAction.CreatePlayCardAction(this));
        }
        if (BackCard != null)
        {
            if (CardGame.CanPlayCard(BackCard))
            {
                actions.Add(CardGameAction.CreatePlayCardAction(BackCard));
            }
        }

        foreach (var ability in Abilities.GetOfType<ICastModifier>())
        {
            //_cardGame.Log("Checking how many cast modifiers this has?");
            //_cardGame.Log($"{Abilities.GetOfType<ICastModifier>().Count()}");
            //Buyback TODO - need to check if we can actually play this card.
            var cardWithModifierAction = CardGameAction.CreatePlayCardAction(this, ability);
            //cardWithModifierAction.CastModifiers.Add(ability);

            if (cardWithModifierAction.IsValidAction(_cardGame))
            {
                actions.Add(cardWithModifierAction);
            }
        }



        foreach (var ability in GetActivatedAbilities())
        {
            if (_cardGame.ActivatedAbilitySystem.CanActivateAbility(this, ability))
            {
                actions.Add(CardGameAction.CreateAction(this, ability));
            }
        }

        return actions;
    }

    public CardInstance ShallowClone()
    {
        return (CardInstance)this.MemberwiseClone();
    }

    public List<ActivatedAbility> GetActivatedAbilities()
    {
        return Abilities.GetOfType<ActivatedAbility>();
    }

    public bool HasMultipleOptions()
    {
        //First we need to get all the possible options for this card.
        //Filter them by if its doable
        //Can we pay the mana cost
        //Can we pay the additional cost

        return GetAvailableActions().Count() > 1;

    }

    public bool HasActivatedAbility()
    {
        return GetAbilitiesAndComponents<ActivatedAbility>().Any();
    }

    public void AddModification(Modification mod)
    {
        Modifications.Add(mod);
    }
    #endregion
}

