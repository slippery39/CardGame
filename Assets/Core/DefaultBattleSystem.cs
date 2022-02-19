using System.Collections.Generic;
using UnityEngine;

public class DefaultBattleSystem : IBattleSystem
{
    public void ExecuteBattles(CardGame cardGame)
    {
        var lanesForAttacker = cardGame.ActivePlayer.Lanes;
        var lanesForDefender = cardGame.InactivePlayer.Lanes;



        for (int i = 0; i < lanesForAttacker.Count; i++)
        {
            Debug.Log($"Battle System Attacking Player : {cardGame.ActivePlayerId} for Lane {(i + 1)}");
            Debug.Log("Checking Attacking Lane is Empty");
            Debug.Log(lanesForAttacker[i].IsEmpty());
            Debug.Log("Checking Defending Lane is Empty");
            Debug.Log(lanesForDefender[i].IsEmpty());

            //Neither player has units in lane
            if (lanesForAttacker[i].IsEmpty())
            {
                Debug.Log("Both Lanes were empty");
                continue;
            }
            //Attacking an empty lane
            else if (lanesForDefender[i].IsEmpty())
            {
                Debug.Log("Defending Lane was Empty");
                var unit = (UnitCardData)lanesForAttacker[i].UnitInLane.CurrentCardData;
                if (cardGame.ActivePlayerId == 1)
                {
                    cardGame.Player2.Health -= unit.Power;
                }
                else
                {
                    cardGame.Player1.Health -= unit.Power;
                }
            }
            else
            {
                Debug.Log("Both Lanes have Units");
                //Both lanes have units, they will attack eachother.
                var attackingUnit = (UnitCardData)lanesForAttacker[i].UnitInLane.CurrentCardData;
                var defendingUnit = (UnitCardData)lanesForDefender[i].UnitInLane.CurrentCardData;

                Debug.Log("Checking Attacking Unit In Lane");
                Debug.Log(lanesForAttacker[i].UnitInLane);
                Debug.Log("Checking Defending Unit In Lane");
                Debug.Log(lanesForDefender[i].UnitInLane);

                //UnitInLane - is not null
                //CurrentCardData - is null
                //WTF?

                Debug.Log(attackingUnit);
                Debug.Log(defendingUnit);

                attackingUnit.Toughness -= defendingUnit.Power;
                defendingUnit.Toughness -= attackingUnit.Power;

                if (attackingUnit.Toughness <= 0)
                {
                    //should die
                    Debug.Log("Attacking Unit Is Dying");
                    lanesForAttacker[i].RemoveUnitFromLane();
                }
                if (defendingUnit.Toughness <= 0)
                {
                    Debug.Log("Defending Unit Is Dying");
                    lanesForDefender[i].RemoveUnitFromLane();
                }
            }
            //Both players have units in lane
        }
        //Temporary hack for now. In the future we should have a seperate system which handles our turn logic.
        _ = cardGame.ActivePlayerId == 1 ? cardGame.ActivePlayerId = 2 : cardGame.ActivePlayerId = 1;
    }
}
