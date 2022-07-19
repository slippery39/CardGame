public class CantBlockAbility : CardAbility, IModifyCanBlock
{
    public override string RulesText => "Can't Block";
    public CantBlockAbility()
    {
        Type = "Cant Block";
    }

    public bool ModifyCanBlock(CardGame cardGame)
    {
        //This creature can't ever block;
        return false;
    }
}


