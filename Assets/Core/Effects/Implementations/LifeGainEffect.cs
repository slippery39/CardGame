using System.Collections.Generic;

public class LifeGainEffect : Effect
{
    public override string RulesText => $"Gain {Amount} Life";
    public int Amount { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.PlayerSelf;

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            cardGame.HealingSystem.HealPlayer((Player)entity, this.Amount);
        }
    }
}


