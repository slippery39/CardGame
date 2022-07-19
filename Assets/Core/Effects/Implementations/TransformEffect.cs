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
            card.SetCardData(TransformData.Clone());
        }
    }
}

//Basically just the above effect but with the delver condition. Would be better if we could have conditions be a part of effects or triggers or something.
public class DelverTransformEffect : Effect
{
    public override string RulesText => $"if the top card of your deck is a spell transform into {TransformData.Name}";
    public UnitCardData TransformData { get; set; }

    public override TargetType TargetType { get; set; } = TargetType.None;

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {

        if (!player.Deck.GetTopCard().IsOfType<SpellCardData>())
        {
            return;
        }

        foreach (var entity in entitiesToApply)
        {
            if (!(entity is CardInstance))
            {
                throw new Exception("Error : only units can be effected with the sacrifice self effect");
            }
            var card = (CardInstance)entity;
            cardGame.Log($"Top card of deck was a spell:{player.Deck.GetTopCard().Name} ! Transforming into Insectile Abberation!");
            card.SetCardData(TransformData.Clone());
        }
    }
}


