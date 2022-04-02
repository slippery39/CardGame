using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This class represents a card as it exists inside the game state.
//It is essentially just a wrapper class around an existing card data.
public class CardInstance : CardGameEntity
{
    private BaseCardData _originalCardData;
    private BaseCardData _currentCardData;
    private int _ownerId;
    private bool _isSummoningSick = true;

    #region Public Properties
    public BaseCardData CurrentCardData { get => _currentCardData; set => _currentCardData = value; }
    public int OwnerId { get => _ownerId; set => _ownerId = value; }
    public override string Name { get => _currentCardData.Name; set => _currentCardData.Name = value; }
    public string RulesText { get => _currentCardData.RulesText; set => _currentCardData.Name = value; }
    public string ManaCost { get => _currentCardData.ManaCost; set => _currentCardData.ManaCost = value; }
    public string CardType { get => _currentCardData.CardType; }
    
    public bool IsSummoningSick { get => _isSummoningSick; set => _isSummoningSick = value; }
    public List<CardAbility> Abilities { get => _currentCardData.Abilities; }


    //Temporary sort of unsafe properties for accessing Unit Power and Toughness,
    //While I figure out how I actually want to do this properly in a more type safe way.
    //I was casting CardInstance.CurrentCardData all over my code base anyways,
    //at least this keeps it in one place.
    public int Power
    {
        get
        {
            if (_currentCardData is UnitCardData)
            {
                return ((UnitCardData)_currentCardData).Power;
            }
            else
            {
                throw new Exception("Card Instance is not a creature, cannot access the power property");
            }
        }
        set
        {
            if (_currentCardData is UnitCardData)
            {
                ((UnitCardData)_currentCardData).Power = value;
            }
        }
    }

    public int Toughness
    {
        get
        {
            if (_currentCardData is UnitCardData)
            {
                return ((UnitCardData)_currentCardData).Toughness;
            }
            else
            {
                throw new Exception("Card Instance is not a creature, cannot access the power property");
            }
        }
        set
        {
            if (_currentCardData is UnitCardData)
            {
                ((UnitCardData)_currentCardData).Toughness = value;
            }
        }

    }

    #endregion

    public CardInstance(BaseCardData cardData)
    {
        _originalCardData = cardData;
        _currentCardData = cardData.Clone();
    }

    #region Public Methods

    public List<T> GetAbilities<T>()
    {
        return _currentCardData.GetAbilities<T>();
    }
    #endregion
}

