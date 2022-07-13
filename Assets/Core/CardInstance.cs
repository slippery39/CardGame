using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DefaultContinousEffectSystem;

//This class represents a card as it exists inside the game state.
//It is essentially just a wrapper class around an existing card data.
public class CardInstance : CardGameEntity
{
    private BaseCardData _originalCardData;
    private BaseCardData _currentCardData;
    private int _ownerId;
    private bool _isSummoningSick = true;
    private CardGame _cardGame;

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

    //Testing out this method for getting the card data
    public T GetCardData<T>() where T : BaseCardData
    {
        return _currentCardData as T;
    }

    public int OwnerId { get => _ownerId; set => _ownerId = value; }
    public override string Name { get => _currentCardData.Name; set => _currentCardData.Name = value; }

    public List<Effect> Effects { get => ((SpellCardData)_currentCardData).Effects; }

    public string RulesText
    {
        get
        {
            var str = string.Join("\r\n", Abilities.Select(ab => ab.RulesText)).Replace("#this#", Name);

            if (_currentCardData is SpellCardData)
            {
                var spellData = _currentCardData as SpellCardData;
                str += string.Join("\r\n", spellData.Effects.Select(ab => ab.RulesText)).Replace("#this#", Name);
            }

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
            var originalCost = _currentCardData.ManaCost;

            var allCostModifiers = GetAbilities<IModifyManaCost>();

            foreach (var costModifier in allCostModifiers)
            {
                originalCost = costModifier.ModifyManaCost(_cardGame, this, originalCost);
            }

            //TODO Have the cost reduction effects also use IModifyManaCost
            var costAsCounts = new Mana(originalCost);
            var allCostReductions = ContinuousEffects.SelectMany(ce => ce.SourceAbility.Effects).Where(ab => ab is StaticManaReductionEffect).Cast<StaticManaReductionEffect>();

            foreach (var effect in allCostReductions)
            {
                var costAsDict = new Mana(effect.ReductionAmount);

                costAsCounts.ColorlessMana -= costAsDict.ColorlessMana;
                costAsCounts.ColorlessMana = Math.Max(0, costAsCounts.ColorlessMana);

                foreach (var essenceType in costAsDict.ColoredMana)
                {
                    costAsCounts.ColoredMana[essenceType.Key] -= essenceType.Value;
                    costAsCounts.ColoredMana[essenceType.Key] = Math.Max(0, costAsCounts.ColoredMana[essenceType.Key]);
                }
            }

            var modifiedCost = costAsCounts.ToManaString();
            return modifiedCost;
        }
    }

    public AdditionalCost AdditionalCost
    {
        get
        {
            var originalCost = CurrentCardData.AdditionalCost;
            var allCostModifiers = GetAbilities<IModifyAdditionalCost>();

            foreach (var costModifier in allCostModifiers)
            {
                originalCost = costModifier.ModifyAdditionalCost(_cardGame, this, originalCost);
            }

            if (Name == "Goblin Grenade")
            {
                var test = 1;
            }

            return originalCost;
        }
    }


    public string CardType { get => _currentCardData.CardType; }

    public bool IsSummoningSick { get => _isSummoningSick; set => _isSummoningSick = value; }
    public List<CardAbility> Abilities { get; set; } = new List<CardAbility>();


    //How do we figure this out?
    public List<ContinuousEffect> ContinuousEffects { get; set; }

    public List<Modification> Modifications { get; set; } = new List<Modification>();

    public bool IsOfType<T>()
    {
        return CurrentCardData is T;
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

            var pumpContinuousEffects = ContinuousEffects.SelectMany(e => e.SourceAbility.Effects).Where(e => e is StaticPumpEffect).Cast<StaticPumpEffect>();
            if (pumpContinuousEffects.Any())
            {
                foreach (var pumpEffect in pumpContinuousEffects)
                {
                    calculatedPower += pumpEffect.Power;
                }
            }


            var powerModifications = Modifications.GetOfType<IModifyPower>();

            //We have to ignore any switching power and toughness modifications or we could cause an infinite loop.
            if (!applyPowerToughnessSwitchEffects)
            {
                powerModifications = powerModifications.Where(mod => !(mod is ModSwitchPowerandToughness)).ToList();
            }

            foreach (var modification in powerModifications)
            {
                //Null is temporary, we haven't set everything up to pass in the CardGame yet.
                calculatedPower = modification.ModifyPower(null, this, calculatedPower);
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

            var pumpContinuousEffects = ContinuousEffects.SelectMany(e => e.SourceAbility.Effects).Where(e => e is StaticPumpEffect).Cast<StaticPumpEffect>();
            if (pumpContinuousEffects.Any())
            {
                foreach (var pumpEffect in pumpContinuousEffects)
                {
                    calculatedToughness += pumpEffect.Toughness;
                }
            }

            //Add any modifications to the unit as well.
            var toughnessModifications = Modifications.GetOfType<IModifyToughness>();

            //We have to ignore any switching power and toughness modifications or we could cause an infinite loop.
            if (!applyPowerToughnessSwitchEffects)
            {
                toughnessModifications = toughnessModifications.Where(mod => !(mod is ModSwitchPowerandToughness)).ToList();
            }

            foreach (var modification in toughnessModifications)
            {
                calculatedToughness = modification.ModifyToughness(null, this, calculatedToughness);
            }

            calculatedToughness = calculatedToughness - DamageTaken;

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
                //return - power without mods, + mods power;
                int calculatedToughness = _toughnessWithoutMods;

                var pumpContinuousEffects = ContinuousEffects.SelectMany(e => e.SourceAbility.Effects).Where(e => e is StaticPumpEffect).Cast<StaticPumpEffect>();
                if (pumpContinuousEffects.Any())
                {
                    foreach (var pumpEffect in pumpContinuousEffects)
                    {
                        calculatedToughness += pumpEffect.Toughness;
                    }
                }

                //Add any modifications to the unit as well.

                var toughnessModifications = Modifications.GetOfType<IModifyToughness>();

                foreach (var modification in toughnessModifications)
                {
                    calculatedToughness = modification.ModifyToughness(null, this, calculatedToughness);
                }
                return calculatedToughness;
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

    #endregion

    public CardInstance(CardGame cardGame, BaseCardData cardData)
    {
        _cardGame = cardGame;
        ContinuousEffects = new List<ContinuousEffect>();
        _originalCardData = cardData;
        _currentCardData = cardData.Clone();
        Abilities = _currentCardData.Abilities.ToList();

        if (_currentCardData is UnitCardData)
        {
            var unitCardData = (UnitCardData)_currentCardData;
            _powerWithoutMods = unitCardData.Power;
            _toughnessWithoutMods = unitCardData.Toughness;
        }
    }

    #region Public Methods

    public List<T> GetAbilities<T>()
    {

        var abilities = Abilities.Where(a => a is T).Cast<T>().ToList();

        //also look for ability components

        var abilityComponents = Abilities.SelectMany(a => a.Components).Where(c => c is T).Cast<T>().ToList();

        return abilities.Union(abilityComponents).ToList();
    }

    public bool HasActivatedAbility()
    {
        return GetAbilities<ActivatedAbility>().Any();
    }

    public void AddModification(Modification mod)
    {
        Modifications.Add(mod);
    }
    #endregion
}

