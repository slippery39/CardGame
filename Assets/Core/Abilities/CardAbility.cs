using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CardAbility:  IGameCloneable<CardAbility>
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
        clone.Components = Components.Clone();
        return clone;
    }

    public T GetComponent<T>()
    {
        return Components.Where(c => c is T).Cast<T>().FirstOrDefault();
    }
}

public abstract class AbilityComponent : IGameCloneable<AbilityComponent>
{
    public AbilityComponent Clone()
    {
        return (AbilityComponent)MemberwiseClone();
    }
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
                default:
                    text += "";
                    break;
            }

            foreach (var effect in Effects)
            {
                text += effect.RulesText;
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
    Opponent,
    AllUnits,
    OurUnits,
    CardsInHand,
    OtherCreaturesYouControl,
    OpponentUnits,
    UnitSelf, //Self Unit
    TargetPlayers,
    TargetUnits,
    TargetUnitsOrPlayers,
    RandomOurUnits,
    RandomOpponentOrUnits,
    OpenLane,
    OpenLaneBesideUnit, //mainly for token creation, tries to place the token nearest left or right to the unit that is creating it.
}

public static class TargetTypeHelper
{
    public static string TargetTypeToRulesText(TargetType targetType)
    {
        switch (targetType)
        {
            case TargetType.AllUnits: return "each unit";
            case TargetType.OurUnits: return "each unit you control";
            case TargetType.OpponentUnits: return "each unit your opponent controls";
            case TargetType.TargetUnits: return "target unit";
            case TargetType.TargetPlayers: return "target player";
            case TargetType.TargetUnitsOrPlayers: return "target unit or player";
            case TargetType.UnitSelf: return "#this#";
            case TargetType.PlayerSelf: return "to itself";
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


