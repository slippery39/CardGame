using System.Linq;


//Putting this in this file, as it directly interacts with the FlyingAbility.
public class ReachAbility : CardAbility
{
    public override string RulesText => "Reach";
}

public class FlyingAbility : CardAbility, IModifyCanAttackDirectly
{
    public override string RulesText => "Flying";
    public FlyingAbility()
    {
        Type = "Flying";
    }

    public bool ModifyCanAttackDirectly(CardGame gameState, Lane attackingLane, Lane defendingLane)
    {
        var defendingUnit = defendingLane.UnitInLane;
        //If the other unit does not have flying, then this creature can attack directly.
        if (defendingUnit.Abilities.Where(ab => ab is FlyingAbility || ab is ReachAbility).Count() > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}


