using System.Collections.Generic;
using System.Linq;

public class GoblinPiledriverEffect : Effect
{
    public override string RulesText => $@"Gets +2/+0 for each goblin you control";
    public override TargetType TargetType { get; set; } = TargetType.UnitSelf;

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        var goblinsInPlay = player.Lanes.Where(l => l.IsEmpty() == false).Select(l => l.UnitInLane).Where(u => u != source && u.CreatureType == "Goblin").Count();
        cardGame.UnitPumpSystem.PumpUnit(source, new PumpUnitEffect { Power = 2 * goblinsInPlay, Toughness = 0 });
    }
}


