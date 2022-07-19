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

    public List<StaticAbilityEffect> Effects { get; set; }

    public ZoneType ApplyWhenIn { get; set; } = ZoneType.Discard;

    private EntityType EntitiesAffected => EntitiesAffectedInfo.EntitiesAffected;
    private CardFilter Filter => EntitiesAffectedInfo.Filter;
}

public enum StaticAbilityEntitiesAffected
{
    Self,
    OtherCreaturesYouControl,
    CardsInHand
}

//each unit should have an instance of the effect

public abstract class StaticAbilityEffect
{
    public abstract string RulesText { get; }
}

public class StaticPumpEffect : StaticAbilityEffect
{
    public override string RulesText => $" gain {(Power >= 0 ? "+" : "-")}{Power}/{(Toughness >= 0 ? "+" : "-")}{Toughness}";
    public int Power { get; set; }
    public int Toughness { get; set; }
}

public class StaticManaReductionEffect : StaticAbilityEffect
{
    public override string RulesText => $" cost {ReductionAmount} less to play.";
    public string ReductionAmount { get; set; }
}

public class StaticGiveAbilityEffect : StaticAbilityEffect
{
    public override string RulesText => " gain haste.";
    public CardAbility Ability { get; set; }
}







