using System.Collections.Generic;

public class CyclingAbility : ActivatedAbility
{
    public override string RulesText => $"Cycling : {ManaCost}";

    public CyclingAbility()
    {
        Init(null);
    }

    public CyclingAbility(List<Effect> AdditionalEffects)
    {
        Init(AdditionalEffects);
    }

    private void Init(List<Effect> AdditionalEffects)
    {
        ActivationZone = ZoneType.Hand;
        AdditionalCost = new DiscardSelfAdditionalCost()
        {
        };
        Effects = new List<Effect>()
        {
            new DrawCardEffect
            {
                Amount = 1
            },
        };
        if (AdditionalEffects != null)
        {
            Effects.AddRange(AdditionalEffects);
        }
    }
}



