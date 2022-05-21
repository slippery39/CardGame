using System.Collections.Generic;
using System.Linq;

public class Deck : IZone
{
    public string Name => "Deck";

    private List<CardInstance> cards = new List<CardInstance>();

    public List<CardInstance> Cards => cards;

    public void Add(CardInstance card)
    {
        cards.Add(card);
    }

    public void Remove(CardInstance card)
    {
        cards.Remove(card);
    }

    public void Shuffle()
    {
        cards = cards.Randomize().ToList();
    }

    public CardInstance GetTopCard()
    {
        //The top of our deck will be the last card added to the list.
        var card = cards[cards.Count - 1];
        return card;
    }
}