public class UnblockableAbility : CardAbility, IModifyCanAttackDirectly
{
    public override string RulesText => "Unblockable";
    public UnblockableAbility()
    {
        Priority = 10; //Unblockable should take priority over any IModifyCanAttackDirectly Ability.
    }
    public bool ModifyCanAttackDirectly(CardGame gameState, Lane attackingLane, Lane defendingLane)
    {
        return true;
    }
}


