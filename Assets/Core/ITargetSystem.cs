using System.Collections.Generic;
using System.Linq;

public interface ITargetSystem
{
    List<CardGameEntity> GetValidTargets(CardGame cardGame, Player player, CardInstance card);

    bool SpellNeedsTargets(CardGame cardGame, Player player, CardInstance card);
    bool EffectNeedsTargets(Effect effect);
}

public class DefaultTargetSystem : ITargetSystem
{

    private List<TargetType> typesThatDontNeedTargets = new List<TargetType> { TargetType.Self, TargetType.Opponent, TargetType.None };

    //Need to differentiate between spells with targets and spells that don't have targets.
    //TODO - Make the non targetted spells work.
    public bool SpellNeedsTargets(CardGame cardGame, Player player, CardInstance card)
    {
        var spellCard = (SpellCardData)card.CurrentCardData;
        var spellTargetTypes = spellCard.Effects.Select(effect => effect.TargetType);

        return (spellTargetTypes.Where(tt => typesThatDontNeedTargets.Contains(tt) == false).Count() > 0);
    }
    public List<CardGameEntity> GetValidTargets(CardGame cardGame, Player player, CardInstance card)
    {
        if (card.CurrentCardData is UnitCardData)
        {
            var emptyLanes = player.Lanes.Where(lane => lane.IsEmpty()).ToList();
            var emptyLanesCast = emptyLanes.Cast<CardGameEntity>();

            return emptyLanesCast.ToList();
        }
        else if (card.CurrentCardData is SpellCardData)
        {
            var spellCard = (SpellCardData)card.CurrentCardData;
            var effects = spellCard.Effects.Select(e => e.TargetType);

            if (!SpellNeedsTargets(cardGame, player, card))
            {
                return new List<CardGameEntity>();
            }
            else
            {
                if (effects.Contains(TargetType.TargetPlayers))
                {
                    return cardGame.Players.Cast<CardGameEntity>().ToList();
                }
                else if (effects.Contains(TargetType.TargetUnits))
                {
                    return cardGame.GetEntities<Lane>().Where(lane => lane.IsEmpty() == false).Select(lane=>lane.UnitInLane).Cast<CardGameEntity>().ToList();
                }
                else if (effects.Contains(TargetType.TargetUnitsOrPlayers))
                {
                    var validTargets = cardGame.GetEntities<Lane>().Where(lane => lane.IsEmpty() == false).Select(lane => lane.UnitInLane).Cast<CardGameEntity>().ToList();
                    var players = cardGame.Players.Cast<CardGameEntity>().ToList();

                    validTargets.AddRange(players);

                    return validTargets;

                }
            }          
        }
        return new List<CardGameEntity>();
    }
    public bool EffectNeedsTargets(Effect effect)
    {
        return typesThatDontNeedTargets.Contains(effect.TargetType) == false;
    }
}