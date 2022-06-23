using System.Collections.Generic;
using System.Linq;

public class ActivatedAbility : CardAbility
{
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
                additionalCostStr = $@"{AdditionalCost.RulesText}:";
                if (manaCostStr != "")
                {
                    manaCostStr += ", ";
                }
            }

            return $@"{manaCostStr}{additionalCostStr}{string.Join("and ",Effects.Select(e=>e.RulesText))}";
        }
    }
    public string ManaCost { get; set; }
    public AdditionalCost AdditionalCost { get; set; } = null;
    public List<Effect> Effects { get; set; }
    public CardFilter Filter { get; internal set; }

    public bool HasAdditionalCost()
    {
        return AdditionalCost != null;
    }

    public bool HasChoices()
    {
        return HasAdditionalCost() && AdditionalCost.NeedsChoice;
    }

    public bool HasTargets()
    {
        return TargetHelper.NeedsTargets(this);
    }
}
