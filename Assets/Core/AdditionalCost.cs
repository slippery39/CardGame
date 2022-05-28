public abstract class AdditionalCost
{    public AdditionalCostType Type { get; set; }
     public int Amount { get; set; }

    public abstract string RulesText { get; }
    public abstract bool CanPay(CardGame cardGame, Player player, CardGameEntity source);

    public abstract void PayCost(CardGame cardGame, Player player,CardGameEntity sourceCard);
};

public class PayLifeAdditionalCost: AdditionalCost
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

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard)
    {
        player.Health -= Amount;
    }
}




