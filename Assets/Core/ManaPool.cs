using System;
using System.Collections.Generic;
using System.Linq;

public class ManaAndEssence
{
    public int Mana { get; set; } = 0;
    public Dictionary<EssenceType, int> Essence { get; set; }

    public ManaAndEssence()
    {
        Essence = ManaAndEssenceHelper.CreateManaDict();
    }

    public ManaAndEssence(string manaCost)
    {
        Essence = ManaAndEssenceHelper.CreateManaDict();
        //Need to process the string...

        //From Left To Right
        //Count the number of colors symbols (i.e. should be letters)
        //Then Count the number as the generic symbol

        //Mana Costs should be in Magic Format (i.e. 3U, 5BB) with the generic mana cost first.
        var manaChars = manaCost.ToCharArray();
        string currentNumber = ""; //should only be 1 currentNumber

        for (int i = 0; i < manaChars.Length; i++)
        {
            if (manaChars[i].IsNumeric())
            {
                currentNumber += manaChars[i].ToString();
            }
            else
            {
                if (currentNumber.Length > 0)
                {
                    Mana = Convert.ToInt32(currentNumber);
                    currentNumber = "";
                }

                //Add the color type
                switch (manaChars[i].ToString().ToUpper())
                {
                    case "W": Essence[EssenceType.White]++; break;
                    case "U": Essence[EssenceType.Blue]++; break;
                    case "B": Essence[EssenceType.Black]++; break;
                    case "R": Essence[EssenceType.Red]++; break;
                    case "G": Essence[EssenceType.Green]++; break;
                    case "*": Essence[EssenceType.Any]++; break;
                    default: throw new Exception($@"Unaccounted for essence symbol {manaChars[i].ToString().ToUpper()}");
                }
            }
        }
        //Need this for colorless only strings..
        if (currentNumber.Length > 0)
        {
            Mana = Convert.ToInt32(currentNumber);
        }
    }
    public int TotalSumOfEssence => Essence.Values.Sum();
    public bool IsEnoughToPayCost(ManaAndEssence cost)
    {
        var colorsCost = cost.Essence;
        var colorsPaying = Essence;

        bool hasEnoughColor = true;

        //We are going to compare the player's mana pool with the colors needed in the card.
        //If the player does not have enough color he cannot play the card.
        foreach (var color in cost.Essence.Keys)
        {
            if (colorsCost[color] == 0) continue;
            if (colorsCost[color] > colorsPaying[color])
            {
                hasEnoughColor = false;
                break;
            }
        }

        //Do a last check of the Any ("*") type mana to see if we can still play the card off of that.
        if (!hasEnoughColor)
        {
            hasEnoughColor = CanPayEssenceWithAny(cost);
        }

        bool hasEnoughTotalMana = Mana >= cost.Mana;

        return hasEnoughColor && hasEnoughTotalMana;
    }

    private bool CanPayEssenceWithAny(ManaAndEssence cost)
    {
        return Essence[EssenceType.Any] >= cost.TotalSumOfEssence;
    }
}


public static class ManaAndEssenceHelper
{
    private static List<EssenceType> _manaTypes;
    public static List<EssenceType> GetManaTypes()
    {
        //Cache the result, since it shouldn't change at runtime.
        if (_manaTypes == null)
        {
            _manaTypes = Enum.GetValues(typeof(EssenceType)).Cast<EssenceType>().ToList();
        }
        return _manaTypes;
    }

    public static Dictionary<EssenceType, int> CreateManaDict()
    {
        Dictionary<EssenceType, int> manaDict = new Dictionary<EssenceType, int>();

        foreach (var manaType in GetManaTypes())
        {
            manaDict.Add(manaType, 0);
        }

        return manaDict;
    }
}

public class ManaPool
{

    private ManaAndEssence _totalManaAndEssence;
    private ManaAndEssence _currentManaAndEssence;

    //TODO - need to be able to differentiate between spent mana and saved mana.
    //i.e. we should have a CurrentManaByType and a CurrentTotalMana
    //the TotalMana and ManaByType properties will do the overall (i.e. mana we would have if we didn't spend any yet)
    //the CurrentManaByType and CurrentTotalMana will give us the mana we have left after spending.
    //To Reset we just set CurrentManaByType to the same values as ManaByType.
    //If we want temp mana we can just add to the CurrentManaByType without addinf to the ManaByType.

    public ManaAndEssence CurrentManaAndEssence => _currentManaAndEssence;
    public ManaAndEssence TotalManaAndEssence => _totalManaAndEssence;

    /// <summary>
    /// The total converted mana in the pool.
    /// </summary>
    public int TotalMana
    {
        get { return _totalManaAndEssence.Mana; }
        set { _totalManaAndEssence.Mana = value; }

    }
    public int CurrentTotalMana
    {
        get { return _currentManaAndEssence.Mana; }
        set { _currentManaAndEssence.Mana = value; }
    }
    /// <summary>
    /// Returns the total available mana NOT accounting for any spent.
    /// </summary>
    public Dictionary<EssenceType, int> TotalEssence
    {
        get { return _totalManaAndEssence.Essence; }
        set { _totalManaAndEssence.Essence = value; }
    }
    /// <summary>
    /// Returns the available mana we have accounting for any spent.
    /// </summary>
    public Dictionary<EssenceType, int> CurrentEssence
    {
        get { return _currentManaAndEssence.Essence; }
        set { _currentManaAndEssence.Essence = value; }
    }

    public ManaPool()
    {
        _totalManaAndEssence = new ManaAndEssence();
        _currentManaAndEssence = new ManaAndEssence();
    }

    //How do we do this?
    public void SpendMana(int amount)
    {
        CurrentTotalMana -= amount;
    }

    public void SpendEssence(EssenceType type, int amount)
    {
        //Spend as much in the amount as possible, the rest should go into Any.
        int remainder = Math.Max(amount - CurrentEssence[type],0);
        CurrentEssence[type] -= Math.Min(amount, CurrentEssence[type]);
        if (remainder > 0)
        {
            CurrentEssence[EssenceType.Any] -= remainder;
        };
    }

    public void AddManaAndEssence(EssenceType type, int manaAmount)
    {
        TotalMana += manaAmount;
        CurrentTotalMana += manaAmount;
        CurrentEssence[type] += manaAmount;
        TotalEssence[type] += manaAmount;
    }

    public void AddMana(int amount)
    {
        //Need to add to both so they have access to it in the same turn.
        //If they don't need access to it, then 
        TotalMana += amount;
        CurrentTotalMana += amount;

    }

    public void AddTemporaryEssenceAndMana(EssenceType type, int amount)
    {
        CurrentTotalMana += amount;
        CurrentEssence[type] += amount;
    }

    public void AddEssence(EssenceType type, int amount)
    {
        CurrentEssence[type] += amount;
        TotalEssence[type] += amount;
    }

    public void AddTemporaryEssence(EssenceType type, int amount)
    {
        CurrentEssence[type] += amount;
    }

    public void ResetManaAndEssence()
    {
        //Reset the total mana
        CurrentTotalMana = TotalMana;
        //init the mana by type and temp mana by type dictionaries
        foreach (var manaType in ManaAndEssenceHelper.GetManaTypes())
        {
            CurrentEssence[manaType] = TotalEssence[manaType];
        }
    }

}

public enum EssenceType
{
    White,
    Blue,
    Black,
    Red,
    Green,
    Any //is a placeholder for mana of any color.
}