using System.Linq;
using UnityEngine;

public abstract class CardAbility
{
    public string Type;
    public int Priority { get; set; }
}

public interface IModifyCanBlock
{
    bool ModifyCanBlock(CardGame gameState);
}

public class CantBlockAbility : CardAbility, IModifyCanBlock
{
    public CantBlockAbility()
    {
        Type = "Cant Block";
    }

    public bool ModifyCanBlock(CardGame gameState)
    {
        //This creature can't ever block;
        return false;
    }
}

public interface IOnDamageDealt
{
    void OnDamageDealt(CardGame gameState, CardInstance damagingUnit, CardInstance damagedUnit, int damage);
}

public class LifelinkAbility : CardAbility, IOnDamageDealt
{
    public LifelinkAbility()
    {
        Type = "Lifelink";
    }

    public void OnDamageDealt(CardGame gameState, CardInstance damagingUnit, CardInstance damagedUnit, int damage)
    {
        //Need a way to find out who owns which unit
        Player playerToGainLife = gameState.GetOwnerOfUnit(damagingUnit);
        gameState.HealingSystem.HealPlayer(gameState, playerToGainLife, damage);
    }
}

public class DeathtouchAbility : CardAbility, IOnDamageDealt
{
    public DeathtouchAbility()
    {
        Type = "Deathtouch";
    }

    public void OnDamageDealt(CardGame gameState, CardInstance damagingUnit, CardInstance damagedUnit, int damage)
    {
        //Filter out damage events that are not dealing to units, or else this will crash.
        if (damagedUnit == null)
        {
            return;
        }
        //Need a way to find out who owns which unit
        //hack - setting toughness to 0.
        //later on we will probably have some sort of DestroyingSystem and we would call that instead.
        ((UnitCardData)damagedUnit.CurrentCardData).Toughness = 0;
    }
}
public interface IModifyCanAttackDirectly
{
    bool ModifyCanAttackDirectly(CardGame gameState, Lane attackingLane, Lane defendingLane);
}
public class FlyingAbility : CardAbility, IModifyCanAttackDirectly
{
    public FlyingAbility()
    {
        Type = "Flying";
    }

    public bool ModifyCanAttackDirectly(CardGame gameState, Lane attackingLane, Lane defendingLane)
    {
        var defendingUnit = (UnitCardData)defendingLane.UnitInLane.CurrentCardData;
        //If the other unit does not have flying, then this creature can attack directly.
        if (defendingUnit.Abilities.Where(ab => ab is FlyingAbility).Count() > 0)
        {
            Debug.Log("can't attack directly");
            return false;
        }
        else
        {
            Debug.Log("cannot attack directly");
            return true;
        }
    }
}

public class UnblockableAbility : CardAbility, IModifyCanAttackDirectly
{
    public UnblockableAbility()
    {
        Priority = 10; //Unblockable should take priority over any IModifyCanAttackDirectly Ability.
    }
    public bool ModifyCanAttackDirectly(CardGame gameState, Lane attackingLane, Lane defendingLane)
    {
        return true;
    }
}