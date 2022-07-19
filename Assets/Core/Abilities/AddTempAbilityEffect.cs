public class AddTempAbilityEffect : Effect
{
    public override string RulesText => $@"Give {TempAbility.RulesText} to {TargetTypeHelper.TargetTypeToRulesText(TargetType)} until end of turn";
    public CardAbility TempAbility { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.TargetUnits;
    public AddTempAbilityEffect(CardAbility tempAbility)
    {
        TempAbility = tempAbility;
        TempAbility.ThisTurnOnly = true;
    }
}


