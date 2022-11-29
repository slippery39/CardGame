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

public class DefaultTurnSystem : CardGameSystem, ITurnSystem
{
    public int TurnId { get; set; }

    public DefaultTurnSystem(CardGame cardGame)
    {
        TurnId = 1;
        this.cardGame = cardGame;
    }
    public void StartTurn()
    {
        cardGame.EventLogSystem.AddEvent($"{cardGame.ActivePlayer.Name}'s Turn: ID: {TurnId}");
        cardGame.SpellsCastThisTurn = 0;
        //Reset any spent mana
        cardGame.ManaSystem.ResetManaAndEssence(cardGame.ActivePlayer);
        //Reset any summoning sick units
        var activePlayersUnits = cardGame.GetUnitsInPlay().Where(unit => cardGame.GetOwnerOfCard(unit) == cardGame.ActivePlayer);
        foreach (var unit in activePlayersUnits)
        {
            unit.IsSummoningSick = false;
        }

        RefreshExhaustionAndCooldowns();

        /*
         * TODO - There seems to be a pattern here of needing to grab all potential abilities / modifications on cards as well as players
         * that implement a certain interface. In this case it is the IOnTurnStart interface, but we could for example have IOnTurnEnd, or IOnWhatever
         * interfaces to implement as well. Lets make a method for that 
         */

        foreach (var card in cardGame.ActivePlayer.GetUnitsInPlay().Union(cardGame.ActivePlayer.Items.Cards))
        {
            var turnStartAbilities = card.Abilities.GetOfType<IOnTurnStart>();

            foreach (var ab in turnStartAbilities)
            {
                ab.OnTurnStart(cardGame, cardGame.ActivePlayer, card);
            }
        }

        cardGame.ActivePlayer.Modifications.GetOfType<IOnTurnStart>().ForEach(mod =>
        {
            //Shouldn't need to put null if its on a player or should we?
            mod.OnTurnStart(cardGame, cardGame.ActivePlayer, null);
        });

        //Remove Before Comitting - Temporary Code For Testing Out Lotus Bloom 
        //We need to call a ToList() here or else we get an error if a card moves
        //to a different zone as a result of OnTurnStart.
        //of course this then means we need to actually make sure that the card is
        //in the proper zone as the OnTUrnStart method is called
        foreach (var card in cardGame.ActivePlayer.Exile.ToList())
        {
            if (cardGame.GetZoneOfCard(card).ZoneType != ZoneType.Exile)
            {
                continue;
            }

            var turnStartMods = card.Modifications.GetOfType<IOnTurnStart>();
            foreach (var mod in turnStartMods)
            {
                mod.OnTurnStart(cardGame, cardGame.ActivePlayer, card);
            }
        };

        cardGame.HandleTriggeredAbilities(activePlayersUnits, TriggerType.AtTurnStart);
        //Active Player Gains A Mana - not anymore.
        //cardGame.ManaSystem.AddMana(cardGame, cardGame.ActivePlayer, 1);
        //Active Player draws a card
        cardGame.CardDrawSystem.DrawCard(cardGame.ActivePlayer);
    }

    private void RefreshExhaustionAndCooldowns()
    {
        var unitsAndItems = cardGame.GetCardsInPlay(cardGame.ActivePlayer);

        //Remove any ability cooldowns
        foreach (var unit in unitsAndItems)
        {
            //Unexhaust any units
            if (unit.Abilities.GetOfType<DontUnexhaustAbility>().Any() == false)
            {
                unit.IsExhausted = false;
            }
            var activatedAbilities = unit.GetAbilitiesAndComponents<ActivatedAbility>().Where(ability => ability.OncePerTurnOnly);
            foreach (var ab in activatedAbilities)
            {
                //Remove all ability cooldowns.
                ab.Components = ab.Components.Where(c => !(c is AbilityCooldown)).ToList();
            }
        }
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
        var respondToEndOfTurnCards = cardGame.InactivePlayer.Hand.Where(card => card.Abilities.GetOfType<RespondToOpponentEndOfTurnAbility>().Any()).ToList();

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