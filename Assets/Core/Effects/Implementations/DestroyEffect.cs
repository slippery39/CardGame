public class DestroyEffect : Effect
{
    public override string RulesText => $"Destroy {TargetTypeHelper.TargetTypeToRulesText(TargetType)}";
    public override TargetType TargetType { get; set; } = TargetType.TargetUnits;
}


