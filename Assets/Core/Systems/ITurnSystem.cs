﻿using System.Collections.Generic;
using System.Linq;

public interface ITurnSystem
{
    int TurnId { get; set; }
    void StartTurn(CardGame cardGame);
    void EndTurn(CardGame cardGame);
}

//Not in use right now, but keep in mind these phases as we add more functionality.
public enum TurnPhase
{
    StartOfTurn,
    Main,
    Battle,
    EndOfTurn
}

public class DefaultTurnSystem : ITurnSystem
{
    public int TurnId { get; set; }

    public DefaultTurnSystem()
    {
        TurnId = 1;
    }
    public void StartTurn(CardGame cardGame)
    {
        //Reset any summoning sick units
        var activePlayersUnits = cardGame.GetUnitsInPlay().Where(unit => cardGame.GetOwnerOfCard(unit) == cardGame.ActivePlayer);
        foreach (var unit in activePlayersUnits)
        {
            unit.IsSummoningSick = false;
        }

        //Remove any ability cooldowns
        foreach (var unit in activePlayersUnits)
        {
            var activatedAbilities = unit.GetAbilities<ActivatedAbility>().Where(ability => ability.OncePerTurn);
            foreach (var ab in activatedAbilities)
            {
                //Remove all ability cooldowns.
                ab.Components = ab.Components.Where(c => !(c is AbilityCooldown)).ToList();
            }
        }

        cardGame.HandleTriggeredAbilities(activePlayersUnits, TriggerType.AtTurnStart);
        //Reset any spent mana
        cardGame.ManaSystem.ResetManaAndEssence(cardGame, cardGame.ActivePlayer);
        //Active Player Gains A Mana - not anymore.
        //cardGame.ManaSystem.AddMana(cardGame, cardGame.ActivePlayer, 1);
        //Active Player draws a card
        cardGame.CardDrawSystem.DrawCard(cardGame.ActivePlayer);
    }

    public void EndTurn(CardGame cardGame)
    {
        //Execute our battles:
        cardGame.BattleSystem.ExecuteBattles(cardGame);


        var activePlayersUnits = cardGame.GetUnitsInPlay().Where(unit => cardGame.GetOwnerOfCard(unit) == cardGame.ActivePlayer);

        //Trigger any End of turn abilities
        cardGame.HandleTriggeredAbilities(activePlayersUnits, TriggerType.AtTurnEnd);

        //Remove any temporary abilities
        cardGame.GetUnitsInPlay().ForEach(c =>
        c.Abilities = c.Abilities
       .Where(c => c.ThisTurnOnly == false)
       .ToList());

        //Remove any temporary modifications
        cardGame.GetUnitsInPlay().ForEach(c =>
        c.Modifications = c.Modifications
        .Where(c => c.OneTurnOnly == false)
        .ToList());

        //Reset the players mana played this turn.
        cardGame.ActivePlayer.ManaPlayedThisTurn = 0;

        //Change the active player
        _ = cardGame.ActivePlayerId == 1 ? cardGame.ActivePlayerId = 2 : cardGame.ActivePlayerId = 1;


        TurnId++;
    }
}