using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IStateBasedEffectSystem
{
    void CheckStateBasedEffects(CardGame cardGame);
}


public class DefaultStateBasedEffectSystem: IStateBasedEffectSystem
{
    public void CheckStateBasedEffects(CardGame cardGame)
    {
        //Check Toughness of all creatures in play.

        var units =  cardGame.GetUnitsInPlay();

        foreach(var unit in units)
        {
            //State Based effect, all units with toughness 0 or less get moved to the discard pile.
            if (unit.Toughness <= 0)
            {
                var owner = cardGame.GetOwnerOfUnit(unit);
                cardGame.ZoneChangeSystem.MoveToZone(cardGame, unit, owner.DiscardPile);
            }
        }
    }
}

