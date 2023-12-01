using Assets.Core;
using System.Collections.Generic;
using System.Linq;

public class ActivatedAbility : CardAbility
{
    public ZoneType ActivationZone { get; set; } = ZoneType.InPlay; //default to in play.
    public override string RulesText
    {
        get
        {
            string manaCostStr = "";
            if (ManaCost != "0")
            {
                manaCostStr = ManaCost;
            }

            string additionalCostStr = "";
            if (HasAdditionalCost())
            {
                additionalCostStr = $@"{AdditionalCost.RulesText}";
                if (manaCostStr != "")
                {
                    manaCostStr += ", ";
                }
            }

            return $@"{manaCostStr}{additionalCostStr}: {string.Join(" and ",
                Effects.Select(GetEffectRulesText)
                )
                .UpperFirst()}";

        }
    }
    public string ManaCost { get; set; }
    public AdditionalCost AdditionalCost { get; set; } = null;
    public CardFilter Filter { get; internal set; }
    public bool OncePerTurnOnly { get; set; } = false;
    public bool ExhaustOnUse { get; set; } = false;

    //TODO - Why can't the effect compile its own rules text? Why does it need to be a static method?
    private string GetEffectRulesText(Effect e)
    {
        return e.RulesText;
    }

    public bool HasAdditionalCost()
    {
        return AdditionalCost != null;
    }

    public bool HasAdditionalCostChoices()
    {
        return HasAdditionalCost() && AdditionalCost.NeedsChoice;
    }

    public bool HasTargets()
    {
        return Effects.NeedsTargets();
    }
}
