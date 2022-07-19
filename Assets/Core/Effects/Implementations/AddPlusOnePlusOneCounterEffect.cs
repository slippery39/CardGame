using System;
using System.Collections.Generic;

public class AddPlusOnePlusOneCounterEffect : Effect
{
    public int Amount { get; set; }

    public override string RulesText => "Add a +1/+1 counter";

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            if (!(entity is CardInstance))
            {
                throw new Exception("Error : only units can be effected with PumpPowerByNumberOfArtifactsEffect");
            }
            var card = (CardInstance)entity;
            cardGame.CountersSystem.AddPlusOnePlusOneCounter(card, Amount);
        }
    }
}



