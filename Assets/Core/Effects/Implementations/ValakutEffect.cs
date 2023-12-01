using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ValakutEffect : Effect
{
    public override string RulesText => "if you have more than 7 total red mana, deal 3 damage to a random target";

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

