public class ShroudAbility : CardAbility, IModifyCanBeTargeted
{
    public override string RulesText => "Shroud";

    public bool ModifyCanBeTargeted(CardGame cardGame, CardInstance unitWithAbility, Player ownerOfEffect)
    {
        return false;
    }
}


