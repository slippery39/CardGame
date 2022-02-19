using System.Linq;
using UnityEngine;

public abstract class CardAbility
{
    public string Type;
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

public class LifelinkAbility : CardAbility
{
    public LifelinkAbility()
    {
        Type = "Lifelink";
    }

    //TODO - in order for this to work, we need our players and an id system setup.
    //public void OnDamageDealt(CardGame gameState,DamageDealtInfo damageDealtInfo)
    //{

    //}
}

public class DeathtouchAbility : CardAbility
{
    public DeathtouchAbility()
    {
        Type = "Deathtouch";
    }

    //TODO - in order for this to work, we need a Unit destroying system.
    //public void OnDamageDealt(CardGame gameState,DamageDealtInfo damageDealtInfo)
    //{

    //}
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
            Debug.Log("can attack directly");
            return false;
        }
        Debug.Log("cannot attack directly");
        return true;
    }
}