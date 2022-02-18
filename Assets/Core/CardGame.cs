using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CardGame
{
    //We will probably create our own Lane class eventually, but for now plain old lists are good enough.

    private List<CardInstance> _player1Lanes;
    private List<CardInstance> _player2Lanes;

    private IBattleSystem battleSystem;

    #region Public Properties
    public List<CardInstance> Player1Lane { get => _player1Lanes; set => _player1Lanes = value; }
    public List<CardInstance> Player2Lane { get => _player2Lanes; set => _player2Lanes = value; }
    internal IBattleSystem BattleSystem { get => battleSystem; set => battleSystem = value; }
    #endregion

    public CardGame()
    {
        _player1Lanes = new List<CardInstance>();
        _player2Lanes = new List<CardInstance>();

        //Create Random Cards in Each Lane 
        AddRandomUnitsToLane(_player1Lanes);
        AddRandomUnitsToLane(_player2Lanes);

        battleSystem = new DefaultBattleSystem();
    }

    private void AddRandomUnitsToLane(List<CardInstance> lane)
    {
        CardDatabase db = new CardDatabase();
        var unitsOnly = db.GetAll().Where(c => c.GetType() == typeof(UnitCardData)).ToList();

        var rng = new Random();

        for (int i = 0; i < 5; i++)
        {
            var randomIndex = rng.Next(0, unitsOnly.Count());
            var instancedCard = new CardInstance(unitsOnly[randomIndex]);
            lane.Add(instancedCard);
        }
    }
}

internal interface IBattleSystem
{
    void ExecuteBattles(List<CardInstance> lane1, List<CardInstance> lane2);
}

public class DefaultBattleSystem : IBattleSystem
{
    public void ExecuteBattles(List<CardInstance> lane1, List<CardInstance> lane2)
    {
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
            }

        }
    }
}
