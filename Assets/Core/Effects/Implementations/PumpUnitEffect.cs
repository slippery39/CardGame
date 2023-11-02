using System;
using System.Collections.Generic;

public class PumpUnitEffect : Effect
{
    public override string RulesText
    {
        get
        {
            //Example rules texts:
            //Creatures you control get +1/+1 until end of turn.
            //Atog gets +2/+2 until end of turn.
            //Shivan Dragon gets +1/+0 until end of turn.
            //Target creature gets +3/+3 until end of turn.

            var powerSymbol = Power >= 0 ? "+" : "-";
            var toughnessSymbol = Toughness >= 0 ? "+" : "-";
            var cardTargetText = TargetTypeHelper.TargetTypeToRulesText(TargetType);
            var rulesText = $"{cardTargetText} gets {powerSymbol}{Power}/{toughnessSymbol}{Toughness} until end of turn";

            return rulesText;
        }
    }
    public int Power { get; set; }
    public int Toughness { get; set; }

    public PumpUnitEffect()
    {
        TargetInfo = TargetInfo.TargetOwnUnit();
    }

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


