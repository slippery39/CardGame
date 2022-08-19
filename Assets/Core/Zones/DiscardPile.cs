using System.Collections.Generic;

public class DiscardPile : Zone
{
    public DiscardPile()
    {
        _zoneType = ZoneType.Discard;
        _name = "Discard";
    }
}

