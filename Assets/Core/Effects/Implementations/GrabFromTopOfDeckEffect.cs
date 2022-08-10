using System.Collections.Generic;
using System.Linq;

public class GrabFromTopOfDeckEffect : Effect
{
    public override string RulesText
    {
        get
        {
            var str = $"draw up to {Amount} #cardType# from the top {CardsToLookAt} cards of your deck";

            if (CardsToLookAt == 1)
            {
                str = $"draw a #cardType# from the top {CardsToLookAt} card of your deck";
            }
            else if (CardsToLookAt == Amount)
            {
                str = $"draw all #cardType# from the top {CardsToLookAt} cards of your deck";
            }
            else if (Amount == 1)
            {
                str = $"draw a #cardType# from the top {CardsToLookAt} cards of your deck";
            }

            var needsPlural = Amount > 1;

            if (Filter != null)
            {
                return str.Replace("#cardType#", Filter.RulesTextString(needsPlural).ToLower());
            }
            else
            {
                var cardStr = needsPlural ? "cards" : "card";
                return str.Replace("#cardType#", cardStr);
            }
        }
    }
    public int CardsToLookAt { get; set; }
    public int Amount { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.None;
    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        cardGame.CardDrawSystem.GrabFromTopOfDeck(player, Filter, CardsToLookAt, Amount);
    }
}



