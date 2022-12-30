using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

public enum EventLogType
{
    Regular,
    GameOver
}

public class GameEventLog
{
    public int EventId { get; set; } //every game event should have a unique id
    public string Log { get; set; }
    public EventLogType Type { get; set; }

    public GameEventLog()
    {
        Type = EventLogType.Regular;
    }
}

public class GameOverEventLog : GameEventLog
{
    public GameOverEventLog()
    {
        Type = EventLogType.GameOver;
    }
}


public interface IEventLogSystem
{
    IObservable<GameEventLog> GetGameEventLogsAsObservable();
    public List<GameEventLog> Events { get; set; }
    public int NextEventId { get; set; }
    public void AddEvent(string log);
    public void AddGameOverEvent();
}

public class EmptyEventLogSystem : CardGameSystem, IEventLogSystem
{
    public List<GameEventLog> Events { get; set; }
    public int NextEventId { get; set; }

    public void AddEvent(string log)
    {
    }

    public void AddGameOverEvent()
    {

    }

    public IObservable<GameEventLog> GetGameEventLogsAsObservable()
    {
        return Observable.Create<GameEventLog>(observer =>
        {
            return Disposable.Empty;
        });
    }
}

public class EventLogSystem : CardGameSystem, IEventLogSystem
{
    public int NextEventId { get; set; } = 1;
    public List<GameEventLog> Events { get; set; } = new List<GameEventLog>();
    private ISubject<GameEventLog> eventsAsSubject = new Subject<GameEventLog>();

    public EventLogSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public IObservable<GameEventLog> GetGameEventLogsAsObservable()
    {
        return eventsAsSubject.AsObservable();
    }
    public void AddEvent(string log)
    {
        var eventLog = new GameEventLog()
        {
            EventId = NextEventId++,
            Log = log
        };

        Events.Add(eventLog);

        eventsAsSubject.OnNext(eventLog);
    }

    public void AddGameOverEvent()
    {
        var gameOverEvent = new GameOverEventLog()
        {
            EventId = NextEventId++,
            Log = "Game over!"
        };

        Events.Add(gameOverEvent);
        eventsAsSubject.OnNext(gameOverEvent);
    }
}

