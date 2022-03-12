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
}

public class DefaultTargetSystem : ITargetSystem
{
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