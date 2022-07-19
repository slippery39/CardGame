
//Damage Abiltiies are handled by the DamageSystem themselves?
public class DamageEffect : Effect
{
    public override string RulesText => $"Deal {Amount} Damage to {TargetTypeHelper.TargetTypeToRulesText(TargetType)}";
    public int Amount { get; set; }

    public override TargetType TargetType { get; set; } = TargetType.TargetUnitsOrPlayers;
}


