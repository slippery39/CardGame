using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MillEffect : Effect
{
    public int Amount { get; set; } = 1;
    public override string RulesText => $"Mill {Amount} cards";

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            if (!(entity is Player))
            {
                continue;
            }

            var playerEntity = entity as Player;

            for (var i = 0; i < Amount; i++)
            {
                cardGame.ZoneChangeSystem.MoveToZone(playerEntity.Deck.GetTopCard(), playerEntity.DiscardPile);
            }
        }
    }
}

