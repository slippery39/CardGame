using System.Collections.Generic;
using System.Linq;

public class GrabFromTopOfDeckEffect : Effect
{
    public override string RulesText
    {
        get
        {
            var str = $"draw up to {Amount} #cardType# from the top {CardsToLookAt} cards of your deck";

            if (Filter?.CreatureType != null)
            {
                return str.Replace("#cardType#", Filter.CreatureType);
            }
            else
            {
                return str.Replace("#cardType#", "card");
            }
        }
    }
    public int CardsToLookAt { get; set; }
    public int Amount { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.None;

    public CardFilter Filter { get; set; }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        var validCardsToGet = player.Deck.Cards.Where(card =>
        {
            if (Filter.CreatureType != null)
            {
                return card.CreatureType == Filter.CreatureType;
            }
            return true;
        });

        cardGame.CardDrawSystem.GrabRandomCardFromDeck(player, Filter);
    }
}



