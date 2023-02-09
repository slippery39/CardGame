using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

public interface IGameEventSystem
{
    public IObservable<GameEvent> GameEventObservable { get; }
    public void FireEvent(GameEvent gameEvent);

    public void FireGameStateUpdatedEvent();
}

//Used if we are using our card game for the AI. 
//No need for events there.
public class EmptyGameEventSystem : CardGameSystem, IGameEventSystem
{
    private Subject<GameEvent> gameEventSubject = new Subject<GameEvent>();

    public IObservable<GameEvent> GameEventObservable => gameEventSubject.AsObservable();

    public void FireGameStateUpdatedEvent()
    {

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
        gameEvent.EventId = nextEventId++;
        gameEventSubject.OnNext(gameEvent);
    }

    public void FireGameStateUpdatedEvent()
    {
        gameEventSubject.OnNext(new GameStateUpdatedEvent
        {
            EventId = nextEventId++,
            ResultingState = cardGame.Copy()
        });
    }
}


public abstract class GameEvent
{
    public int EventId { get; set; }
}

//Note if we just store the attacker and defender as references then 
//we wont be able to ensure that they will be the same by the time the
//UI acts on the event.

public class AttackGameEvent : GameEvent
{
    public int AttackerId { get; set; }
    public int DefenderId { get; set; }
}

public class DamageEvent : GameEvent
{
    public int DamagedId { get; set; }
    public int DamageAmount { get; set; }
}

public class DrawCardEvent : GameEvent
{
    public int PlayerId { get; set; }
    public int DrawnCardId { get; set; }
}

public class UnitDiedEvent : GameEvent
{
    public int UnitId { get; set; }
}

public class GameStateUpdatedEvent : GameEvent
{
    public CardGame ResultingState { get; set; }
}

//Game Events needed (phase 1)
//UnitAttack
//EntityDamaged
//Draw Opening Hand
//GameStart
//GameEnd
//Draw Card
//Turn Start

