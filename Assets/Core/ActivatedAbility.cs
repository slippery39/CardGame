public class ActivatedAbility : CardAbility
{
    public override string RulesText => $@"{ManaCost}:{AbilityEffect.RulesText}";
    public string ManaCost { get; set; }
    public Effect AbilityEffect { get; set; }
}

