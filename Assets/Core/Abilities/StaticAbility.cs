using System.Collections.Generic;
using System.Linq;
//Other creatures you control get +1/+1
//This creature gets +0/+2 as long as its your turn
//Lhurgoyf's Power and Toughness are equal to the number of creatures in all graveyards (or boneyard wurm)

public class StaticAbility : CardAbility
{
    public override string RulesText
    {
        get
        {
            switch (AffectedEntities)
            {
                case StaticAbilityEntitiesAffected.Self:
                    return $"This {string.Join(" and ", Effects.Select(e => e.RulesText))}";
                case StaticAbilityEntitiesAffected.OtherCreaturesYouControl:
                    return $"Other creatures you control {string.Join(" and ", Effects.Select(e => e.RulesText))}";
                default:
                    return "Rules Text Not Defined";
            }
        }
    }

    public StaticAbilityEntitiesAffected AffectedEntities { get; set; }

    public List<StaticAbilityEffect> Effects { get; set; }
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
    public override string RulesText => $"gain {(Power >= 0 ? "+" : "-")}{Power}/{(Toughness >= 0 ? "+" : "-")}{Toughness}";
    public int Power { get; set; }
    public int Toughness { get; set; }
}

public class StaticManaReductionEffect : StaticAbilityEffect
{
    public override string RulesText => "**Fix this - Reduce mana cost by 1 ";
    public string ReductionAmount { get; set; }
    public CardFilter Filter { get; set; }
}







