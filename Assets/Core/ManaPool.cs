using System;
using System.Collections.Generic;
using System.Linq;


public static class ManaHelper
{
    private static List<ManaType> _manaTypes;
    public static List<ManaType> GetManaTypes()
    {
        //Cache the result, since it shouldn't change at runtime.
        if (_manaTypes == null)
        {
            _manaTypes = Enum.GetValues(typeof(ManaType)).Cast<ManaType>().ToList();
        }
        return _manaTypes;
    }

    public static Dictionary<ManaType, int> CreateManaDict()
    {
        Dictionary<ManaType, int> manaDict = new Dictionary<ManaType, int>();

        foreach (var manaType in GetManaTypes())
        {
            manaDict.Add(manaType, 0);
        }

        return manaDict;
    }
    public static Dictionary<ManaType, int> ManaStringToColorCounts(string manaCost)
    {
        var colorCounts = CreateManaDict();
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
                    colorCounts[ManaType.Colorless] = Convert.ToInt32(currentNumber);
                    currentNumber = "";
                }

                //Add the color type
                switch (manaChars[i].ToString().ToUpper())
                {
                    case "W": colorCounts[ManaType.White]++; break;
                    case "U": colorCounts[ManaType.Blue]++; break;
                    case "B": colorCounts[ManaType.Black]++; break;
                    case "R": colorCounts[ManaType.Red]++; break;
                    case "G": colorCounts[ManaType.Green]++; break;
                    case "*": colorCounts[ManaType.Any]++; break;
                    default: throw new Exception($@"Unaccounted for mana symbol {manaChars[i].ToString().ToUpper()}");
                }
            }
        }
        //Need this for colorless only strings..
        if (currentNumber.Length > 0)
        {
            colorCounts[ManaType.Colorless] = Convert.ToInt32(currentNumber);
        }
        return colorCounts;
    }
}

public class ManaPool
{

    //TODO - need to be able to differentiate between spent mana and saved mana.
    //i.e. we should have a CurrentManaByType and a CurrentTotalMana
    //the TotalMana and ManaByType properties will do the overall (i.e. mana we would have if we didn't spend any yet)
    //the CurrentManaByType and CurrentTotalMana will give us the mana we have left after spending.
    //To Reset we just set CurrentManaByType to the same values as ManaByType.
    //If we want temp mana we can just add to the CurrentManaByType without addinf to the ManaByType.

    /// <summary>
    /// The total converted mana in the pool.
    /// </summary>
    public int TotalMana
    {
        get
        {
            int manaCount = 0;
            foreach (var manaType in ManaByType.Keys)
            {
                manaCount += ManaByType[manaType];
            }
            return manaCount;
        }
    }
    public int CurrentTotalMana
    {
        get
        {
            int manaCount = 0;
            foreach (var manaType in CurrentManaByType.Keys)
            {
                manaCount += CurrentManaByType[manaType];
            }
            return manaCount;
        }
    }
    /// <summary>
    /// Returns the total available mana NOT accounting for any spent.
    /// </summary>
    public Dictionary<ManaType, int> ManaByType { get; set; }
    /// <summary>
    /// Returns the available mana we have accounting for any spent.
    /// </summary>
    public Dictionary<ManaType, int> CurrentManaByType { get; set; }

    public ManaPool()
    {
        ManaByType = ManaHelper.CreateManaDict();
        CurrentManaByType = ManaHelper.CreateManaDict();
    }
    public void SpendMana(ManaType type, int amount)
    {
        CurrentManaByType[type] -= amount;
    }

    public void AddMana(ManaType type, int amount)
    {
        //Need to add to both so they have access to it in the same turn.
        //If they don't need access to it, then 
        CurrentManaByType[type] += amount;
        ManaByType[type] += amount;
    }

    public void AddTemporaryMana(ManaType type, int amount)
    {
        CurrentManaByType[type] += amount;
    }

    public void ResetMana()
    {
        //init the mana by type and temp mana by type dictionaries
        foreach (var manaType in ManaHelper.GetManaTypes())
        {
            CurrentManaByType[manaType] = ManaByType[manaType];
        }
    }

}

public enum ManaType
{
    Colorless,
    White,
    Blue,
    Black,
    Red,
    Green,
    Any //is a placeholder for mana of any color.
}