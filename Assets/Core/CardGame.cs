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
    private int _startingHandSize = 5;
    private IBattleSystem _battleSystem;
    private IDamageSystem _damageSystem;
    private IHealingSystem _healingSystem;
    private ICardGameLogger _cardGameLogger;

    #region Public Properties
    public Player Player1 { get => _players.Where(p => p.PlayerId == 1).FirstOrDefault(); }
    public Player Player2 { get => _players.Where(p => p.PlayerId == 2).FirstOrDefault(); }

    public int ActivePlayerId { get => _activePlayerId; set => _activePlayerId = value; }
    public Player ActivePlayer { get => _players.Where(p => p.PlayerId == ActivePlayerId).FirstOrDefault(); }
    public Player InactivePlayer { get => _players.Where(p => p.PlayerId != ActivePlayerId).FirstOrDefault(); }
    public ICardGameLogger Logger { get => _cardGameLogger; }
    #region Systems
    public IBattleSystem BattleSystem { get => _battleSystem; set => _battleSystem = value; }
    public IDamageSystem DamageSystem { get => _damageSystem; set => _damageSystem = value; }
    public IHealingSystem HealingSystem { get => _healingSystem; set => _healingSystem = value; }
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
        AddRandomUnitsToLane(Player1);
        AddRandomUnitsToLane(Player2);

        AddRandomSpellsToHand(Player1);

        _battleSystem = new DefaultBattleSystem();
        _damageSystem = new DefaultDamageSystem();
        _healingSystem = new DefaultHealingSystem();

        _cardGameLogger = new UnityCardGameLogger();

        //TODO - some sort of check to make sure all systems are initialized?
        //maybe have 
    }

    public Player GetOwnerOfUnit(CardInstance unitInstance)
    {
        return _players.Where(p => p.PlayerId == unitInstance.OwnerId).FirstOrDefault();
    }

    public void AddCardToGame(Player player, BaseCardData data, IZone zone)
    {
        var cardInstance = new CardInstance(data);
        cardInstance.OwnerId = player.PlayerId;
        zone.Add(cardInstance);
    }

    //For general console logging purposes.
    public void Log(string message)
    {
        Logger.Log(message);
    }
    private void AddRandomUnitsToLane(Player player)
    {
        CardDatabase db = new CardDatabase();
        var unitsOnly = db.GetAll().Where(c => c.GetType() == typeof(UnitCardData)).ToList();

        var rng = new Random();

        foreach (Lane lane in player.Lanes)
        {
            var randomIndex = rng.Next(0, unitsOnly.Count());
            AddCardToGame(player, unitsOnly[randomIndex], lane);
        }
    }

    private void AddRandomSpellsToHand(Player player)
    {
        CardDatabase db = new CardDatabase();
        var spellsOnly = db.GetAll().Where(c => c is SpellCardData).ToList();

        var rng = new Random();

        for (int i = 0; i < _startingHandSize; i++)
        {
            var randomIndex = rng.Next(0, spellsOnly.Count());
            AddCardToGame(player, spellsOnly[randomIndex], player.Hand);
        }
    }
}
