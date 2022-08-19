using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Thoughts here,
//Our Zone Transition System will need the Add and Remove methods as it will need to be able to move cards between zones without
//knowing the exact details of the zone.

//Example

//Playing a Creature from Hand
//ZoneTransitionSystem.ChangeZones(CardInstance card, IZone zone1, IZone zone2);

//orrr

//Move to Lane

//UnitSummonSystem.SummonUnit(CardInstance card,IZone fromZone,Lane laneTo); //this way we could summon creatures from our hand, our discard pile
//or even some sort of random zone that we make up (The Twilight Zone!).
//SpellCastingSystem.CastSpell(CardInstance card,IZone fromZone); //needs to move it to the discard pile or whatever.
//same as above, we could be casting it from our hand, or we could be casting it from some other zone...

public interface IZone
{
    ZoneType ZoneType { get; }
    string Name { get; }
    List<CardInstance> Cards { get; }
    void Add(CardInstance card);
    void Remove(CardInstance card);
}

public enum ZoneType
{
    Discard,
    Hand,
    InPlay,
    Deck,
    Stack,
    Exile,
    Items
}

//A generic zone that can be used if no extra functionality is needed.
public class Zone : IZone, IEnumerable<CardInstance>
{

    protected ZoneType _zoneType;
    protected string _name;
    protected List<CardInstance> _cards;

    public ZoneType ZoneType { get { return _zoneType; } }

    public string Name { get { return _name; } }
    public List<CardInstance> Cards { get { return _cards; } }

    public Zone()
    {
        _cards = new List<CardInstance>();
    }

    public Zone(ZoneType zoneType, string name)
    {
        _zoneType = zoneType;
        _name = name;
        _cards = new List<CardInstance>();
    }

    public void Add(CardInstance card)
    {
        _cards.Add(card);
    }

    public void Remove(CardInstance card)
    {
        _cards.Remove(card);
    }

    public IEnumerator<CardInstance> GetEnumerator()
    {
        return ((IEnumerable<CardInstance>)_cards).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_cards).GetEnumerator();
    }
}


