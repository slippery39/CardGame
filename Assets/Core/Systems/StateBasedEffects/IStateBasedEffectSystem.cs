using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IStateBasedEffectSystem
{
    void CheckStateBasedEffects();
}


public class DefaultStateBasedEffectSystem : CardGameSystem, IStateBasedEffectSystem
{

    public DefaultStateBasedEffectSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }

    public void CheckStateBasedEffects()
    {
        cardGame.EventLogSystem.AddEvent("Starting to Check State Based Effects");
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

        cardGame.Player1.Modifications.GetOfType<IOnAfterStateBasedEffects>().ToList().ForEach(mod =>
        {
            mod.OnAfterStateBasedEffects(cardGame, cardGame.Player1);
        });

        cardGame.Player2.Modifications.GetOfType<IOnAfterStateBasedEffects>().ToList().ForEach(mod =>
        {
            mod.OnAfterStateBasedEffects(cardGame, cardGame.Player2);
        });


        //Win Loss Checks
        if (cardGame.Player1.Health <= 0)
        {
            cardGame.WinLoseSystem.LoseGame(cardGame.Player1);
        }

        if (cardGame.Player2.Health <= 0)
        {
            cardGame.WinLoseSystem.LoseGame(cardGame.Player2);
        }

        if (cardGame.Player1.DrawnCardWithNoDeck)
        {
            cardGame.WinLoseSystem.LoseGame(cardGame.Player1);
        }

        if (cardGame.Player2.DrawnCardWithNoDeck)
        {
            cardGame.WinLoseSystem.LoseGame(cardGame.Player1);
        }

        //Check to see if any player has lost
        cardGame.WinLoseSystem.CheckLosers();

        //This taking a lot of cpu usage for not much benefit... we only want to fire this method in situations where it matters (i.e.
        //we have a subscriber to it, otherwise it is useless, we might want to consider changing this to an event so we can easily check
        //if it has been subscribed to.
        cardGame.EventLogSystem.FireGameStateChanged();
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


