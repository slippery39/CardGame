using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Player
{
    #region Private Fields
    private int _playerId;
    private int _health;
    private List<Lane> _lanes;
    private Hand _hand;
    private DiscardPile _discardPile;
    private Deck deck;
    #endregion

    public Player(int numberOfLanes)
    {
        Hand = new Hand();
        Lanes = new List<Lane>();
        DiscardPile = new DiscardPile();
        Deck = new Deck();

        InitLanes(numberOfLanes);
        InitDeck();
    }

    #region Public Properties
    public int PlayerId { get => _playerId; set => _playerId = value; }
    public int Health { get => _health; set => _health = value; }
    public List<Lane> Lanes { get => _lanes; set => _lanes = value; }
    public Hand Hand { get => _hand; set => _hand = value; }
    public DiscardPile DiscardPile { get => _discardPile; set => _discardPile = value; }
    public Deck Deck { get => deck; set => deck = value; }

    #endregion

    #region Public Methods

    public bool IsOwnerOfCard(CardInstance card)
    {
        return Hand.Cards.Contains(card) || DiscardPile.Cards.Contains(card) || Lanes.SelectMany(l => l.Cards).Contains(card);
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

    void InitDeck()
    {
        var cardDB = new CardDatabase();
        var spells = cardDB.GetAll().Where(card => card is SpellCardData).ToList();
        Deck = new Deck();
        for (int i = 0; i < 60; i++)
        {
            Deck.Add( new CardInstance(spells.Randomize().ToList()[0]));
        }
    }
    #endregion
}

