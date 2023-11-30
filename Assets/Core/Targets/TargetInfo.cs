using System;
using System.Collections.Generic;
using System.Linq;

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
    /// Creates a TargetInfo that is equal in behaviour to "Each Unit you Control"
    /// </summary>
    /// <returns></returns>
    public static TargetInfo EachUnitYouControl()
    {
        var info = new TargetInfo();
        info.TargetType = TargetType.Units;
        info.TargetMode = TargetMode.All;
        info.OwnerType = TargetOwnerType.Ours;
        return info;
    }

    /// <summary>
    /// Creates a TargetInfo that is equal in behaviour to "Each Opponent Unit"
    /// </summary>
    /// <returns></returns>
    public static TargetInfo EachOpponentUnit()
    {
        var info = new TargetInfo();
        info.TargetType = TargetType.Units;
        info.TargetMode = TargetMode.All;
        info.OwnerType = TargetOwnerType.Theirs;
        return info;
    }

    /// <summary>
    /// Creates a TargetInfo that is equal in behaviour to "Target Opponent Unit"
    /// </summary>
    /// <returns></returns>
    public static TargetInfo TargetOpponentUnit()
    {
        var info = new TargetInfo();
        info.TargetType = TargetType.Units;
        info.TargetMode = TargetMode.Target;
        info.OwnerType = TargetOwnerType.Theirs;
        return info;
    }

    /// <summary>
    /// Creates a TargetInfo that is equal in behaviour to "Target Own Unit"
    /// </summary>
    /// <returns></returns>
    public static TargetInfo TargetOwnUnit()
    {
        var info = new TargetInfo
        {
            TargetType = TargetType.Units,
            TargetMode = TargetMode.Target,
            OwnerType = TargetOwnerType.Ours,
        };
        return info;
    }

    /// <summary>
    /// Creates a TargetInfo that is equal in behaviour to "Target Any Unit"
    /// </summary>
    /// <returns></returns>
    public static TargetInfo TargetAnyUnit()
    {
        var info = new TargetInfo
        {
            TargetType = TargetType.Units,
            TargetMode = TargetMode.Target,
            OwnerType = TargetOwnerType.Any
        };
        return info;
    }

    /// <summary>
    /// Creates a TargetInfo that is equal in behaviour to "Target Opponent or Their Units"
    /// </summary>
    /// <returns></returns>
    public static TargetInfo TargetOpponentOrTheirUnits()
    {
        var info = new TargetInfo
        {
            TargetType = TargetType.UnitsAndPlayers,
            TargetMode = TargetMode.Target,
            OwnerType = TargetOwnerType.Theirs
        };
        return info;
    }

    /// <summary>
    /// Creates a TargetInfo that is equal in behaviour to "Random Opponent or Their Units"
    /// </summary>
    /// <returns></returns>
    public static TargetInfo RandomOpponentOrUnits()
    {
        var info = new TargetInfo
        {
            TargetType = TargetType.UnitsAndPlayers,
            TargetMode = TargetMode.Random,
            OwnerType = TargetOwnerType.Theirs,
        };
        return info;
    }

    /// <summary>
    /// Creates a TargetInfo that is equal in behaviour to "Your Opponent"
    /// </summary>
    /// <returns></returns>
    public static TargetInfo Opponent()
    {
        var info = new TargetInfo
        {
            TargetType = TargetType.Players,
            TargetMode = TargetMode.All,
            OwnerType = TargetOwnerType.Theirs
        };
        return info;
    }

    public static TargetInfo Source()
    {
        var info = new TargetInfo
        {
            TargetType = TargetType.Source, //non target type attributes shouldn't matter here
            TargetMode = TargetMode.None,
            OwnerType = TargetOwnerType.Any
        };
        return info;
    }

    public static TargetInfo PlayerSelf()
    {
        var info = new TargetInfo
        {
            TargetType = TargetType.PlayerSelf, //non target type attributes shouldn't matter here
            TargetMode = TargetMode.None,
            OwnerType = TargetOwnerType.Ours
        };
        return info;
    }

    /// <summary>
    /// Convenience method to help create a TargetType in a declarative way. 
    /// ex. TargetInfo.OpponentUnits().WithUnitType("Goblin");
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns></returns>
    public TargetInfo WithUnitType(string unitType)
    {
        if (TargetFilter == null)
        {
            TargetFilter = new CardFilter();
        }
        TargetFilter.CreatureType = unitType;
        return this;
    }

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
        baseTargets = FilterByCardFilter(baseTargets);
        //Target Mode needs to be the last filter since we are actually taking values there.
        //We had an issue with Restoration Angel where it would select itself, then be filtered out
        //by the non angel criteria.Other option is to have a seperate method for random selections 
        //and explciitly apply it at the end here.
        baseTargets = FilterByTargetMode(baseTargets);

        return baseTargets;
    }

    /// <summary>
    /// Creates the rules text for this specific target info object.    /// 
    /// </summary>
    /// <param name="actionStr">Optionally in a string so that the method can decide if it needs to be plural or not.
    /// Currently only used for the "Players" TargetType.
    /// Ex. You create / An opponent creates</param>
    /// <returns></returns>

    public string GetRulesText(string actionStr = null)
    {
        //Note that there are way too many different combinations of effects and target types and other stuff..
        //We are building this as we go, some combinations may not be accounted for and therefore may show up incorrect 
        //on the actual card.

        string retStr = "";

        //Quick hack to properly show the card type if we don't have a TargetFilter.
        string defaultCardType = "";

        bool hasActionStr = !actionStr.IsNullOrEmpty();

        string amountStr = NumberOfTargets > 1 ? NumberOfTargets.ToString() : "a";
        string pluralStr = NumberOfTargets > 1 ? "s" : "";

        switch (TargetType)
        {
            case TargetType.PlayerSelf:
                retStr = "You";
                if (hasActionStr)
                {
                    retStr += $" {actionStr}";
                }
                break;
            case TargetType.Players:
                if (OwnerType == TargetOwnerType.Ours)
                {
                    retStr = $"You";
                    if (hasActionStr)
                    {
                        retStr += $" {actionStr}";
                    }

                }
                else if (OwnerType == TargetOwnerType.Theirs)
                {
                    retStr = "Your opponent ";
                    if (hasActionStr)
                    {
                        //Making plural here, ex. Your opponent creates
                        retStr += $" {actionStr}s";
                    }
                }
                else
                {
                    retStr = "Players";
                    if (hasActionStr)
                    {
                        retStr = $" {actionStr}";
                    }
                }
                break;
            case TargetType.Units:
                defaultCardType = "unit";
                if (TargetMode == TargetMode.All)
                {
                    retStr = "each #cardType# #ownerType#";
                }
                if (TargetMode == TargetMode.Random)
                {
                    retStr = "random #cardType# #ownerType#";
                }
                if (TargetMode == TargetMode.Target)
                {
                    retStr = "a #cardType# #ownerType#";
                }
                break;
            case TargetType.UnitsAndPlayers:
                defaultCardType = "unit";
                if (TargetMode == TargetMode.All)
                {
                    if (OwnerType == TargetOwnerType.Any)
                    { 
                        retStr = "each #cardType# and player";
                    }
                    else if (OwnerType == TargetOwnerType.Theirs)
                    {
                        retStr = "each enemy #cardType# and opponent";
                    }
                }
                if (TargetMode == TargetMode.Random)
                {
                    if (OwnerType == TargetOwnerType.Any)
                    {
                        retStr = $"{amountStr} randomly chosen #cardType#{pluralStr} or player{pluralStr}";
                    }
                    else if (OwnerType == TargetOwnerType.Theirs)
                    {
                        retStr = $"{amountStr} randomly chosen enemy #cardType#{pluralStr} or opponent{pluralStr}";
                    }
                }
                if (TargetMode == TargetMode.Target)
                {
                    if (OwnerType == TargetOwnerType.Any)
                    {
                        retStr = "a #cardType# or player";
                    }
                    else if (OwnerType == TargetOwnerType.Theirs)
                    {
                        retStr = "an enemy #cardType# or opponent";
                    }
                }
                break;
            case TargetType.Source:
                retStr = "this";
                break;
            default:
                retStr = $"Rules Text not implemented yet in TargetInfo for {TargetType}";
                break;
        }

        //Factor in our card type
        if (TargetFilter != null)
        {
            retStr = retStr.Replace("#cardType#", TargetFilter.RulesTextString().Trim().ToLower());
        }
        else
        {
            retStr = retStr.Replace("#cardType#", defaultCardType);
        }

        //Factor in our owner type
        if (OwnerType == TargetOwnerType.Theirs)
        {
            retStr = retStr.Replace("#ownerType#", "your opponent controls");
        }
        else if (OwnerType == TargetOwnerType.Ours)
        {
            retStr = retStr.Replace("#ownerType#", "you control");
        }
        else
        {
            retStr = retStr.Replace("#ownerType#", "");
        }

        return retStr;
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

    private IEnumerable<CardGameEntity> FilterByCardFilter(IEnumerable<CardGameEntity> baseTargets)
    {
        if (TargetFilter == null)
        {
            return baseTargets;
        }

        return TargetFilter.ApplyFilter(baseTargets);
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
        else if (TargetType == TargetType.CardsInHand)
        {
            return GetPlayers(cardGame).Cast<Player>().SelectMany(p => p.Hand.Cards);
        }
        else if (TargetType == TargetType.Source)
        {
            return new List<CardGameEntity> { effectSource };
        }
        else if (TargetType == TargetType.PlayerSelf)
        {
            return new List<CardGameEntity> { player };
        }

        //TODO - This is the old way of doing things that needs to slowly be refactored as we update our TargetInfo class and Cards that use it.
        switch (TargetType)
        {
            case TargetType.None:
                return new List<CardGameEntity> { effectSource };
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


public class TargetInfoWithComponents
{
    /// <summary>
    /// The type of targetting that will be used.
    /// </summary>
    public TargetMode TargetMode { get; set; }

    /// <summary>
    /// The amount of targets that need to be selected
    /// </summary>
    public int Amount { get; set; }

    public List<TargetInfoFilter> TargetFilters = new List<TargetInfoFilter>();

    public IEnumerable<CardGameEntity> GetValidTargets(CardGame gameState, TargetSourceInfo sourceInfo)
    {
        var entities = gameState.GetEntities();
        return entities.Intersect(
            TargetFilters.SelectMany(tf => tf.GetTargets(gameState, sourceInfo))
        );
    }
}


public class TargetSourceInfo
{
    /// <summary>
    /// The source player for a target info
    /// </summary>
    public Player SourcePlayer { get; set; }

    /// <summary>
    /// The source card for a target info
    /// </summary>
    public CardInstance SourceCard { get; set; }
}

public abstract class TargetInfoFilter
{
    public abstract IEnumerable<CardGameEntity> GetTargets(
        CardGame gameState,
        TargetSourceInfo sourceInfo);
}

public class PlayerSelfTargetFilter : TargetInfoFilter
{
    public override IEnumerable<CardGameEntity> GetTargets(
        CardGame gameState,
        TargetSourceInfo sourceInfo
    )
    {
        return new List<CardGameEntity> { sourceInfo.SourcePlayer };
    }
}

public class UnitsTargetFilter : TargetInfoFilter
{
    public override IEnumerable<CardGameEntity> GetTargets(
        CardGame gameState,
        TargetSourceInfo sourceInfo
    )
    {
        return new List<CardGameEntity> { sourceInfo.SourcePlayer };
    }
}



