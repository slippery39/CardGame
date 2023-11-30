using System;

public class SacrificeSelfAdditionalCost : AdditionalCost
{
    public override string RulesText => $@"Sacrifice #this#"; //#this needs to be replaced with the name of the unit.

    public SacrificeSelfAdditionalCost()
    {
        Type = AdditionalCostType.SacrificeSelf;
    }

    public override bool CanPay(CardGame cardGame, Player player, CardGameEntity source)
    {
        //check if the source is in play...
        if (!(source is CardInstance))
        {
            throw new Exception("Source should be a card instance for a SacrificeSelfAdditionalCost");
        }
        return cardGame.IsInPlay(source as CardInstance);
    }
    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo)
    {
        if (!(sourceCard is CardInstance))
        {
            throw new Exception("Source should be a card instance for a SacrificeSelfAdditionalCost");
        }

        cardGame.SacrificeSystem.Sacrifice(player, sourceCard as CardInstance);
    }
}




