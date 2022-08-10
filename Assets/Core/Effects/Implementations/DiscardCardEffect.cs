using System.Collections.Generic;

public class DiscardCardEffect : Effect
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
}



