using System.Collections.Generic;

public interface IDiscardSystem
{
    void Discard(CardGame cardGame, Player player, CardInstance card);
    void Discard(CardGame cardGame, Player player, List<CardInstance> cards);
}

public class DefaultDiscardSystem : IDiscardSystem
{
    public void Discard(CardGame cardGame, Player player, CardInstance card)
    { 
        cardGame.ZoneChangeSystem.MoveToZone(cardGame, card, player.DiscardPile);
    }

    public void Discard(CardGame cardGame, Player player, List<CardInstance> cards)
    {
        foreach(var card in cards)
        {
            Discard(cardGame, player, card);
        }
    }
}