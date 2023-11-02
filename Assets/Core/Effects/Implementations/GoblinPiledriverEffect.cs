using System.Collections.Generic;
using System.Linq;

public class GoblinPiledriverEffect : Effect
{
    public override string RulesText => $@"Gets +2/+0 for each goblin you control";

    public GoblinPiledriverEffect()
    {
        TargetInfo = TargetInfo.Source();
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        var goblinsInPlay = player.Lanes.Where(l => !l.IsEmpty()).Select(l => l.UnitInLane).Count(u => u != source && u.CreatureType == "Goblin");
        cardGame.UnitPumpSystem.PumpUnit(source, new PumpUnitEffect { Power = 2 * goblinsInPlay, Toughness = 0 });
    }
}


