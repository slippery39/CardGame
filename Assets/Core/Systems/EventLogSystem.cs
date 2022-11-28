using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

public class GameEventLog
{
    public int EventId { get; set; } //every game event should have a unique id
    public string Log { get; set; }
}

public class EventLogSystem : CardGameSystem
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
}

