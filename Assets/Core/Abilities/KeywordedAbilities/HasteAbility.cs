public class HasteAbility : CardAbility, IModifyCanAttack
{
    public override string RulesText => "Haste";

    public HasteAbility()
    {
        //should always apply before any other effects, if something external causes the unit to not be able to attack then it should take preference.
        Priority = -1;
    }

    public bool CanAttack(CardGame cardGame, CardInstance card)
    {
        return true;
    }
    //The ability should let the unit attack the same turn it comes into play, but it should not override any "Can't Attack" effects.
}


