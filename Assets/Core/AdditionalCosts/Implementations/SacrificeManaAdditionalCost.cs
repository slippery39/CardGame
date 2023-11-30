public class SacrificeManaAdditionalCost : AdditionalCost
{
    public override string RulesText => $@" Sacrifice {Amount} mana";

    public SacrificeManaAdditionalCost()
    {
        Type = AdditionalCostType.SacrificeMana;
    }

    public override bool CanPay(CardGame cardGame, Player player, CardGameEntity source)
    {
        return player.ManaPool.TotalColorlessMana >= Amount;
    }

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo)
    {
        player.ManaPool.TotalColorlessMana -= Amount;
    }
}




