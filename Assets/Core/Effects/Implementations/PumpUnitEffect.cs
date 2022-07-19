public class PumpUnitEffect : Effect
{
    public override string RulesText
    {
        get
        {
            var powerSymbol = Power >= 0 ? "+" : "-";
            var toughnessSymbol = Toughness > 0 ? "+" : "-";
            var rulesText = $"Give {powerSymbol}{Power}/{toughnessSymbol}{Toughness} to {TargetTypeHelper.TargetTypeToRulesText(TargetType)}";
            return rulesText;
        }
    }
    public int Power { get; set; }
    public int Toughness { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.TargetUnits;
}


