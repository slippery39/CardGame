using System.Collections.Generic;
using System.Linq;

public class DiscardCardEffect : Effect, IEffectWithChoice
{
    public override string RulesText
    {
        get
        {
            if (Amount == 1)
            {
                return "Discard a card";
            }
            else
            {
                return $@"Discard {Amount} Cards";
            }
        }
    }
    public int Amount { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.PlayerSelf;

    public string ChoiceMessage => $"Discard {Amount} cards";

    public int NumberOfChoices { get => Amount; set => Amount = value; }

    public List<CardInstance> Choices => new List<CardInstance>();

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        //DISCARDING BY CHOICE IS HANDLED SOMEWHERE ELSE RIGHT NOW
        return;
        //POTENTIAL CODE FOR IF WE HAVE TO IMPLEMENT RANDOM DISCARD
        /*
        var validCardsToDiscard = player.Hand.Cards;

        if (validCardsToDiscard.Count() < discardCardEffect.Amount)
        {
            foreach (var card in validCardsToDiscard)
            {
                cardGame.DiscardSystem.Discard(player, card);
            }
        }
        else
        {
            foreach (var card in validCardsToDiscard.Randomize().Take(discardCardEffect.Amount))
            {
                cardGame.DiscardSystem.Discard(player, card);
            }
        }
        */
    }

    public void ChoiceSetup(CardGame cardGame, Player player, CardInstance source)
    {
        
    }

    public List<CardInstance> GetValidChoices(CardGame cardGame, Player player)
    {
        return player.Hand.Cards;
    }

    public void OnChoicesSelected(CardGame cardGame, Player player, List<CardGameEntity> choices)
    {
        cardGame.DiscardSystem.Discard(player,choices.Cast<CardInstance>().ToList());
    }
}



