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
            return String.Join(" and ", Effects.Select(eff =>
             {
                 string cardType = "";

                 switch (eff.TargetType)
                 {
                     case TargetType.CardsInHand:
                         {
                             cardType = "Cards you play";
                             break;
                         }
                     case TargetType.OtherCreaturesYouControl:
                         {
                             cardType = "Other units you control";
                             break;
                         }
                     case TargetType.Self:
                         {
                             cardType = "You";
                             break;
                         }
                     default:
                         {
                             throw new Exception($"Unhandled TargetType {eff.TargetType.ToString()} in StaticAbility rules text");
                         }
                 }

                 //TODO apply filter for goblins or something.

                 return eff.RulesText.Replace("#cardType", cardType);
             })
                );

        }
    }

    //public EntitiesAffectedInfo EntitiesAffectedInfo { get; set; }
    public ZoneType ApplyWhenIn { get; set; } = ZoneType.InPlay;

    //private EntityType EntitiesAffected => EntitiesAffectedInfo.EntitiesAffected;
    //private CardFilter Filter => EntitiesAffectedInfo.Filter;
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
    public override string RulesText => $"#cardType# gain {(Power >= 0 ? "+" : "-")}{Power}/{(Toughness >= 0 ? "+" : "-")}{Toughness}";
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
                SourceAbility = abilitySource,
                SourceCard = source
            };

            cardInstance.Modifications.Add(modification);
        }
    }
}

//TODO - replace with with a mana effect with a static info
public class StaticManaReductionEffect : Effect
{
    public override string RulesText => $"#cardType# cost {ReductionAmount} less to play.";
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
                ReductionAmount = ReductionAmount,
                OneTurnOnly = false
            };

            var abilitySource = source.Abilities.Where(ab => ab.Effects.Contains(this)).ToList();

            manaModification.StaticInfo = new StaticInfo
            {
                SourceAbility = abilitySource.First(),
                SourceCard = source
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

public class StaticPlayAdditionalLandEffect : Effect
{
    public int Amount { get; set; }
    public override string RulesText => $"#cardType# may play an additional {Amount} mana cards each turn";

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            var playerEntity = entity as Player;

            if (playerEntity == null)
            {
                continue;
            }

            var manaPerTurnModification = new ManaPerTurnModification
            {
                Amount = Amount,
                OneTurnOnly = false
            };

            manaPerTurnModification.StaticInfo = new StaticInfo
            {
                SourceAbility = source.Abilities.Where(ab => ab.Effects.Contains(this)).First(),
                SourceCard = source
            };

            playerEntity.Modifications.Add(manaPerTurnModification);
        }
    }
}

//TODO - replace with a GiveAbility effect with a static info
public class StaticGiveAbilityEffect : Effect
{
    public override string RulesText => $"#cardType# gain {Ability.RulesText}.";
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

            var sourceAbility = source.Abilities.First(ab => ab.Effects.Contains(this));

            if (sourceAbility == null)
            {
                cardGame.Log("Could not find the source ability for an effect... possibly it was modified before the effect could take place");
            }

            abilityToGive.Components.Add(new ContinuousAblityComponent
            {
                SourceEffect = this,
                SourceCard = source,
                SourceAbility = sourceAbility as StaticAbility
            });

            cardInstance.Abilities.Add(abilityToGive);
        }
    }
}







