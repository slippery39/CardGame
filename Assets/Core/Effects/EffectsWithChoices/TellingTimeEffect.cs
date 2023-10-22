using System;
using System.Collections.Generic;
using System.Linq;

public class TellingTimeEffect : EffectWithChoice
{
    public override string ChoiceMessage
    {
        get
        {
            if (Choices.Count == 0)
            {
                return "Choose a card to put in your hand.";
            }
            if (Choices.Count == 1)
            {
                return "Choose a card to put on top of your deck";
            }
            if (Choices.Count == 2)
            {
                return "Choose a card to put on the bottom of your deck";
            }
            return "should never see this message";
        }
    }

    public override int NumberOfChoices { get; set; } = 3;

    public override string RulesText => "Look at the top 3 cards of your deck. Put one into your hand, one on top of your deck, and the other on the bottom of your deck.";

    public override List<CardInstance> GetValidChoices(CardGame cardGame, Player player)
    {
        return GetValidChoicesInternal(cardGame, player, Choices);
    }

    /// <summary>
    /// Gets the valid choices assuming a blank state (i.e. at the start before they have selected any cards
    /// Need this because GetValidChoices returns differently once they have selected some cards
    /// </summary>
    /// <returns></returns>
    private List<CardInstance> GetValidChoicesInternal(CardGame cardGame, Player player, List<CardInstance> selected = null)
    {
        if (selected == null)
        {
            selected = new List<CardInstance>();
        }

        var cards = player.Deck.TakeLast(3).ToList();
        cards = cards.Where(c => !selected.Contains(c)).ToList();
        return cards;
    }

    public override bool IsValid(CardGame cardGame, Player player)
    {
        var choicesAsIds = Choices.Select(c => c.EntityId).ToList();
        var validChoicesAsIds = GetValidChoicesInternal(cardGame, player).Select(c => c.EntityId).ToList();

        var hasDuplicates = choicesAsIds.GroupBy(x => x).Any(grp => grp.Count() > 1);
        var selectedInvalidOption = choicesAsIds.Except(validChoicesAsIds).Any();

        //Check if there are any duplicates selected and if the choices match up with what is valid.
        return !hasDuplicates && !selectedInvalidOption;
    }

    public override void ChoiceSetup(CardGame cardGame, Player player, CardInstance source)
    {
        Choices = new List<CardInstance>();
        GetValidChoices(cardGame, player).ForEach(c => c.RevealedToOwner = true);
    }

    public override void OnChoicesSelected(CardGame cardGame, Player player, List<CardGameEntity> choices)
    {
        //Quick bug fix, we have a problem with different parts of the code using this in different ways.
        //This helps sync it up, but we should 
        if (Choices.Count == 0)
        {
            Choices = choices.Cast<CardInstance>().ToList();
        }

        var castedChoices = choices.Cast<CardInstance>().ToList();

        //First choice gets put into your hand,
        //Second choice gets put on the top of your deck.
        //Last choice gets put on the bottom of the deck.

        //Note we are using the local method variable 'choices' instead of 'Choices' 
        //since 'Choices' is only for the front end ui.

        //TODO - choices vs Choices?
        var cardToPutIntoHand = castedChoices[0];
        var cardToPutOnTop = castedChoices[1];
        var cardToPutOnBottom = castedChoices[2];

        cardGame.CardDrawSystem.PutIntoHand(player, cardToPutIntoHand);
        player.Deck.MoveToBottom(cardToPutOnBottom);
        //the remaining card will stay on top by default.        
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        throw new NotImplementedException();
    }
}

