using System.Collections.Generic;

public class DiscardPile : IZone
{
    private List<CardInstance> _cards = new List<CardInstance>();
    public string Name => "Discard";
    public List<CardInstance> Cards => _cards;
    public ZoneType ZoneType => ZoneType.Discard;

    public void Add(CardInstance card)
    {
        _cards.Add(card);
    }

    public void Remove(CardInstance card)
    {
        _cards.Remove(card);
    }
}

