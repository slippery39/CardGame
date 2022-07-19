using System;
using System.Collections.Generic;

public class PumpUnitEffect : Effect
{
    public override string RulesText
    {
        get
        {
            var powerSymbol = Power >= 0 ? "+" : "-";
            var toughnessSymbol = Toughness > 0 ? "+" : "-";
            var rulesText = $"Give {powerSymbol}{Power}/{toughnessSymbol}{Toughness} to {TargetTypeHelper.TargetTypeToRulesText(TargetType)}";
            return rulesText;
        }
    }
    public int Power { get; set; }
    public int Toughness { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.TargetUnits;

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            if (!(entity is CardInstance))
            {
                throw new Exception("Error: only card instances can be pumped");
            }

            cardGame.UnitPumpSystem.PumpUnit((CardInstance)entity, this);
        }
    }
}


