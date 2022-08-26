using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RiteOfFlameEffect : Effect
{
    public override string RulesText => "Add 2RR to your mana pool, then add 1R for each card named Rite of Flame in a discard pile";

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        var numberOfRiteOfFlames = player.DiscardPile.Where(d => d.Name.ToLower() == "rite of flame");

        cardGame.ManaSystem.AddTemporaryMana(player, "2RR");
        for (var i = 0; i < numberOfRiteOfFlames.Count(); i++)
        {
            cardGame.ManaSystem.AddTemporaryMana(player, "1R");
        }
    }
}

