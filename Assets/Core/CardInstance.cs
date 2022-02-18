using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This class represents a card as it exists inside the game state.
[System.Serializable]
public class CardInstance
{
    private BaseCardData _originalCardData;
    private BaseCardData _currentCardData;

    public BaseCardData CurrentCardData { get => _currentCardData; set => _currentCardData = value; }

    public CardInstance(BaseCardData cardData)
    {
        _originalCardData = cardData;
        _currentCardData = cardData.Clone();
    }
}

