using System;
using System.Collections.Generic;
using System.Linq;
//Other creatures you control get +1/+1
//This creature gets +0/+2 as long as its your turn
//Lhurgoyf's Power and Toughness are equal to the number of creatures in all graveyards (or boneyard wurm)


/*
public class TargetInfo
{
    TargetType TargetType { get; set; } //needs target, does not need target??
    CardFilter Filter { get; set; } //goblin - It needs to target a goblin
}
*/

/*
interface ITargetInfo
{
    public TargetType TargetType { get; set; } - TargetType.NoTarget;
    public CardFilter CardFilter { get; set; } 
}
*/

public enum EntityType
{
    Self,
    OtherCreaturesYouControl,
    CardsInHand
}


public class EntitiesAffectedInfo
{
    //Entities to Affect
    public EntityType EntitiesAffected { get; set; }
    public CardFilter Filter { get; set; }
}

//How do we handle spells that have targets but affect other entities?

public class StaticAbility : CardAbility
{
    public override string RulesText
    {
        get
        {
            switch (EntitiesAffected)
            {
                case EntityType.Self:
                    return $"This {string.Join(" and ", Effects.Select(e => e.RulesText))}";
                case EntityType.OtherCreaturesYouControl:
                    {
                        var startOfText = Filter?.CreatureType == null ? "Other creatures" : $"Other {Filter.CreatureType}s";
                        return $"{startOfText} you control {string.Join(" and ", Effects.Select(e => e.RulesText))}";
                    }
                case EntityType.CardsInHand:
                    {
                        var startOfText = Filter?.CreatureType == null ? " Cards " : $"{Filter.CreatureType} cards";
                        return $@"{startOfText} In your hand {Effects.Select(e => e.RulesText).First()}";
                    }
                default:
                    return "Rules Text Not Defined";
            }
        }
    }

    public EntitiesAffectedInfo EntitiesAffectedInfo { get; set; }
    public ZoneType ApplyWhenIn { get; set; } = ZoneType.InPlay;

    private EntityType EntitiesAffected => EntitiesAffectedInfo.EntitiesAffected;
    private CardFilter Filter => EntitiesAffectedInfo.Filter;
}

public enum StaticAbilityEntitiesAffected
{
    Self,
    OtherCreaturesYouControl,
    CardsInHand
}


//These need to be changed to use modifictions instead.

//TODO - replace with a PumpEffect with a StaticInfo
public class StaticPumpEffect : Effect
{
    public override string RulesText => $" gain {(Power >= 0 ? "+" : "-")}{Power}/{(Toughness >= 0 ? "+" : "-")}{Toughness}";
    public int Power { get; set; }
    public int Toughness { get; set; }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            var cardInstance = entity as CardInstance;
            if (cardInstance == null)
            {
                continue;
            }
            var abilitySource = source.Abilities.Where(ab => ab.Effects.Contains(this)).First();

            var modification = new ModAddToPowerToughness
            {
                Power = Power,
                Toughness = Toughness,
                OneTurnOnly = false
            };

            modification.StaticInfo = new StaticInfo
            {
                AbilitySource = abilitySource,
                EffectSource = source
            };

            cardInstance.Modifications.Add(modification);
        }
    }
}

//TODO - replace with with a mana effect with a static info
public class StaticManaReductionEffect : Effect
{
    public override string RulesText => $" cost {ReductionAmount} less to play.";
    public string ReductionAmount { get; set; }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            var cardInstance = entity as CardInstance;
            if (cardInstance == null)
            {
                continue;
            }

            var manaModification = new ModReduceManaCost
            {
                ReductionAmount = ReductionAmount
            };

            var abilitySource = source.Abilities.Where(ab => ab.Effects.Contains(this)).ToList();

            manaModification.StaticInfo = new StaticInfo
            {
                AbilitySource = abilitySource.First(),
                EffectSource = source
            };

            cardInstance.Modifications.Add(manaModification);
        }
    }
}

public class ModReduceManaCost : Modification, IModifyManaCost
{
    public string ReductionAmount { get; set; }
    public string ModifyManaCost(CardGame cardGame, CardInstance card, string originalManaCost)
    {
        var costAsCounts = new Mana(originalManaCost);
        var reductionCostAsCounts = new Mana(ReductionAmount);

        costAsCounts.ColorlessMana -= reductionCostAsCounts.ColorlessMana;
        costAsCounts.ColorlessMana = Math.Max(0, costAsCounts.ColorlessMana);

        foreach (var essenceType in reductionCostAsCounts.ColoredMana)
        {
            costAsCounts.ColoredMana[essenceType.Key] -= essenceType.Value;
            costAsCounts.ColoredMana[essenceType.Key] = Math.Max(0, costAsCounts.ColoredMana[essenceType.Key]);
        }

        return costAsCounts.ToManaString();
    }
}

//TODO - replace with a GiveAbility effect with a static info
public class StaticGiveAbilityEffect : Effect
{
    public override string RulesText => $" gain {Ability.RulesText}.";
    public CardAbility Ability { get; set; }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            var cardInstance = entity as CardInstance;

            if (cardInstance == null)
            {
                continue;
            }

            var abilitySource = source.Abilities.Where(ab => ab.Effects.Contains(this)).First();

            var abilityToGive = Ability.Clone();

            abilityToGive.Components.Add(new ContinuousAblityComponent
            {
                SourceEffect = this
            });

            cardInstance.Abilities.Add(abilityToGive);
        }
    }
}







