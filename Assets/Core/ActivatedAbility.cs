using System.Collections.Generic;

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

            return $@"{manaCostStr}{additionalCostStr}{AbilityEffect.RulesText}";
        }
    }
    public string ManaCost { get; set; }
    public AdditionalCost AdditionalCost { get; set; } = null;
    public Effect AbilityEffect { get; set; }

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
