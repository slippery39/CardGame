using System.Collections.Generic;

public class ResolvingActionInfo
{
    public Player Owner { get; set; }
    //The zone that the entity was in before it was added to the stack.
    public IZone SourceZone { get; set; }
    public CardInstance Source { get; set; }
    public List<CardGameEntity> Targets { get; set; }
}

public class ResolvingAbilityActionInfo : ResolvingActionInfo
{
    public CardAbility Ability { get; set; }
}
public class ResolvingCardInstanceActionInfo : ResolvingActionInfo
{
    public CardInstance CardInstance { get; set; }
}
