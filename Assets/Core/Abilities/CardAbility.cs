using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class CardAbility
{
    public string Type;
    public int Priority { get; set; }

    [JsonIgnore]
    public abstract string RulesText { get; }
    public bool ThisTurnOnly { get; set; } = false;
    public List<AbilityComponent> Components { get; set; } = new List<AbilityComponent>();
    public List<Effect> Effects { get; set; } = new List<Effect> { };

    public CardAbility Clone()
    {
        CardAbility clone = (CardAbility)MemberwiseClone();
        clone.Components = Components.Select(c => c.Clone()).ToList();
        return clone;
    }

    public T GetComponent<T>()
    {
        return Components.Where(c => c is T).Cast<T>().FirstOrDefault();
    }

    //Components and effects also need to be deep cloned.
}

public abstract class AbilityComponent
{
}

public class AbilityCooldown : AbilityComponent
{

}


public interface IModifyCanBlock
{
    bool ModifyCanBlock(CardGame cardGame);
}

public interface IOnDamageDealt
{
    void OnDamageDealt(CardGame cardGame, CardInstance damagingUnit, CardInstance damagedUnit, int damage);
}
public interface IModifyCanAttackDirectly
{
    bool ModifyCanAttackDirectly(CardGame gameState, Lane attackingLane, Lane defendingLane);
}

public interface IModifyCanBeTargeted
{
    bool ModifyCanBeTargeted(CardGame cardGame, CardInstance unitWithAbility, Player ownerOfEffect);
}

public interface IModifyCanAttack
{
    public bool CanAttack(CardGame cardGame, CardInstance card);

}


public enum TriggerType
{
    SelfEntersPlay,
    SelfDies,
    SelfAttacks,
    SelfManaPlayed,
    AtTurnStart,
    AtTurnEnd,
    SomethingDies
}


public class TriggeredAbility : CardAbility
{
    public override string RulesText
    {
        get
        {
            string text = "";

            switch (TriggerType)
            {
                case TriggerType.SelfEntersPlay:
                    text = "When this enters play ";
                    break;
                case TriggerType.SelfDies:
                    text = "When this dies ";
                    break;
                case TriggerType.SelfAttacks:
                    text = "When this attacks ";
                    break;
                case TriggerType.AtTurnStart:
                    text = "At the start of your turn ";
                    break;
                case TriggerType.AtTurnEnd:
                    text += "At the end of the turn ";
                    break;
                case TriggerType.SomethingDies:

                    var cardFilterString = Filter.RulesTextString();

                    text += $"When a {(cardFilterString != "" ? cardFilterString : "anything")} dies ";
                    break;
                default:
                    text += "";
                    break;
            }

            foreach (var effect in Effects)
            {
                text += Effect.CompileRulesText(effect);
                text += ",";
            }

            //Get rid of the last ",";
            text = text.Substring(0, text.Length - 1);


            return text;
        }
    }
    public TriggerType TriggerType { get; set; }
    //Filter that causes the trigger only to apply if it meets the filtering requirements (i.e. Instead of when a creature dies, When a goblin dies, When a goblin comes into play etc).
    public CardFilter Filter { get; set; } = new CardFilter();

    public TriggeredAbility(TriggerType triggerType, Effect effect)
    {
        TriggerType = triggerType;
        Effects = new List<Effect>();
        Effects.Add(effect);
    }

    public TriggeredAbility()
    {

    }
}

public enum TargetType
{
    None,
    PlayerSelf, //Player
    Opponent, //non targetting
    AllUnits, //non targetting
    OurUnits, //non targetting
    CardsInHand,
    OtherCreaturesYouControl, //non targetting
    OpponentUnits, //non targetting
    UnitSelf, //Self Unit
    TargetPlayers, //any player
    TargetUnits, //any unit
    TargetUnitsOrPlayers, //any unit or player
    RandomOurUnits,
    RandomOpponentOrUnits,
    OpenLane,
    OpenLaneBesideUnit, //mainly for token creation, tries to place the token nearest left or right to the unit that is creating it.
    //NEW TARGET TYPES FOR UPDATED SYSTEM HERE:
    /// <summary>
    /// Targets units and/or players
    /// </summary>
    UnitsAndPlayers
}

/// <summary>
/// This enum describes which entities will be targeted by an effect basedd on the owner of the entity.
/// </summary>
public enum TargetOwnerType
{
    /// <summary>
    /// Target entities from any owner i.e. Destroy all creatures.
    /// </summary>
    Any,
    /// <summary>
    /// Only target our entities i.e. ex. Our Creatures get +1/+1 until end of turn.
    /// </summary>
    Ours,
    /// <summary>
    /// Only target their cards i.e. Deal 2 damage to target creature an opponent controls
    /// </summary>
    Theirs
}

