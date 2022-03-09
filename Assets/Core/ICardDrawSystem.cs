public interface ICardDrawSystem
{
    void DrawCard(CardGame cardGame, Player player);
}


public class DefaultCardDrawSystem : ICardDrawSystem
{
    public void DrawCard(CardGame cardGame, Player player)
    {
        var card = player.Deck.GetTopCard();
        cardGame.ZoneChangeSystem.MoveToZone(cardGame, card, player.Hand);
    }
}
