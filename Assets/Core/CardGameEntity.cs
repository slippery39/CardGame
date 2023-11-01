﻿

using System.Collections.Generic;
/**
* 
* Entities are things in our card game which can be interacted with.
* 
* For example : Card, Players, Zones etc..
* 
* Each entity should have an Id which is generated by the game upon creation.
*/
public abstract class CardGameEntity
{
    public virtual string Name { get; set; }
    public int EntityId { get; set; }
    public List<ContinuousEffect> ContinuousEffects { get; set; } = new List<ContinuousEffect>();
    public List<Modification> Modifications { get; set; } = new List<Modification>();
    public int OwnerId { get; set; }

    public void RemoveModification(Modification modToRemove)
    {
        Modifications.Remove(modToRemove);
    }

    public bool IsOwnedBy(Player player)
    {
        return OwnerId == player.PlayerId|| EntityId == player.EntityId;
    }
}