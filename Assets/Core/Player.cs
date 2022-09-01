using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Player : CardGameEntity
{
    #region Private Fields
    private int _playerId;
    private int _health;
    private List<Lane> _lanes;
    private Hand _hand;
    private DiscardPile _discardPile;
    private Deck _deck;
    private Zone _exile;
    private IZone _items;
    private ManaPool _manaPool;

    private string _name;
    #endregion

    public Player(int numberOfLanes)
    {
        _manaPool = new ManaPool();
        Hand = new Hand();
        Lanes = new List<Lane>();
        DiscardPile = new DiscardPile();
        Deck = new Deck();
        Exile = new Zone(ZoneType.Exile, "Exile");
        Items = new Zone(ZoneType.InPlay, "Items");
        InitLanes(numberOfLanes);
    }

    #region Public Properties
    public int PlayerId { get => _playerId; set => _playerId = value; }
    public int Health { get => _health; set => _health = value; }
    public List<Lane> Lanes { get => _lanes; set => _lanes = value; }
    public Hand Hand { get => _hand; set => _hand = value; }
    public DiscardPile DiscardPile { get => _discardPile; set => _discardPile = value; }
    public Deck Deck { get => _deck; set => _deck = value; }
    //public int Mana { get => _mana; set => _mana = value; }
    public Zone Exile { get => _exile; set => _exile = value; }
    public int ManaPlayedThisTurn { get; set; } = 0;
    public int TotalManaThatCanBePlayedThisTurn
    {
        get
        {
            var manaThatCanBePlayed = 1;
            var manaPlayedModifications = Modifications.GetOfType<IModifyManaPerTurn>();

            foreach (var mod in manaPlayedModifications)
            {
                manaThatCanBePlayed = mod.ModifyManaPerTurn(null, this, manaThatCanBePlayed);
            }
            return manaThatCanBePlayed;
        }
    }

    public ManaPool ManaPool { get => _manaPool; }
    public override string Name { get => $@"Player {PlayerId}"; set { _name = value; } }

    public IZone Items { get => _items; set => _items = value; }

    #endregion

    #region Public Methods
    public List<CardInstance> GetUnitsInPlay()
    {
        return Lanes.Where(l => l.IsEmpty() == false).Select(l => l.UnitInLane).ToList();
    }

    public List<CardInstance> GetCardsInPlay()
    {
        var cardsInPlay = GetUnitsInPlay();
        cardsInPlay.AddRange(Items.Cards);
        return cardsInPlay;
    }

    #endregion

    #region Private Methods
    void InitLanes(int numberOfLanes)
    {
        for (int i = 0; i < numberOfLanes; i++)
        {
            Lanes.Add(new Lane()
            {
                LaneId = (i + 1)
            });
        }
    }

    public List<Lane> GetEmptyLanes()
    {
        return _lanes.Where(l => l.IsEmpty()).ToList();
    }
    #endregion
}

