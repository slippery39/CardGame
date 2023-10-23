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
            _cardGame.Logger.LogError("Invalid State Detected. Found more than one entity of the same id when checking all the zones");
        }
    }
}