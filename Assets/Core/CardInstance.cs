using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This class represents a card as it exists inside the game state.
//It is essentially just a wrapper class around an existing card data.
public class CardInstance : CardGameEntity
{
    private BaseCardData _originalCardData;
    private BaseCardData _currentCardData;
    private int _ownerId;
    private bool _isSummoningSick = true;

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

    public string RulesText => string.Join("\r\n", Abilities.Select(ab => ab.RulesText)).Replace("#this#", Name);

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

            var costAsCounts = new ManaAndEssence(originalCost);

            var allCostReductions = ContinuousEffects.SelectMany(ce => ce.SourceAbility.Effects).Where(ab => ab is StaticManaReductionEffect).Cast<StaticManaReductionEffect>();

            foreach (var effect in allCostReductions)
            {
                var costAsDict = new ManaAndEssence(effect.ReductionAmount);

                costAsCounts.Mana -= costAsDict.Mana;
                costAsCounts.Mana = Math.Max(0, costAsCounts.Mana);

                foreach (var essenceType in costAsDict.Essence)
                {
                    costAsCounts.Essence[essenceType.Key] -= essenceType.Value;
                    costAsCounts.Essence[essenceType.Key] = Math.Max(0, costAsCounts.Essence[essenceType.Key]);
                }
            }

            var modifiedCost = costAsCounts.ToManaString();
            return modifiedCost;
        }
    }

    public string CardType { get => _currentCardData.CardType; }

    public bool IsSummoningSick { get => _isSummoningSick; set => _isSummoningSick = value; }
    public List<CardAbility> Abilities { get; set; } = new List<CardAbility>();


    //How do we figure this out?
    public List<ContinuousEffect> ContinuousEffects { get; set; }

    public List<ModAddToPowerToughness> Modifications { get; set; } = new List<ModAddToPowerToughness>();


    private int _powerWithoutMods;
    private int _toughnessWithoutMods;

    //Temporary sort of unsafe properties for accessing Unit Power and Toughness,
    //While I figure out how I actually want to do this properly in a more type safe way.
    //I was casting CardInstance.CurrentCardData all over my code base anyways,
    //at least this keeps it in one place.
    public int Power
    {
        get
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
                foreach (var modification in Modifications)
                {
                    calculatedPower += modification.Power;
                }

                return calculatedPower;
            }
            else
            {
                throw new Exception("Card Instance is not a creature, cannot access the power property");
            }
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

                foreach (var modification in Modifications)
                {
                    calculatedToughness += modification.Toughness;
                }

                calculatedToughness = calculatedToughness - DamageTaken;

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

    public CardInstance(BaseCardData cardData)
    {
        ContinuousEffects = new List<ContinuousEffect>();        
        _originalCardData = cardData;
        _currentCardData = cardData.Clone();
        Abilities = _currentCardData.Abilities;

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
        return Abilities.Where(a => a is T).Cast<T>().ToList();
    }

    public bool HasActivatedAbility()
    {
        return GetAbilities<ActivatedAbility>().Any();
    }

    public void AddModification(ModAddToPowerToughness mod)
    {
        Modifications.Add(mod);
    }
    #endregion
}

