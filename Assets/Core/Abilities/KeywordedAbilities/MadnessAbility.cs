public class MadnessAbility : CardAbility
{
    public string ManaCost { get; set; }
    public override string RulesText => $"Madness : {ManaCost}";
}


