using System.Collections.Generic;
using System.Linq;



public class DebugSystem
{
    public bool Enabled { get; set; } = true;
    private readonly CardGame _cardGame;
    public DebugSystem(CardGame cardGame)
    {
        _cardGame = cardGame;
    }

    /// <summary>
    ///  Does a check to see if any entity ids are duplicated. We have run into some issues where this can happen and they shouldn't
    ///  and they are really hard to debug. Hopefully this helps catch these situations.
    /// </summary>
    public void CheckForInvalidEntities()
    {
        if (!Enabled)
        {
            return;
        }

        var allEntityIds = this._cardGame.GetZones().SelectMany(zone => zone.Cards).Select(c => c.EntityId);
        var duplicates = allEntityIds.GroupBy(x => x).Any(grp => grp.Count() > 1);

        if (duplicates)
        {
            //Note Telling Time had an issue with this because of how we were using the "Choices" property... If this occurs
            //again look there or in a simlar place.
            _cardGame.Logger.LogError("Invalid State Detected. Found more than one entity of the same id when checking all the zones");
        }
    }

    public void CheckForNullModificationsOnPlayers()
    {
        if (!Enabled)
        {
            return;
        }
        CheckForNullsInList(_cardGame.Player1.Modifications, "Invalid State Detected.Null Modification Detected on Player 1");
        CheckForNullsInList(_cardGame.Player1.ContinuousEffects, "Invalid State Detected.Null Continuous Effects Detected on Player 1");

        CheckForNullsInList(_cardGame.Player2.Modifications, "Invalid State Detected.Null Modification Detected on Player 2");
        CheckForNullsInList(_cardGame.Player2.ContinuousEffects, "Invalid State Detected.Null Continuous Effects Detected on Player 2");
    }

    private void CheckForNullsInList<T>(List<T> list,string messageIfFound)
    {
        if (list.Contains(default(T)))
        {
            _cardGame.Logger.LogError(messageIfFound);
        }
    }
}