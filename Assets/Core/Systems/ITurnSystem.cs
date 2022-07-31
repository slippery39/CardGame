using System.Collections.Generic;
using System.Linq;

public interface ITurnSystem
{
    int TurnId { get; set; }
    void StartTurn();
    void EndTurn();
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

    private CardGame cardGame;

    public DefaultTurnSystem(CardGame cardGame)
    {
        TurnId = 1;
        this.cardGame = cardGame;
    }
    public void StartTurn()
    {
        //Reset any summoning sick units
        var activePlayersUnits = cardGame.GetUnitsInPlay().Where(unit => cardGame.GetOwnerOfCard(unit) == cardGame.ActivePlayer);
        foreach (var unit in activePlayersUnits)
        {
            unit.IsSummoningSick = false;
        }

        var unitsAndItems = cardGame.GetCardsInPlay(cardGame.ActivePlayer);

        //Remove any ability cooldowns
        foreach (var unit in unitsAndItems)
        {
            var activatedAbilities = unit.GetAbilitiesAndComponents<ActivatedAbility>().Where(ability => ability.OncePerTurn);
            foreach (var ab in activatedAbilities)
            {
                //Remove all ability cooldowns.
                ab.Components = ab.Components.Where(c => !(c is AbilityCooldown)).ToList();
            }
        }

        cardGame.HandleTriggeredAbilities(activePlayersUnits, TriggerType.AtTurnStart);
        //Reset any spent mana
        cardGame.ManaSystem.ResetManaAndEssence(cardGame.ActivePlayer);
        //Active Player Gains A Mana - not anymore.
        //cardGame.ManaSystem.AddMana(cardGame, cardGame.ActivePlayer, 1);
        //Active Player draws a card
        cardGame.CardDrawSystem.DrawCard(cardGame.ActivePlayer);
    }

    public void EndTurn()
    {
        //Execute our battles:
        //No need for this anymore.
        //cardGame.BattleSystem.ExecuteBattles();


        var activePlayersUnits = cardGame.GetUnitsInPlay().Where(unit => cardGame.GetOwnerOfCard(unit) == cardGame.ActivePlayer);

        //Trigger any End of turn abilities
        cardGame.HandleTriggeredAbilities(activePlayersUnits, TriggerType.AtTurnEnd);

        //Handle any RespondToEndOfTurnAbilities (Like Summoning Trap);
        var respondToEndOfTurnCards = cardGame.InactivePlayer.Hand.Cards.Where(card => card.Abilities.GetOfType<RespondToOpponentEndOfTurnAbility>().Any()).ToList();

        foreach (var card in respondToEndOfTurnCards)
        {
            var abilities = card.GetAbilitiesAndComponents<RespondToOpponentEndOfTurnAbility>();
            foreach (var ab in abilities)
            {
                ab.AtOpponentEndOfTurn(cardGame, card);
            }
        }



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

        //Remove any temporary player abilities.
        cardGame.PlayerAbilitySystem.RemoveOneTurnModifications(cardGame.Player1);
        cardGame.PlayerAbilitySystem.RemoveOneTurnModifications(cardGame.Player2);

        //Reset the players mana played this turn.
        cardGame.ActivePlayer.ManaPlayedThisTurn = 0;

        //Change the active player
        _ = cardGame.ActivePlayerId == 1 ? cardGame.ActivePlayerId = 2 : cardGame.ActivePlayerId = 1;


        TurnId++;
    }
}