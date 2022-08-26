using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ValakutEffect : Effect
{
    public override string RulesText => " deal 3 damage to a random target";

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        if (player.ManaPool.TotalColoredMana[ManaType.Red] < 7)
        {
            return;
        }

        cardGame.Log("Valakut Trigger!");

        foreach (var entity in entitiesToApply)
        {
            cardGame.DamageSystem.DealDamage(source, entity, 3);
        }
    }
}

public class StormEffect : Effect
{
    public override string RulesText => $"Storm\r\n {ChildEffect.RulesText}";
    public Effect ChildEffect { get; set; }

    public override TargetType TargetType { get=>ChildEffect.TargetType; }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        var amount = 3;
        cardGame.Log($"Storming {amount} times");

        for(var i= 0;i < amount; i++) 
        {
            //Potential issue here with the entities to apply.
            ChildEffect.Apply(cardGame, player, source, entitiesToApply);
        }
    }
}

