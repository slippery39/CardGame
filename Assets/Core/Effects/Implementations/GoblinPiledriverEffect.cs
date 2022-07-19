public class GoblinPiledriverEffect : Effect
{
    public override string RulesText => $@"Gets +2/+0 for each goblin you control";
    public override TargetType TargetType { get; set; } = TargetType.UnitSelf;
}


