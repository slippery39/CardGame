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

        cardGame.Player1.Modifications.GetOfType<IOnAfterStateBasedEffects>().ForEach(mod =>
        {
            mod.OnAfterStateBasedEffects(cardGame, cardGame.Player1);
        });

        cardGame.Player2.Modifications.GetOfType<IOnAfterStateBasedEffects>().ForEach(mod =>
        {
            mod.OnAfterStateBasedEffects(cardGame, cardGame.Player2);
        });

        //Check to see if any player has lost
        cardGame.WinLoseSystem.CheckLosers();

        //Sanity Check for any cards which do not have a zone.
        if (cardGame.RegisteredEntities.GetOfType<CardInstance>().Where(e => e.GetZone() == null).Any())
        {
            var cards = cardGame.RegisteredEntities.GetOfType<CardInstance>().Where(e => e.GetZone() == null).ToList();

            cards.ForEach(c =>
            {
                cardGame.EventLogSystem.AddEvent($"{c.Name} does not have a zone attached to it? Where does it exist?");
            });

            var i = 0;
        }


        cardGame.EventLogSystem.AddEvent("End of checking state based effects");

        cardGame.OnGameStateChanged.OnNext(cardGame.Copy());
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


