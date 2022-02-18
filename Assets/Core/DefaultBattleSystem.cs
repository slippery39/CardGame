using System.Collections.Generic;
using UnityEngine;

public class DefaultBattleSystem : IBattleSystem
{
    public void ExecuteBattles(CardGame cardGame)
    {
        var lane1 = cardGame.Player1Lane;
        var lane2 = cardGame.Player2Lane;

        var lanesForAttacker = cardGame.ActivePlayer == 1 ? lane1 : lane2;
        var lanesForDefender = cardGame.ActivePlayer == 1 ? lane2 : lane1;

        //TODO - we need a concept of a player here, since things are getting a bit trickier.

        for (int i = 0; i < lanesForAttacker.Count; i++)
        {
            //Neither player has units in lane
            if (lanesForAttacker[i] == null)
            {
                continue;
            }
            //Attacking an empty lane
            if (lanesForAttacker[i] != null && lanesForDefender[i] == null)
            {
                var unit = (UnitCardData)lanesForAttacker[i].CurrentCardData;
                if (cardGame.ActivePlayer == 1)
                {
                    cardGame.Player2Health -= unit.Power;
                }
                else
                {
                    cardGame.Player1Health -= unit.Power;
                }
            }
            else
            {
                //Both lanes have units, they will attack eachother.
                var attackingUnit = (UnitCardData)lanesForAttacker[i].CurrentCardData;
                var defendingUnit = (UnitCardData)lanesForDefender[i].CurrentCardData;

                attackingUnit.Toughness -= defendingUnit.Power;
                defendingUnit.Toughness -= attackingUnit.Power;

                if (attackingUnit.Toughness <= 0)
                {
                    //should die
                    lanesForAttacker[i] = null;
                }
                if (defendingUnit.Toughness <= 0)
                {
                    lanesForDefender[i] = null;
                }
            }
            //Both players have units in lane
        }
        //Temporary hack for now. In the future we should have a seperate system which handles our turn logic.
        _ = cardGame.ActivePlayer == 1 ? cardGame.ActivePlayer = 2 : cardGame.ActivePlayer = 1;
    }
}
