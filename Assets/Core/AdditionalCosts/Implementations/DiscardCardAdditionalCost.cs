using System.Collections.Generic;
using System.Linq;

public class DiscardCardAdditionalCost : AdditionalCost
{
    public override string RulesText
    {
        get
        {
            return "Discard a card";
        }
    }

    public DiscardCardAdditionalCost()
    {
        Type = AdditionalCostType.Discard;
        NeedsChoice = true;
    }

    public override bool CanPay(CardGame cardGame, Player player, CardGameEntity source)
    {
        return player.Hand.Any();
    }

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo)
    {
        var cardsToDiscard = costInfo.EntitiesChosen.Cast<CardInstance>().ToList();

        foreach (var entity in cardsToDiscard)
        {
            cardGame.DiscardSystem.Discard(player, entity);
        }
    }
    public override List<CardGameEntity> GetValidChoices(CardGame cardGame, Player player, CardGameEntity sourceEntity)
    {
        return player.Hand.Cast<CardGameEntity>().ToList();
    }
}




