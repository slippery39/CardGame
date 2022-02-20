using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This class represents a card as it exists inside the game state.

public class CardInstance
{
    private BaseCardData _originalCardData;
    private BaseCardData _currentCardData;
    private int _ownerId;

    #region Public Properties
    public BaseCardData CurrentCardData { get => _currentCardData; set => _currentCardData = value; }
    public int OwnerId { get => _ownerId; set => _ownerId = value; }
    #endregion

    public CardInstance(BaseCardData cardData)
    {
        _originalCardData = cardData;
        _currentCardData = cardData.Clone();
    }
}

