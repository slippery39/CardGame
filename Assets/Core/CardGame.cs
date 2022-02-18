using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class CardGame
{
    //We will probably create our own Lane class eventually, but for now plain old lists are good enough.
    private List<CardInstance> _player1Lanes;
    private List<CardInstance> _player2Lanes;
    private int _player1Health = 100;
    private int _player2Health = 100;

    private IBattleSystem battleSystem;

    #region Public Properties
    public List<CardInstance> Player1Lane { get => _player1Lanes; set => _player1Lanes = value; }
    public List<CardInstance> Player2Lane { get => _player2Lanes; set => _player2Lanes = value; }
    internal IBattleSystem BattleSystem { get => battleSystem; set => battleSystem = value; }
    public int Player1Health { get => _player1Health; set => _player1Health = value; }
    public int Player2Health { get => _player2Health; set => _player2Health = value; }
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
