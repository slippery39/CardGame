public interface ICardDrawSystem
{
    void DrawCard(CardGame cardGame, Player player);
}


public class DefaultCardDrawSystem : ICardDrawSystem
{

    //TODO - What happens if we have too many cards in our hand?
    //TODO - What happens if we draw a card with no cards left in our deck?
    public void DrawCard(CardGame cardGame, Player player)
    {
        if (player.Deck.Cards.Count > 0)
        {
            var card = player.Deck.GetTopCard();
            cardGame.ZoneChangeSystem.MoveToZone(cardGame, card, player.Hand);
        }
    }
}
