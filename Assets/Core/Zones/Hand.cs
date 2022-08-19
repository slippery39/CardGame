using System.Collections.Generic;

public class Hand : Zone 
{
    public Hand()
    {
        _zoneType = ZoneType.Hand;
        _name = "Hand";
    }
}

public class GenericZone
{
    private List<CardInstance> _cards = new List<CardInstance>();

    private string _name = "";

    public List<CardInstance> Cards { get { return _cards; } }
    public string Name { get => _name; set => _name = value; }

    public void Add(CardInstance card)
    {
        _cards.Add(card);
    }
    public void Remove(CardInstance card)
    {
        _cards.Remove(card);
    }
}



