﻿using System.Collections.Generic;

public class StormEffect : Effect
{
    public override string RulesText => $"Storm\r\n {ChildEffect.RulesText}";
    public Effect ChildEffect { get; set; }

    public override TargetType TargetType { get=>ChildEffect.TargetType; }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        var amount = cardGame.SpellsCastThisTurn;
        cardGame.Log($"Storming {amount} times");

        for(var i= 0;i < amount; i++) 
        {
            //Potential issue here with the entities to apply
            ChildEffect.Apply(cardGame, player, source, entitiesToApply);
        }
    }
}
