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
        cardGame.ContinuousEffectSystem.ApplyStaticEffects();
        cardGame.ContinuousEffectSystem.RemoveStaticEffects();

        var deathCheckCounter = 0; //just in case something enters an infinite loop, I will want to know
        do
        {
            deathCheckCounter++;
            cardGame.ContinuousEffectSystem.ApplyStaticEffects();
            cardGame.ContinuousEffectSystem.RemoveStaticEffects();
        }
        while (CheckForDeadUnits() && deathCheckCounter < 10);

        if (deathCheckCounter >= 10)
        {
            cardGame.Log("Potential infinite loop regarding death checks in our state based effects...");
        }



        cardGame.Player1.Modifications.GetOfType<IOnAfterStateBasedEffects>().ForEach(mod =>
        {
            mod.OnAfterStateBasedEffects(cardGame, cardGame.Player1);
        });

        cardGame.Player2.Modifications.GetOfType<IOnAfterStateBasedEffects>().ForEach(mod =>
        {
            mod.OnAfterStateBasedEffects(cardGame, cardGame.Player2);
        });
    }

    //Returns true if something died, which means we have to apply/remove any static effects and check again.
    private bool CheckForDeadUnits()
    {
        var units = cardGame.GetUnitsInPlay();
        bool somethingDied = false;
        foreach (var unit in units)
        {
            //State Based effect, all units with current toughness 0 or less get moved to the discard pile.
            if (unit.Toughness - unit.DamageTaken <= 0)
            {
                somethingDied = true;
                var owner = cardGame.GetOwnerOfCard(unit);
                cardGame.ZoneChangeSystem.MoveToZone(unit, owner.DiscardPile);
            }
        }

        return somethingDied;
    }
}


