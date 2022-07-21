using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BounceUnitEffect : Effect
{
    public override TargetType TargetType { get; set; } = TargetType.TargetUnits;
    public override string RulesText => "Return a unit to its owners hand";

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            if (!(entity is CardInstance))
            {
                continue;
            }
            var unit = entity as CardInstance;
            var owner = cardGame.GetOwnerOfCard(unit);
            cardGame.ZoneChangeSystem.MoveToZone(unit, owner.Hand);
        }
    }
}

