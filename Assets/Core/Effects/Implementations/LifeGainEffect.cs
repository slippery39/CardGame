public class LifeGainEffect : Effect
{
    public override string RulesText => $"Gain {Amount} Life";
    public int Amount { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.Self;
}


