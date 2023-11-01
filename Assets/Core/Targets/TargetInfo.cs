using System;
using System.Collections.Generic;
using System.Linq;
//Our new target type might have the following:
/*
 * None,
 * PlayerSelf,
 * Opponent,
 * Units,
 * CardsInHand,
 * UnitSelf,
 * OpenLane,
 * OpenLaneBesidenUnit
 */

/// <summary>
/// In progress class to replace our TargetType enum and make it more flexible.
/// </summary>
public class TargetInfo
{
    /// <summary>
    /// Used to determine the overall type of target. Temporary while we refactor this out
    /// </summary>
    public TargetType TargetType { get; set; }

    /// <summary>
    /// Whose cards should be able to chosen as targets. (Any, Ours or Theirs)
    /// ex. All creatures get +1/+1, Our creatures get +1/+!, Their creatures get +1/+1
    /// </summary>
    public TargetOwnerType OwnerType { get; set; } = TargetOwnerType.Any;

    public TargetMode TargetMode { get; set; } = TargetMode.Target;
    /// <summary>
    /// A class to define if we should filter the original target info by Type / Subtype / Colors etc..
    /// </summary>
    public CardFilter TargetFilter { get; set; }

    /// <summary>
    /// Helper method, checkes whether this TargetInfo represents an effect that needs targets
    /// </summary>
    public bool NeedsTargets => TargetMode == TargetMode.Target;

    /// <summary>
    /// Number of targets required for the effect in question. Note that our codebase does not currently support cards with multiple targets.
    /// This may change in the future.
    /// Note that this is also the number of entities that will be affected by a random selection as well.
    /// Does not apply if TargetMode is All or None
    /// </summary>
    public int NumberOfTargets { get; set; } = 1;

    /// <summary>
    /// This grabs the targets from a cardGame based on the TargetInfo, but before any systems have done their modifications.
    /// For example, Hexproof 
    /// </summary>
    /// <param name="cardGame"></param>
    /// <param name="player"></param>
    /// <param name="effectSource"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public IEnumerable<CardGameEntity> GetTargets(CardGame cardGame, Player player, CardGameEntity effectSource)
    {
        var baseTargets = GetTargetsInternal(cardGame, player, effectSource);
        baseTargets = FilterByOwnerType(player, baseTargets);
        baseTargets = FilterByTargetMode(baseTargets);
        return baseTargets;
    }

    private IEnumerable<CardGameEntity> FilterByOwnerType(Player player, IEnumerable<CardGameEntity> baseTargets)
    {
        if (OwnerType == TargetOwnerType.Ours)
        {
            baseTargets = baseTargets.Where(target => target.IsOwnedBy(player));
        }
        else if (OwnerType == TargetOwnerType.Theirs)
        {
            baseTargets = baseTargets.Where(target => !target.IsOwnedBy(player));
        }
        //Note - OwnerType.Any would have no changes to the original list here.
        return baseTargets;
    }

    private IEnumerable<CardGameEntity> FilterByTargetMode(IEnumerable<CardGameEntity> baseTargets)
    {
        if (TargetMode == TargetMode.Random)
        {
            baseTargets = baseTargets.Randomize().Take(NumberOfTargets);
        }
        return baseTargets;
    }

    private IEnumerable<CardGameEntity> GetTargetsInternal(CardGame cardGame, Player player, CardGameEntity effectSource)
    {
        //TODO - Filter by OwnerType
        //Differentiating between TargetTypes that need targets vs TargetTypes that don't at the moment.
        //In the future this might change.

        //Handled in both TargetEffects and NonTaregetedEffects
        if (TargetType == TargetType.Units)
        {
            return GetUnits(cardGame);
        }
        else if (TargetType == TargetType.UnitsAndPlayers)
        {
            return GetUnits(cardGame).Concat(GetPlayers(cardGame));
        }
        else if (TargetType == TargetType.Players)
        {
            return GetPlayers(cardGame);
        }

        //TODO - This is the old way of doing things that needs to slowly be refactored as we update our TargetInfo class and Cards that use it.
        switch (TargetType)
        {
            case TargetType.None:
                return new List<CardGameEntity> { effectSource };
            case TargetType.PlayerSelf:
                {
                    return new List<CardGameEntity> { player };
                }
            case TargetType.AllUnits:
                {
                    return cardGame.GetUnitsInPlay().Cast<CardGameEntity>().ToList();
                }
            case TargetType.OurUnits:
                return player.Lanes.Where(l => !l.IsEmpty()).Select(l => l.UnitInLane).Cast<CardGameEntity>().ToList();
            case TargetType.OpponentUnits:
                return cardGame.Players.Find(p => player.PlayerId != p.PlayerId).Lanes.Where(l => !l.IsEmpty()).Select(l => l.UnitInLane).Cast<CardGameEntity>().ToList();
            case TargetType.UnitSelf:
                return new List<CardGameEntity>() { effectSource };
            case TargetType.Opponent:
                return cardGame.Players.Where(p => p.EntityId != player.EntityId).Cast<CardGameEntity>().ToList();
            case TargetType.RandomOurUnits:
                var ourUnits = player.Lanes.Where(l => !l.IsEmpty()).Select(l => l.UnitInLane).Randomize();
                var filtered = CardFilter.ApplyFilter(ourUnits.ToList(), TargetFilter);
                return new List<CardGameEntity> { filtered.FirstOrDefault() };
            case TargetType.RandomOpponentOrUnits:
                var opponent = cardGame.Players.Find(p => p.EntityId != player.EntityId);
                var things = new List<CardGameEntity> { opponent };

                var everything = things.Union(opponent.GetUnitsInPlay());

                if (!everything.Any())
                {
                    return new List<CardGameEntity> { };
                }
                return new List<CardGameEntity> { everything.Randomize().First() };
            default:
                throw new Exception($"Wrong target type to call in GetEntitiesToApplyEffect : {TargetType}");
        }
        throw new Exception($"Could not find a way to handle {TargetType} in GetTargets()");

    }


    private IEnumerable<CardGameEntity> GetPlayers(CardGame cardGame)
    {
        return cardGame.Players.Cast<CardGameEntity>().ToList();
    }

    private IEnumerable<CardGameEntity> GetUnits(CardGame cardGame)
    {
        return cardGame.GetEntities<Lane>().Where(lane => !lane.IsEmpty()).Select(lane => lane.UnitInLane);
    }
}


