using System.Collections.Generic;
using System.Linq;

public interface ITargetSystem
{
    /// <summary>
    /// Gets a list of valid targets for a card player from hand
    /// </summary>
    /// <param name="player"></param>
    /// <param name="card"></param>
    /// <returns></returns>
    List<CardGameEntity> GetValidTargetsForCardFromHand(CardGame cardGame,Player player, CardInstance card);

    bool SpellNeedsTargets(CardGame cardGame, Player player, CardInstance card);
}

public class DefaultTargetSystem : ITargetSystem
{

    //Need to differentiate between spells with targets and spells that don't have targets.
    //TODO - Make the non targetted spells work.
    public bool SpellNeedsTargets(CardGame cardGame, Player player, CardInstance card)
    {
        var spellCard = (SpellCardData)card.CurrentCardData;
        var targetTypes = new List<TargetType> { TargetType.Any, TargetType.Units };

        var spellTargetTypes = spellCard.Effects.Select(effect => effect.TargetType);

        return (spellTargetTypes.Where(tt => targetTypes.Contains(tt)).Count() > 0);
    }
    public List<CardGameEntity> GetValidTargetsForCardFromHand(CardGame cardGame, Player player, CardInstance card)
    {
        if (card.CurrentCardData is UnitCardData)
        {

            var emptyLanes = player.Lanes.Where(lane=>lane.IsEmpty()).ToList();
            var emptyLanesCast = emptyLanes.Cast<CardGameEntity>();

            return emptyLanesCast.ToList();
        }
        return null;        
    }


}