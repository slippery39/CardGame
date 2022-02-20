using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CardGame
{

    private List<Player> _players;
    private int _activePlayerId = 1;
    private int _numberOfLanes = 5;
    private int _startingPlayerHealth = 100;
    private IBattleSystem _battleSystem;
    private IDamageSystem _damageSystem;
    private IHealingSystem _healingSystem;

    #region Public Properties
    public Player Player1 { get => _players.Where(p => p.PlayerId == 1).FirstOrDefault(); }
    public Player Player2 { get => _players.Where(p => p.PlayerId == 2).FirstOrDefault(); }

    public int ActivePlayerId { get => _activePlayerId; set => _activePlayerId = value; }
    public Player ActivePlayer { get => _players.Where(p => p.PlayerId == ActivePlayerId).FirstOrDefault(); }
    public Player InactivePlayer { get => _players.Where(p => p.PlayerId != ActivePlayerId).FirstOrDefault(); }
    #region Systems
    public IBattleSystem BattleSystem { get => _battleSystem; set => _battleSystem = value; }
    public IDamageSystem DamageSystem { get => _damageSystem; set => _damageSystem = value; }
    public IHealingSystem HealingSystem { get => _healingSystem; set => _healingSystem = value;}
    #endregion
    #endregion

    public CardGame()
    {
        _players = new List<Player>();

        _players.Add(new Player(_numberOfLanes)
        {
            PlayerId = 1,
            Health = _startingPlayerHealth
        });
        _players.Add(new Player(_numberOfLanes)
        {
            PlayerId = 2,
            Health = _startingPlayerHealth
        });

        //Create Random Cards in Each Lane 
        //AddRandomUnitsToLane(Player1.Lanes);
        //AddRandomUnitsToLane(Player2.Lanes);
        //SetupCantBlockTestLanes();
        //SetupFlyingTestLanes();
        SetupLifelinkTestLanes();
        //SetupDeathtouchTestLanes();

        _battleSystem = new DefaultBattleSystem();
        _damageSystem = new DefaultDamageSystem();
        _healingSystem = new DefaultHealingSystem();

        //TODO - some sort of check to make sure all systems are initialized?
            //maybe have 
    }

    public Player GetOwnerOfUnit(CardInstance unitInstance)
    {
        return _players.Where(p => p.PlayerId == unitInstance.OwnerId).FirstOrDefault();
    }

    public CardInstance AddCardToGame(Player player, BaseCardData data)
    {
        var cardInstance = new CardInstance(data);
        cardInstance.OwnerId = player.PlayerId;
        return cardInstance;
    }

    //WARNING: Obsolete
    //This no longer works since a card instance needs to be registered to a player.
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

    private void SetupCantBlockTestLanes()
    {
        var db = new CardDatabase();

        var hexPlateGolem = db.GetCardData("Hexplate Golem");
        Player1.Lanes[0].UnitInLane = AddCardToGame(Player1, hexPlateGolem);

        var goblinRaider = db.GetCardData("Goblin Raider");
        Player2.Lanes[0].UnitInLane = AddCardToGame(Player2, goblinRaider);
    }

    private void SetupFlyingTestLanes()
    {
        //Test Cases
        //Flying -> Non Flying - should attack directly
        //Flying -> Flying -should attack eachother
        var db = new CardDatabase();

        var stormCrow = db.GetCardData("Storm Crow");
        var hexPlateGolem = db.GetCardData("Hexplate Golem");
        Player1.Lanes[0].UnitInLane = AddCardToGame(Player1, stormCrow);
        Player2.Lanes[0].UnitInLane = AddCardToGame(Player2, hexPlateGolem);

        Player1.Lanes[1].UnitInLane = AddCardToGame(Player1, stormCrow);
        Player2.Lanes[1].UnitInLane = AddCardToGame(Player2, stormCrow);
    }

    private void SetupLifelinkTestLanes()
    {
        //Test Cases
        //2 Lifelinkers in one lane
        //Life linkers in lanes with no defenders
        var db = new CardDatabase();

        var sunstriker = db.GetCardData("Sunstriker");
        Player1.Lanes[0].UnitInLane = AddCardToGame(Player1, sunstriker);
        Player2.Lanes[0].UnitInLane = AddCardToGame(Player2, sunstriker);

        Player1.Lanes[1].UnitInLane = AddCardToGame(Player1, sunstriker);

        Player2.Lanes[2].UnitInLane = AddCardToGame(Player2, sunstriker);

    }

    private void SetupDeathtouchTestLanes()
    {
        var db = new CardDatabase();

        var rats = db.GetCardData("Typhoid Rats");
        var hexPlateGolem = db.GetCardData("Hexplate Golem");
        Player1.Lanes[0].UnitInLane = AddCardToGame(Player1, rats);
        Player2.Lanes[0].UnitInLane = AddCardToGame(Player2, hexPlateGolem);
    }
}
