using System.Collections.Generic;
using UnityEngine;

public class DefaultBattleSystem : IBattleSystem
{
    public void ExecuteBattles(CardGame cardGame)
    {
        var lane1 = cardGame.Player1Lane;
        var lane2 = cardGame.Player2Lane;

        for (int i = 0; i < lane1.Count; i++)
        {
            if (lane1[i] == null && lane2[i] == null)
            {
                continue;
            }
            if (lane2[i] == null)
            {
                //Lane 1 will attack player 2 directly.
            }
            else if (lane1[i] == null)
            {
                //Lane 2 will attack player 1 directly.
            }
            else
            {
                //Both lanes have units, they will attack eachother.
                var card1 = (UnitCardData)lane1[i].CurrentCardData;
                var card2 = (UnitCardData)lane2[i].CurrentCardData;

                card1.Toughness -= card2.Power;
                card2.Toughness -= card1.Power;

                if (card1.Toughness <= 0)
                {
                    //should die
                    lane1[i] = null;
                 }
                if (card2.Toughness <= 0)
                {
                    lane2[i] = null;
                }

            }

        }
    }
}
