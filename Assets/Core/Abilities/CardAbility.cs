using Newtonsoft.Json;
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


