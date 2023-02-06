﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

public interface IGameEventSystem
{
    public IObservable<GameEvent> GameEventObservable { get; }
    public void FireEvent(GameEvent gameEvent);

    public GameEvent CreateAttackEvent(int attackerId, int defenderId);
}

//Used if we are using our card game for the AI. 
public class EmptyGameEventSystem : CardGameSystem, IGameEventSystem
{
    private Subject<GameEvent> gameEventSubject = new Subject<GameEvent>();

    public IObservable<GameEvent> GameEventObservable => gameEventSubject.AsObservable();

    public GameEvent CreateAttackEvent(int attackerId, int defenderId)
    {
        return null;
    }

    public void FireEvent(GameEvent gameEvent)
    {

    }
}

public class GameEventSystem : CardGameSystem, IGameEventSystem
{
    private int nextEventId = 1;
    public GameEventSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }

    private Subject<GameEvent> gameEventSubject = new Subject<GameEvent>();
    public IObservable<GameEvent> GameEventObservable { get => gameEventSubject.AsObservable(); }
    public void FireEvent(GameEvent gameEvent)
    {
        gameEventSubject.OnNext(gameEvent);
    }

    public GameEvent CreateAttackEvent(int attackerId, int defenderId)
    {
        return new AttackGameEvent
        {
            EventId = nextEventId++,
            AttackerId = attackerId,
            DefenderId = defenderId,
            ResultingState = cardGame.Copy()
        };
    }
}

public abstract class GameEvent
{
    public int EventId { get; set; }
    public CardGame ResultingState { get; set; }
}

//Note if we just store the attacker and defender as references then 
//we wont be able to ensure that they will be the same by the time the
//UI acts on the event.
public class AttackGameEvent : GameEvent
{
    public int AttackerId { get; set; }
    public int DefenderId { get; set; }
}

//Game Events needed (phase 1)
//UnitAttack
//EntityDamaged
//Draw Opening Hand
//GameStart
//GameEnd
//Draw Card
