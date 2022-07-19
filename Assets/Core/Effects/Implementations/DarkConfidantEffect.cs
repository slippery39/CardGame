public class DarkConfidantEffect : Effect
{
    public override string RulesText => $"Draw a card and lose life equal to its mana cost";
    public override TargetType TargetType { get; set; } = TargetType.Self;
}


