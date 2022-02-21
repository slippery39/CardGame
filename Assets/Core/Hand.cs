using System.Collections.Generic;

public class Hand : IZone
{

    private List<CardInstance> _cards = new List<CardInstance>();
    public string Name => "Hand";
    public List<CardInstance> Cards { get { return _cards; } }
    public void Add(CardInstance card)
    {
        _cards.Add(card);
    }
    public void Remove(CardInstance card)
    {
        _cards.Remove(card);
    }
}




