using System;
using System.Collections.Generic;

public class GiveShieldEffect : Effect
{
    public override string RulesText
    {
        get
        {
            return $"Give a shield to #effectTargetType#";
        }
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            if (!(entity is CardInstance))
            {
                throw new Exception("Error : only units can be effected with PumpPowerByNumberOfArtifactsEffect");
            }
            var card = (CardInstance)entity;
            card.Shields++;
        }
    }
}



