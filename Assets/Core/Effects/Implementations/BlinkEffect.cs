using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BlinkEffect : Effect
{
    public override string RulesText => "Blink a creature";

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            if (entity is CardInstance card)
            {
                //It should be blinked in place     
                //Ideally this should remove all damage from it and trigger any other battlefield effects.
                var zone = cardGame.GetZoneOfCard(card);
                cardGame.ZoneChangeSystem.MoveToZone(card, player.Exile);
                cardGame.ZoneChangeSystem.MoveToZone(card, zone);
                cardGame.Log($"{entity.Name} was blinked!");
            }
        }
    }
}

