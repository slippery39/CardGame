
//Damage Abiltiies are handled by the DamageSystem themselves?
using System.Collections.Generic;

public class DamageEffect : Effect
{
    public override string RulesText => $"Deal {Amount} Damage to {TargetTypeHelper.TargetTypeToRulesText(TargetType)}";
    public int Amount { get; set; }

    public DamageEffect()
    {
        TargetInfo = TargetInfoBuilder.TargetOpponentOrTheirUnits().Build();
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            cardGame.DamageSystem.DealAbilityDamage(this, source, entity);
        }
    }
}




