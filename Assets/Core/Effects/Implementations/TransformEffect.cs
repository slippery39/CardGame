using System;
using System.Collections.Generic;

public class TransformEffect : Effect
{
    public override string RulesText => $"Transform into {TransformData.Name}";
    public UnitCardData TransformData { get; set; }

    public override TargetType TargetType { get; set; } = TargetType.None;

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            if (!(entity is CardInstance))
            {
                throw new Exception("Error : only units can be effected with the sacrifice self effect");
            }
            var card = (CardInstance)entity;

            //TODO - NOT SURE IF THIS WORKS, since we may not be grabbing the current card data anymore.
            card.CurrentCardData = TransformData.Clone();
        }
    }
}


