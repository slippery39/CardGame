using System.Collections.Generic;

public abstract class Effect
{
    public abstract string RulesText { get; }
    public virtual TargetType TargetType { get; set; }
    public CardFilter Filter { get; set; }
    public abstract void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply);


    public static string CompileRulesText(Effect effect)
    {
        var effectText = effect.RulesText;

        effectText = effectText.Replace("#effectTargetType#", TargetTypeHelper.TargetTypeToRulesText(effect.TargetType));

        var unitType = "unit";
        if (effect.Filter != null && effect.Filter.RulesTextString() != "")
        {
            unitType = effect.Filter.RulesTextString();
        }

        effectText = effectText.Replace("#unitType#", unitType);

        return effectText;
    }
}


