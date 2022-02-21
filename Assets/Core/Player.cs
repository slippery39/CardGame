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
    #endregion

    public Player(int numberOfLanes)
    {

        Hand = new Hand();
        Lanes = new List<Lane>();
        
        for (int i = 0; i < numberOfLanes; i++)
        {
            Lanes.Add(new Lane()
            {
                LaneId = (i+1)
            });
        }
    }

    #region Public Properties
    public int PlayerId { get => _playerId; set => _playerId = value; }
    public int Health { get => _health; set => _health = value; }
    public List<Lane> Lanes { get => _lanes; set => _lanes = value; }
    public Hand Hand { get => _hand; set => _hand = value; }
    #endregion
}

