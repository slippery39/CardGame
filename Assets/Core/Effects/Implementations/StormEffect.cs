using System.Collections.Generic;

public class StormEffect : Effect
{
    public override string RulesText => $"Storm\r\n {ChildEffect.RulesText}";
    public Effect ChildEffect { get; set; }

    public override TargetInfo TargetInfo { get => ChildEffect.TargetInfo; }

    public override TargetType TargetType { get=>ChildEffect.TargetInfo!=null ? ChildEffect.TargetInfo.TargetType : ChildEffect.TargetType; }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        var amount = cardGame.SpellsCastThisTurn;
        for(var i= 0;i < amount; i++) 
        {
            //Potential issue here with the entities to apply
            ChildEffect.Apply(cardGame, player, source, entitiesToApply);
        }
    }
}

