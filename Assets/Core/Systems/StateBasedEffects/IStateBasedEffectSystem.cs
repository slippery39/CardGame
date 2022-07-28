using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IStateBasedEffectSystem
{
    void CheckStateBasedEffects();
}


public class DefaultStateBasedEffectSystem : IStateBasedEffectSystem
{
    private CardGame cardGame;

    public DefaultStateBasedEffectSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }

    public void CheckStateBasedEffects()
    {
        CheckForDeadUnits();
        cardGame.ContinuousEffectSystem.ApplyStaticEffects();
        cardGame.ContinuousEffectSystem.RemoveStaticEffects();

        cardGame.Player1.Modifications.GetOfType<IOnAfterStateBasedEffects>().ForEach(mod =>
        {
            mod.OnAfterStateBasedEffects(cardGame, cardGame.Player1);
        });

        cardGame.Player2.Modifications.GetOfType<IOnAfterStateBasedEffects>().ForEach(mod =>
        {
            mod.OnAfterStateBasedEffects(cardGame, cardGame.Player2);
        });
    }

    private void CheckForDeadUnits()
    {
        var units = cardGame.GetUnitsInPlay();
        foreach (var unit in units)
        {
            //State Based effect, all units with current toughness 0 or less get moved to the discard pile.
            if (unit.Toughness - unit.DamageTaken <= 0)
            {
                var owner = cardGame.GetOwnerOfCard(unit);
                cardGame.ZoneChangeSystem.MoveToZone(unit, owner.DiscardPile);
            }
        }
    }
}


