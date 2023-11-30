public class PayLifeAdditionalCost : AdditionalCost
{
    public override string RulesText => $@"Pay {Amount} Life";

    public PayLifeAdditionalCost()
    {
        Type = AdditionalCostType.PayLife;
    }

    public override bool CanPay(CardGame cardGame, Player player, CardGameEntity source)
    {
        return player.Health >= Amount;
    }

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo)
    {
        player.Health -= Amount;
    }
}




