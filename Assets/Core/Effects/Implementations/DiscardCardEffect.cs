using System.Collections.Generic;
using System.Linq;

public class DiscardCardEffect : EffectWithChoice
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
    public override string ChoiceMessage => $"Discard {Amount} cards";

    public override int NumberOfChoices { get => Amount; set => Amount = value; }

    public DiscardCardEffect()
    {
        TargetInfo = TargetInfo.PlayerSelf();
    }

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

    public override void ChoiceSetup(CardGame cardGame, Player player, CardInstance source)
    {

    }

    public override List<CardInstance> GetValidChoices(CardGame cardGame, Player player)
    {
        return player.Hand.Cards;
    }

    public override void OnChoicesSelected(CardGame cardGame, Player player, List<CardGameEntity> choices)
    {
        cardGame.DiscardSystem.Discard(player, choices.Cast<CardInstance>().ToList());
    }
}



