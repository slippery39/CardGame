using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CardGame
{
    //We will probably create our own Lane class eventually, but for now plain old lists are good enough.
    List<CardInstance> _player1Lane;
    List<CardInstance> _player2Lane;

    #region Public Properties
    public List<CardInstance> Player1Lane { get => _player1Lane; set => _player1Lane = value; }
    public List<CardInstance> Player2Lane { get => _player2Lane; set => _player2Lane = value; }
    #endregion

    public CardGame()
    {
        _player1Lane = new List<CardInstance>();
        _player2Lane = new List<CardInstance>();

        //Create Random Cards in Each Lane 
        AddRandomUnitsToLane(_player1Lane);
        AddRandomUnitsToLane(_player2Lane);       
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