/// <summary>
/// This enum descibes which entities will be targeted based on a list of valid choices.
/// ex. If you want to target a single creature, you would use Target.
/// If you want to target all creatures at once you would use All./// 
/// </summary>
public enum TargetMode
{
    /// <summary>
    /// IN PROGRESS - NOT SURE ABOUT THIS ONE YET, MAY NEED TO CHANGE
    /// Card has no targets at all ex. Divination - Draw 2 Cards
    /// </summary>
    None,
    /// <summary>
    /// Card selects random targets ex. Deal 2 damage to 2 random creatures
    /// </summary>
    Random,
    /// <summary>
    /// You select the targets for the effect. ex. Deal 2 damage to a target creature
    /// </summary>
    Target,
    /// <summary>
    /// All targets are selected for the effect ex. Deal 2 damage to each creature
    /// </summary>
    All
}

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
    /// </summary>
    public int NumberOfTargets => NeedsTargets ? 1 : 0;

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
        //TODO - Filter by OwnerType
        //Differentiating between TargetTypes that need targets vs TargetTypes that don't at the moment.
        //In the future this might change.
        if (NeedsTargets)
        {
            if (TargetType == TargetType.TargetPlayers)
            {
                return GetPlayers(cardGame);
            }
            else if (TargetType == TargetType.TargetUnits)
            {
                return GetUnits(cardGame);
            }
            else if (TargetType == TargetType.TargetUnitsOrPlayers || (TargetType == TargetType.UnitsAndPlayers))
            {
                return GetUnits(cardGame).Concat(GetPlayers(cardGame));
            }
        }
        else
        {
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




public static class TargetTypeHelper
{
    //TODO - Move to TargetInfo
    public static string TargetTypeToRulesText(TargetType targetType)
    {
        switch (targetType)
        {
            case TargetType.AllUnits: return "each #unitType#";
            case TargetType.OurUnits: return "each #unitType# you control";
            case TargetType.OpponentUnits: return "each #unitType# your opponent controls";
            case TargetType.TargetUnits: return "target #unitType#";
            case TargetType.TargetPlayers: return "target player";
            case TargetType.TargetUnitsOrPlayers: return "target #unitType# or player";
            case TargetType.UnitSelf: return "#this#";
            case TargetType.PlayerSelf: return "to itself";
            case TargetType.Opponent: return " an opponent";
            case TargetType.RandomOpponentOrUnits: return " a random opponent or #unitType# an opponent controls";
            default: return "";
        }
    }
}

public interface IModifyManaCost
{
    string ModifyManaCost(CardGame cardGame, CardInstance card, string originalManaCost);
}


/*
public class ModifyManaCostComponent : AbilityComponent, IModifyManaCost
{
    private Func<CardGame, CardInstance, string, string> _modManaCostFunc;

    public ModifyManaCostComponent(Func<CardGame, CardInstance, string, string> modManaCostFunc)
    {
        _modManaCostFunc = modManaCostFunc;
    }
    public string ModifyManaCost(CardGame cardGame, CardInstance card, string originalManaCost)
    {
        //Serialize TODO - This does not get set again?
        return _modManaCostFunc(cardGame, card, originalManaCost);
    }
}
*/

public interface IModifyCastZones
{
    List<ZoneType> ModifyCastZones(CardGame cardGame, CardInstance card, List<ZoneType> originalCastZones);

}

/*
public class ModifyCastZonesComponent : AbilityComponent, IModifyCastZones
{
    private Func<CardGame, CardInstance, List<ZoneType>, List<ZoneType>> _modCastZoneFunc;

    public ModifyCastZonesComponent(Func<CardGame, CardInstance, List<ZoneType>, List<ZoneType>> modCastZoneFunc)
    {
        _modCastZoneFunc = modCastZoneFunc;
    }

    public List<ZoneType> ModifyCastZones(CardGame cardGame, CardInstance card, List<ZoneType> originalCastZones)
    {
        return _modCastZoneFunc(cardGame, card, originalCastZones);
    }
}
*/

public interface IModifyAdditionalCost
{
    AdditionalCost ModifyAdditionalCost(CardGame cardGame, CardInstance sourceCard, AdditionalCost originalAdditionalCost);
}

/*
public class ModifyAdditionalCostComponent : AbilityComponent, IModifyAdditionalCost
{
    private Func<CardGame, CardInstance, AdditionalCost, AdditionalCost> _modAdditionalCostFunc;

    public ModifyAdditionalCostComponent(Func<CardGame, CardInstance, AdditionalCost, AdditionalCost> modCastZoneFunc)
    {
        _modAdditionalCostFunc = modCastZoneFunc;
    }

    public AdditionalCost ModifyAdditionalCost(CardGame cardGame, CardInstance card, AdditionalCost originalAdditionalCost)
    {
        return _modAdditionalCostFunc(cardGame, card, originalAdditionalCost);
    }
}
*/

public interface IOnSummon
{
    void OnSummoned(CardGame cardGame, CardInstance source);
}

public interface IOnDeath
{
    void OnDeath(CardGame cardGame, CardInstance source);
}


