using System;
using System.Collections.Generic;

public class DestroyEffect : Effect
{
    public override string RulesText => $"Destroy {TargetTypeHelper.TargetTypeToRulesText(TargetType)}";

    public DestroyEffect()
    {
        TargetInfo = TargetInfoBuilder.TargetOpponentUnit().Build();
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            if (!(entity is CardInstance))
            {
                throw new Exception("Error : only units or items can be effected with the sacrifice self effect");
            }

            var card = (CardInstance)entity;
            cardGame.DestroySystem.DestroyUnit(source, card);
        }
    }
}


