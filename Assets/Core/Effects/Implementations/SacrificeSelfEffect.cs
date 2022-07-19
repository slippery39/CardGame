public class SacrificeSelfEffect : Effect
{
    public override string RulesText => "Sacrifice this unit";
    public override TargetType TargetType { get; set; } = TargetType.UnitSelf; //Should never need to change.
}


