using System;

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

public class SacrificeSelfAdditionalCost: AdditionalCost
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
        return cardGame.GetZoneOfCard(source as CardInstance).Name.ToLower() == "lane";
    }

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard)
    {
        if (!(sourceCard is CardInstance))
        {
            throw new Exception("Source should be a card instance for a SacrificeSelfAdditionalCost");
        }

        cardGame.SacrificeSystem.SacrificeUnit(cardGame,player,sourceCard as CardInstance);
    }
}




