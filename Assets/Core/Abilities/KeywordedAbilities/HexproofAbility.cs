public class HexproofAbility : CardAbility, IModifyCanBeTargeted
{
    public override string RulesText => "Hexproof";

    public bool ModifyCanBeTargeted(CardGame cardGame, CardInstance unitWithAbility, Player ownerOfEffect)
    {
        return cardGame.GetOwnerOfCard(unitWithAbility) == ownerOfEffect;
    }
}


