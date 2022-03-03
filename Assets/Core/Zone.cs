using System;
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
    string Name { get; }
    List<CardInstance> Cards { get;}

    void Add(CardInstance card);
    //We may not have the actual card reference for whatever reason. 
    //So search for a UID instead?
    //Not sure the best way to deal with this.
    void Remove(CardInstance card);

}




