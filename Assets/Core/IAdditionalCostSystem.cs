public interface IAdditionalCostSystem
{
    //TODO - finish this interface
    public void PayAdditionalCost(Player player, CardInstance card, AdditionalCost additionalCost, CostInfo costInfo);
    public bool CanPayAdditionalCost(Player player, CardInstance source, AdditionalCost cost);

}


public class DefaultAdditionalCostSystem : IAdditionalCostSystem
{
    CardGame cardGame;

    public DefaultAdditionalCostSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void PayAdditionalCost(Player player, CardInstance card, AdditionalCost additionalCost, CostInfo costInfo)
    {
        if (additionalCost == null) { return; }
        additionalCost.PayCost(cardGame, player, card, costInfo);
    }
    public bool CanPayAdditionalCost(Player player, CardInstance source, AdditionalCost cost)
    {
        return cost.CanPay(cardGame, player, source);
    }
}