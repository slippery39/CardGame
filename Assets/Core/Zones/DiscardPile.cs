using System.Collections.Generic;
using System.Linq;

public class DiscardPile : Zone
{

    public DiscardPile(IEnumerable<CardInstance> cards)
    {
        _cards = cards.ToList();
        _name = "Hand";
        _zoneType = ZoneType.Discard;
    }
    public DiscardPile()
    {
        _zoneType = ZoneType.Discard;
        _name = "Discard";
    }
}

