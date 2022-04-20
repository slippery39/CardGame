using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IStateBasedEffectSystem
{
    void CheckStateBasedEffects(CardGame cardGame);
}


public class DefaultStateBasedEffectSystem : IStateBasedEffectSystem
{
    public void CheckStateBasedEffects(CardGame cardGame)
    {
        var units = cardGame.GetUnitsInPlay();


        //Apply any static effects
        foreach (var unit in units)
        {
            var unitStaticAbilities = unit.GetAbilities<StaticAbility>();
            if (unitStaticAbilities.Count > 0)
            {
                foreach (var sAbility in unitStaticAbilities)
                {
                    //cardGame, source, ability.
                    cardGame.ContinuousEffectSystem.Apply(cardGame, unit, sAbility);
                }
            }
        }

        //Check Toughness of all creatures in play.
        //Keep checking until there is no more to check?
        foreach (var unit in units)
        {
            //State Based effect, all units with toughness 0 or less get moved to the discard pile.
            if (unit.Toughness <= 0)
            {
                var owner = cardGame.GetOwnerOfCard(unit);
                cardGame.ZoneChangeSystem.MoveToZone(cardGame, unit, owner.DiscardPile);
            }
        }
    }
}

