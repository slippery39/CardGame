using System.Collections.Generic;

public interface IResolvingSystem
{
    public void Add(ResolvingEntity entity);
    public void ResolveNext();
}

public class DefaultResolvingSystem: IResolvingSystem
{
    private List<ResolvingEntity> _stack = new List<ResolvingEntity>();
    
    public void Add(ResolvingEntity entity)
    {
        _stack.Add(entity);
    }

    public void ResolveNext()
    {
        if (_stack.Count == 0)
        {
            return;
        }

        var nextIndex = _stack.Count - 1;
        var resolvingThing = _stack[nextIndex];
        _stack.RemoveAt(nextIndex);

        if (resolvingThing is ResolvingAbility)
        {
            //todo - handle the resolving of abilities.
            //todo - handle what happens if there are no legal targets
        }
        else if (resolvingThing is ResolvingCardInstance)
        {
            //todo - handle the resolving of a card instance. 
            //todo - handle what happens if there are no legal targets - i.e. place in graveyard if its a spell.
        }
    }
}

public class ResolvingEntity
{
    public List<CardGameEntity> Targets { get; set; }
}

public class ResolvingAbility : ResolvingEntity
{
    public CardAbility Ability { get; set; }
}
public class ResolvingCardInstance : ResolvingEntity
{    public CardInstance CardInstance { get; set; }
}