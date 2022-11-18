using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameEventLog
{
    public int EventId { get; set; } //every game event should have a unique id
    public string Log { get; set; }
}

public class EventLogSystem
{
    public int NextEventId { get; set; } = 1;
    public List<GameEventLog> Events { get; set; } = new List<GameEventLog>();
    public void AddEvent(string log)
    {
        this.Events.Add(new GameEventLog
        {
            EventId = NextEventId++,
            Log = log
        });

    }
}

