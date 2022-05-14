﻿using System;
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

    public int ConvertedManaCost
    {
        get
        {
            //From Left To Right
            //Count the number of colors symbols (i.e. should be letters)
            //Then Count the number as the generic symbol

            //Mana Costs should be in Magic Format (i.e. 3U, 5BB) with the generic mana cost first.
            var manaChars = ManaCost.ToCharArray();
            int convertedCost = 0;
            string currentNumber = ""; //should only be 1 currentNumber
            for (int i = 0; i < manaChars.Length; i++)
            {
                if (manaChars[i].IsNumeric())
                {
                    currentNumber += manaChars[i].ToString();
                    //We can't just convert to an int, as it will give us the char code not the numeric value... 
                    //Calling Char.GetNumericValue gives us the actual numeric value of the char in question.
                    convertedCost += Convert.ToInt32(Char.GetNumericValue(manaChars[i]));
                }
                else
                {
                    if (currentNumber.Length > 0)
                    {
                        convertedCost += Convert.ToInt32(currentNumber);
                        currentNumber = "";
                    }
                    convertedCost++; //if its not a numeric symbol than it should be a colored symbol and we just add 1.
                }
            }
            return convertedCost;
        }
    }


    public string ManaCost { get => _currentCardData.ManaCost; set => _currentCardData.ManaCost = value; }
    public string CardType { get => _currentCardData.CardType; }

    public bool IsSummoningSick { get => _isSummoningSick; set => _isSummoningSick = value; }
    public List<CardAbility> Abilities { get => _currentCardData.Abilities; }

    //How do we figure this out?
    public List<ContinuousEffect> ContinuousEffects { get; set; }


    private int _powerWithoutMods;
    private int _toughnessWithoutMods;

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
                //return - power without mods, + mods power;
                int calculatedPower = _powerWithoutMods;

                var pumpContinuousEffects = ContinuousEffects.SelectMany(e => e.SourceAbility.Effects).Where(e => e is StaticPumpEffect).Cast<StaticPumpEffect>();
                if (pumpContinuousEffects.Any())
                {
                    foreach (var pumpEffect in pumpContinuousEffects)
                    {
                        calculatedPower += pumpEffect.Power;
                    }
                }

                return calculatedPower;
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
                _powerWithoutMods = value;
            }
        }
    }

    public int Toughness
    {
        get
        {
            if (_currentCardData is UnitCardData)
            {
                //return - power without mods, + mods power;
                int calculatedToughness = _toughnessWithoutMods;

                var pumpContinuousEffects = ContinuousEffects.SelectMany(e => e.SourceAbility.Effects).Where(e => e is StaticPumpEffect).Cast<StaticPumpEffect>();
                if (pumpContinuousEffects.Any())
                {
                    foreach (var pumpEffect in pumpContinuousEffects)
                    {
                        calculatedToughness += pumpEffect.Toughness;
                    }
                }

                calculatedToughness = calculatedToughness - DamageTaken;

                return calculatedToughness;
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
                _toughnessWithoutMods = value;
            }
        }

    }

    public int DamageTaken { get; set; }

    #endregion

    public CardInstance(BaseCardData cardData)
    {
        ContinuousEffects = new List<ContinuousEffect>();
        _originalCardData = cardData;
        _currentCardData = cardData.Clone();

        if (_currentCardData is UnitCardData)
        {
            var unitCardData = (UnitCardData)_currentCardData;
            _powerWithoutMods = unitCardData.Power;
            _toughnessWithoutMods = unitCardData.Toughness;
        }
    }

    #region Public Methods

    public List<T> GetAbilities<T>()
    {
        return _currentCardData.GetAbilities<T>();
    }
    #endregion
}

