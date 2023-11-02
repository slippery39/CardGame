public static class TargetTypeHelper
{
    //TODO - Move to TargetInfo
    //Commenting TargetTypes out that we have already removed.
    public static string TargetTypeToRulesText(TargetType targetType)
    {
        switch (targetType)
        {
            //case TargetType.AllUnits: return "each #unitType#";
            //case TargetType.OurUnits: return "each #unitType# you control";
            //case TargetType.OpponentUnits: return "each #unitType# your opponent controls";
            case TargetType.TargetUnits: return "target #unitType#";
            case TargetType.TargetPlayers: return "target player";
            case TargetType.TargetUnitsOrPlayers: return "target #unitType# or player";
            case TargetType.UnitSelf: return "#this#";
            case TargetType.PlayerSelf: return "to itself";
            case TargetType.Opponent: return " an opponent";
            case TargetType.RandomOpponentOrUnits: return " a random opponent or #unitType# an opponent controls";
            default: return "";
        }
    }
}


