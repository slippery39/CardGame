using System.Linq;
using UnityEngine;

public abstract class CardAbility
{
    public string Type;
    public int Priority { get; set; }
    public string RulesText { get; set; }
}

public interface IModifyCanBlock
{
    bool ModifyCanBlock(CardGame cardGame);
}

public class CantBlockAbility : CardAbility, IModifyCanBlock
{
    public CantBlockAbility()
    {
        Type = "Cant Block";
        RulesText = "Can't Block";
    }

    public bool ModifyCanBlock(CardGame cardGame)
    {
        //This creature can't ever block;
        return false;
    }
}

public interface IOnDamageDealt
{
    void OnDamageDealt(CardGame cardGame, CardInstance damagingUnit, CardInstance damagedUnit, int damage);
}

public class LifelinkAbility : CardAbility, IOnDamageDealt
{
    public LifelinkAbility()
    {
        Type = "Lifelink";
        RulesText = "Lifelink";
    }

    public void OnDamageDealt(CardGame cardGame, CardInstance damagingUnit, CardInstance damagedUnit, int damage)
    {
        //Need a way to find out who owns which unit
        Player playerToGainLife = cardGame.GetOwnerOfUnit(damagingUnit);
        cardGame.Log($"{playerToGainLife} gained {damage} life from {damagingUnit.Name}'s Lifelink!");
        cardGame.HealingSystem.HealPlayer(cardGame, playerToGainLife, damage);
    }
}

public class DeathtouchAbility : CardAbility, IOnDamageDealt
{
    public DeathtouchAbility()
    {
        Type = "Deathtouch";
        RulesText = "Deathtouch";
    }

    public void OnDamageDealt(CardGame cardGame, CardInstance damagingUnit, CardInstance damagedUnit, int damage)
    {
        //Filter out damage events that are not dealing to units, or else this will crash.
        if (damagedUnit == null)
        {
            return;
        }
        //Need a way to find out who owns which unit
        //hack - setting toughness to 0.
        //later on we will probably have some sort of DestroyingSystem and we would call that instead.
        cardGame.Log($"{damagedUnit.Name} died from {damagingUnit.Name}'s deathtouch!");
        damagedUnit.Toughness = 0;
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
        RulesText = "Flying";
    }

    public bool ModifyCanAttackDirectly(CardGame gameState, Lane attackingLane, Lane defendingLane)
    {
        var defendingUnit = defendingLane.UnitInLane;
        //If the other unit does not have flying, then this creature can attack directly.
        if (defendingUnit.Abilities.Where(ab => ab is FlyingAbility).Count() > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

public class UnblockableAbility : CardAbility, IModifyCanAttackDirectly
{
    public UnblockableAbility()
    {
        RulesText = "Unblockable";
        Priority = 10; //Unblockable should take priority over any IModifyCanAttackDirectly Ability.
    }
    public bool ModifyCanAttackDirectly(CardGame gameState, Lane attackingLane, Lane defendingLane)
    {
        return true;
    }
}