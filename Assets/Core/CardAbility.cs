using System.Linq;

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

public class FlyingAbility : CardAbility
{
    public FlyingAbility()
    {
        Type = "Flying";
    }

    public bool ModifyCanAttackDirectly(CardGame gameState, UnitCardData otherUnit)
    {
        //If the other unit does not have flying, then this creature can attack directly.
        if (otherUnit.Abilities.Where(ab => ab.GetType() == typeof(FlyingAbility)).Count() > 0)
        {
            return false;
        }
        return true;
    }
}