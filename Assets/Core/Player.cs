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
    #endregion

    public Player(int numberOfLanes)
    {
        Hand = new Hand();
        Lanes = new List<Lane>();
        DiscardPile = new DiscardPile();

        for (int i = 0; i < numberOfLanes; i++)
        {
            Lanes.Add(new Lane()
            {
                LaneId = (i + 1)
            });
        }
    }

    #region Public Properties
    public int PlayerId { get => _playerId; set => _playerId = value; }
    public int Health { get => _health; set => _health = value; }
    public List<Lane> Lanes { get => _lanes; set => _lanes = value; }
    public Hand Hand { get => _hand; set => _hand = value; }
    public DiscardPile DiscardPile { get => _discardPile; set => _discardPile = value; }

    #endregion

    #region Public Methods

    public bool IsOwnerOfCard(CardInstance card)
    {
        return Hand.Cards.Contains(card) || DiscardPile.Cards.Contains(card) || Lanes.SelectMany(l => l.Cards).Contains(card);
    }

    #endregion
}

