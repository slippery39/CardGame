using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


//Thoughts here,
//Our Zone Transition System will need the Add and Remove methods as it will need to be able to move cards between zones without
//knowing the exact details of the zone.

//Example

//Playing a Creature from Hand
//ZoneTransitionSystem.ChangeZones(CardInstance card, IZone zone1, IZone zone2);

//

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
}

//A generic zone that can be used if no extra functionality is needed.
//Hand and Discard both are sort of non unique zones.

[JsonObject]
public class Zone : IZone, IEnumerable<CardInstance> //, /*ISerializable*/
{
    protected ZoneType _zoneType;
    protected string _name;
    protected List<CardInstance> _cards;

    public ZoneType ZoneType { get { return _zoneType; } set { _zoneType = value; } }

    public string Name { get { return _name; } }
    public List<CardInstance> Cards { get { return _cards; } set { _cards = value; } }

    public Zone()
    {
        _cards = new List<CardInstance>();
    }

    //Checking if this makes JSONnet Serialization work
    public Zone(IEnumerable<CardInstance> cards)
    {
        _cards = cards.ToList();
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

    /*
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        throw new NotImplementedException();
    }
    */
}


