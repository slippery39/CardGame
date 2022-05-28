public class ActivatedAbility : CardAbility
{
    public override string RulesText => $@"{ManaCost},{AdditionalCost}:{AbilityEffect.RulesText}";
    public string ManaCost { get; set; }
    public AdditionalCost AdditionalCost { get; set; } = null;
    public Effect AbilityEffect { get; set; }

    public bool HasAdditionalCost()
    {
        return AdditionalCost != null;
    }
}
