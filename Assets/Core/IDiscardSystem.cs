using System.Collections.Generic;

public interface IDiscardSystem
{
    void Discard(Player player, CardInstance card);
    void Discard(Player player, List<CardInstance> cards);
}

public class DefaultDiscardSystem : IDiscardSystem
{
    private CardGame cardGame;

    public DefaultDiscardSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void Discard(Player player, CardInstance card)
    { 
        cardGame.ZoneChangeSystem.MoveToZone(card, player.DiscardPile);
    }

    public void Discard(Player player, List<CardInstance> cards)
    {
        foreach(var card in cards)
        {
            Discard(player, card);
        }
    }
}