using System.Collections.Generic;
using System.Linq;

public interface IResolvingSystem
{
    public void Add(ResolvingEntity entity);
    public void ResolveNext(CardGame cardGame);
}

public class DefaultResolvingSystem: IResolvingSystem
{
    private List<ResolvingEntity> _stack = new List<ResolvingEntity>();
    
    public void Add(ResolvingEntity entity)
    {
        _stack.Add(entity);
    }

    public void ResolveNext(CardGame cardGame)
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
            var resolvingCardInstance = (ResolvingCardInstance)resolvingThing;
            //Handle the resolving of a unit
            if (resolvingCardInstance.CardInstance.CurrentCardData is UnitCardData)
            {
                var player = cardGame.GetOwnerOfCard(resolvingCardInstance.CardInstance);
                cardGame.UnitSummoningSystem.SummonUnit(cardGame, player, resolvingCardInstance.CardInstance, resolvingThing.Targets.First().EntityId);

            }
            //Handle the resolving of a spell
            else if (resolvingCardInstance.CardInstance is SpellCardData)
            {
                var player = cardGame.GetOwnerOfCard(resolvingCardInstance.CardInstance);
                cardGame.SpellCastingSystem.CastSpell(cardGame, player, resolvingCardInstance.CardInstance, resolvingCardInstance.Targets);
            }
                /*
                 * if (!_targetSystem.SpellNeedsTargets(this, player, cardFromHand) &&  ManaSystem.CanPlayCard(this, player, cardFromHand))
            {
                _spellCastingSystem.CastSpell(this, player, cardFromHand);
                _stateBasedEffectSystem.CheckStateBasedEffects(this);
            }
            else if (ManaSystem.CanPlayCard(this, player, cardFromHand))
            {
                var validTargets = _targetSystem.GetValidTargets(this, player, cardFromHand);
                var target = validTargets.Where(entity => entity.EntityId == targetId).FirstOrDefault();
                var validTargetInts = validTargets.Select(x => x.EntityId).ToList();

                if (validTargetInts.Contains(targetId))
                {

                    _spellCastingSystem.CastSpell(this, player, cardFromHand, target);
                    _stateBasedEffectSystem.CheckStateBasedEffects(this);

                }
            }
                 * 
                 */

            }
            //todo - handle the resolving of a card instance. 
            //todo - handle what happens if there are no legal targets - i.e. place in graveyard if its a spell.
        
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