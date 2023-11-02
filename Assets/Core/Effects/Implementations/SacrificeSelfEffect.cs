using System;
using System.Collections.Generic;

public class SacrificeSelfEffect : Effect
{
    public override string RulesText => "Sacrifice this unit";

    public SacrificeSelfEffect()
    {
        TargetInfo = TargetInfo.Source();
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            if (!(entity is CardInstance))
            {
                throw new Exception("Error : only units can be effected with the sacrifice self effect");
            }
            var card = (CardInstance)entity;
            cardGame.SacrificeSystem.Sacrifice(player, card);
        }
    }
}


