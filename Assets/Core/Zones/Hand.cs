using System.Collections.Generic;
using System.Linq;

public class Hand : Zone 
{
    public Hand(IEnumerable<CardInstance> cards)
    {
        _cards = cards.ToList();
        _name = "Hand";
        _zoneType = ZoneType.Hand;
    }
      

    public Hand()
    {
        _zoneType = ZoneType.Hand;
        _name = "Hand";
    }
}




