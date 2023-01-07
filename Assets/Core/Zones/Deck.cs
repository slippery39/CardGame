using System.Collections.Generic;
using System.Linq;

public class Deck : Zone
{
    public Deck(IEnumerable<CardInstance> cards)
    {
        _cards = cards.ToList();
        _name = "Deck";
        _zoneType = ZoneType.Deck;
    }
    public Deck()
    {
        _name = "Deck";
        _zoneType = ZoneType.Deck;
    }

    public void Shuffle()
    {
        _cards = _cards.Randomize().ToList();
    }

    public CardInstance GetTopCard()
    {
        //The top of our deck will be the last card added to the list.

        if (_cards.Count == 0)
        {
            return null;
        }
        
        var card = _cards[_cards.Count - 1];
        return card;
    }

    public void MoveToBottom(CardInstance card)
    {
        Remove(card);
        _cards.Insert(0, card);
    }
}