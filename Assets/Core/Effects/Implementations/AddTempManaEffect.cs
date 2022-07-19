public class AddTempManaEffect : Effect
{
    public override string RulesText => $"Add {ManaToAdd} until end of turn";
    public string ManaToAdd { get; set; } = "0";
    public override TargetType TargetType { get; set; } = TargetType.Self;
}


