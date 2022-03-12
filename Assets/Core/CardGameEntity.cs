﻿/**
 * 
 * Entities are things in our card game which can be interacted with.
 * 
 * For example : Card, Players, Zones etc..
 * 
 * Each entity should have an Id which is generated by the game upon creation.
 */

public abstract class CardGameEntity
{
    public int EntityId { get; set; }
}