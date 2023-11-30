public class DiscardSelfAdditionalCost : AdditionalCost
{
    public override string RulesText => $@"Discard this card";

    public DiscardSelfAdditionalCost()
    {
        Type = AdditionalCostType.DiscardSelf;
    }
    public override bool CanPay(CardGame cardGame, Player player, CardGameEntity source)
    {
        var card = source as CardInstance;
        if (card == null)
        {
            return false;
        }

        return cardGame.GetZoneOfCard(card).ZoneType == ZoneType.Hand;
    }

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo)
    {
        cardGame.DiscardSystem.Discard(player, sourceCard as CardInstance);
    }

}




