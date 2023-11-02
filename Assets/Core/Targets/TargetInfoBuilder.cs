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


