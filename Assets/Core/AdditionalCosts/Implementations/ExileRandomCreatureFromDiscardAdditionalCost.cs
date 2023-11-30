using System.Linq;

public class ExileRandomCreatureFromDiscardAdditionalCost : AdditionalCost
{
    public override string RulesText => "Exile a random unit from your graveyard";

    public ExileRandomCreatureFromDiscardAdditionalCost()
    {
        Type = AdditionalCostType.ExileRandomCreatureFromDiscard;
    }

    public override bool CanPay(CardGame cardGame, Player player, CardGameEntity source)
    {
        return player.DiscardPile.Cards.Where(card => card.IsOfType<UnitCardData>()).Any();
    }

    public override void PayCost(CardGame cardGame, Player player, CardGameEntity sourceCard, CostInfo costInfo)
    {
        var cardChosen = player.DiscardPile.Cards.Where(card => card.IsOfType<UnitCardData>()).Randomize().FirstOrDefault();
        cardGame.ZoneChangeSystem.MoveToZone(cardChosen, player.Exile);
    }
}




