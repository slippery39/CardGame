using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class CardGame
{

    private List<Player> _players;
    private int _activePlayerId = 1;
    private IBattleSystem _battleSystem;
    private int _numberOfLanes = 5;
    private int _startingPlayerHealth = 100;

    #region Public Properties
    public Player Player1 { get => _players.Where(p => p.PlayerId == 1).FirstOrDefault(); }
    public Player Player2 { get => _players.Where(p => p.PlayerId == 2).FirstOrDefault(); }
    internal IBattleSystem BattleSystem { get => _battleSystem; set => _battleSystem = value; }
    public int ActivePlayerId { get => _activePlayerId; set => _activePlayerId = value; }
    public Player ActivePlayer { get => _players.Where(p => p.PlayerId == ActivePlayerId).FirstOrDefault(); }
    public Player InactivePlayer { get => _players.Where(p => p.PlayerId != ActivePlayerId).FirstOrDefault(); }
    #endregion

    public CardGame()
    {
        _players = new List<Player>();

        _players.Add(new Player(_numberOfLanes)
        {
            PlayerId = 1,
            Health = 100
        });
        _players.Add(new Player(_numberOfLanes)
        {
            PlayerId = 2,
            Health = 100
        });

        //Create Random Cards in Each Lane 
        AddRandomUnitsToLane(Player1.Lanes);
        AddRandomUnitsToLane(Player2.Lanes);

        _battleSystem = new DefaultBattleSystem();
    }

    private void AddRandomUnitsToLane(List<Lane> lanes)
    {
        CardDatabase db = new CardDatabase();
        var unitsOnly = db.GetAll().Where(c => c.GetType() == typeof(UnitCardData)).ToList();

        var rng = new Random();

        foreach (Lane lane in lanes)
        {
            var randomIndex = rng.Next(0, unitsOnly.Count());
            var instancedCard = new CardInstance(unitsOnly[randomIndex]);
            lane.UnitInLane = instancedCard;
        }
    }
}
