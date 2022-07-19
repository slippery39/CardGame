public class AddManaEffect : Effect
{
    public override string RulesText => $"Gain {ManaToAdd} Mana";
    public string ManaToAdd { get; set; } = "0";
    public override TargetType TargetType { get; set; } = TargetType.Self;
}


