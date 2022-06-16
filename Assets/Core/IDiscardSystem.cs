public interface IDiscardSystem
{
    void Discard(CardGame cardGame, Player player, CardInstance card);
}

public class DefaultDiscardSystem : IDiscardSystem
{
    public void Discard(CardGame cardGame, Player player, CardInstance card)
    {
        cardGame.ZoneChangeSystem.MoveToZone(cardGame, card, player.DiscardPile);
    }
}