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

        var allEntities = this._cardGame.GetZones().SelectMany(zone => zone.Cards);
        //var allEntityIds = this._cardGame.GetZones().SelectMany(zone => zone.Cards).Select(c => c.EntityId);
        var duplicates = allEntities.GroupBy(x => x.EntityId).Where(grp=>grp.Count()>1);
        var hasDups = duplicates.Any();

        if (hasDups)
        {
            //Note Telling Time had an issue with this because of how we were using the "Choices" property... If this occurs
            //again look there or in a simlar place.
            _cardGame.Logger.LogError("Invalid State Detected. Found more than one entity of the same id when checking all the zones");
            duplicates.ToList().ForEach(val =>
            {
                val.ToList().ForEach(card =>
                {
                    _cardGame.Logger.LogError($"{card.Name} ({card.EntityId}) for {card.GetOwner().Name} exists in {card.CurrentZone.Name}. Are they same refs? {val.ToList()[0] == val.ToList()[1]}");
                });
            });
        }
    }

    /*
    public void CheckForBadCopies()
    {
        if (!Enabled)
        {
            return;
        }

        var allCards = this._cardGame.GetZones().SelectMany(zone => zone.Cards);
        var badOnes = allCards.Where(card => card.CardGame.GameStateId != _cardGame.GameStateId);

        if (badOnes.Count() > 0)
        {
            //Note Telling Time had an issue with this because of how we were using the "Choices" property... If this occurs
            //again look there or in a simlar place.
            _cardGame.Logger.LogError($"Invalid State Detected. Cards found that were not part of this game state. Probably something went wrong when cloning the game state. Expected GameStateId : {_cardGame.GameStateId}");

            badOnes.ToList().ForEach(c =>
            {
                _cardGame.Logger.LogError($"{c.Name} {c.EntityId} : {c.CurrentZone.Name} is from game state {c.CardGame.GameStateId}");
            });
        }
    }
    */

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

    private void CheckForNullsInList<T>(List<T> list, string messageIfFound)
    {
        if (list.Contains(default(T)))
        {
            _cardGame.Logger.LogError(messageIfFound);
        }
    }
}