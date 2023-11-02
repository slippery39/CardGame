public class TargetInfoBuilder
{
    private TargetInfo _targetInfo;

    public TargetInfoBuilder()
    {
        _targetInfo = new TargetInfo();
    }

    public static TargetInfoBuilder EachUnitYouControl()
    {
        return new TargetInfoBuilder()._EachUnitYouControl();
    }

    public static TargetInfoBuilder EachOpponentUnit()
    {
        return new TargetInfoBuilder()._EachOpponentUnit();
    }

    public static TargetInfoBuilder TargetOpponentUnit()
    {
        return new TargetInfoBuilder()._TargetOpponentUnit();
    }

    public static TargetInfoBuilder TargetOwnUnit()
    {
        var builder = new TargetInfoBuilder();
        builder._targetInfo = new TargetInfo
        {
            TargetType = TargetType.Units,
            TargetMode = TargetMode.Target,
            OwnerType = TargetOwnerType.Ours,
        };
        return builder;
    }

    public static TargetInfoBuilder TargetAnyUnit()
    {
        var builder = new TargetInfoBuilder();
        builder._targetInfo = new TargetInfo
        {
            TargetType = TargetType.Units,
            TargetMode = TargetMode.Target,
            OwnerType = TargetOwnerType.Any
        };
        return builder;
    }

    public static TargetInfoBuilder TargetOpponentOrTheirUnits()
    {
        var builder = new TargetInfoBuilder();
        builder._targetInfo = new TargetInfo
        {
            TargetType = TargetType.UnitsAndPlayers,
            TargetMode = TargetMode.Target,
            OwnerType = TargetOwnerType.Theirs
        };
        return builder;
    }

    private TargetInfoBuilder _EachUnitYouControl()
    {
        _targetInfo.TargetType = TargetType.Units;
        _targetInfo.TargetMode = TargetMode.All;
        _targetInfo.OwnerType = TargetOwnerType.Ours;
        return this;
    }

    private TargetInfoBuilder _EachOpponentUnit()
    {
        _targetInfo.TargetType = TargetType.Units;
        _targetInfo.TargetMode = TargetMode.All;
        _targetInfo.OwnerType = TargetOwnerType.Theirs;
        return this;
    }

    private TargetInfoBuilder _TargetOpponentUnit()
    {
        _targetInfo.TargetType = TargetType.Units;
        _targetInfo.TargetMode = TargetMode.Target;
        _targetInfo.OwnerType = TargetOwnerType.Theirs;
        return this;
    }

    public TargetInfoBuilder WithUnitType(string unitType)
    {
        if (_targetInfo.TargetFilter == null)
        {
            _targetInfo.TargetFilter = new CardFilter();
        }
        _targetInfo.TargetFilter.CreatureType = unitType;
        return this;
    }

    public TargetInfoBuilder WithTargetFilter(CardFilter filter)
    {
        _targetInfo.TargetFilter = filter;
        return this;
    }

    public TargetInfo Build()
    {
        return _targetInfo;
    }
}


