using System.Collections.Generic;

public abstract class Effect
{
    public abstract string RulesText { get; }
    public virtual TargetType TargetType { get; set; }
    public abstract void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply);
}


