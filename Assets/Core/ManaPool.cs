using System;
using System.Collections.Generic;
using System.Linq;

public class Mana
{
    public int ColorlessMana { get; set; } = 0;
    public Dictionary<ManaType, int> ColoredMana { get; set; }

    private static List<ManaType> _manaTypes;

    public Mana()
    {
        ColoredMana = Mana.CreateManaDict();
    }

    public Mana(string manaCost)
    {
        ColoredMana = Mana.CreateManaDict();

        if (manaCost == null)
        {
            //edge case if for whatever reason trying to initialize as null.
            return;
        }
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
                    ColorlessMana = Convert.ToInt32(currentNumber);
                    currentNumber = "";
                }

                //Add the color type
                switch (manaChars[i].ToString().ToUpper())
                {
                    case "W": ColoredMana[ManaType.White]++; break;
                    case "U": ColoredMana[ManaType.Blue]++; break;
                    case "B": ColoredMana[ManaType.Black]++; break;
                    case "R": ColoredMana[ManaType.Red]++; break;
                    case "G": ColoredMana[ManaType.Green]++; break;
                    case "*": ColoredMana[ManaType.Any]++; break;
                    default: throw new Exception($@"Unaccounted for mana symbol {manaChars[i].ToString().ToUpper()}");
                }
            }
        }
        //Need this for colorless only strings..
        if (currentNumber.Length > 0)
        {
            ColorlessMana = Convert.ToInt32(currentNumber);
        }
    }
    public int TotalSumOfColoredMana => ColoredMana.Values.Sum();
    public bool IsEnoughToPayCost(Mana cost)
    {
        var colorsCost = cost.ColoredMana;
        var colorsPaying = ColoredMana;

        bool hasEnoughColor = true;

        //We are going to compare the player's mana pool with the colors needed in the card.
        //If the player does not have enough color he cannot play the card.
        foreach (var color in cost.ColoredMana.Keys)
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
            hasEnoughColor = CanPayManaWithAny(cost);
        }

        bool hasEnoughColorlessMana = ColorlessMana >= cost.ColorlessMana;

        return hasEnoughColor && hasEnoughColorlessMana;
    }

    public string ToManaString()
    {
        var str = "";

        if (ColorlessMana == 0 && TotalSumOfColoredMana == 0)
        {
            return "0";
        }

        //We should not show 0 for mana costs that also have an essence component.
        if (ColorlessMana != 0)
        {
            str = ColorlessMana.ToString();
        }

        foreach (var manaType in ColoredMana)
        {
            var manaTypeAsString = ManaTypeToString(manaType.Key);
            str += string.Concat(Enumerable.Repeat(manaTypeAsString, manaType.Value));
        }

        return str;
    }

    private bool CanPayManaWithAny(Mana cost)
    {
        return ColoredMana[ManaType.Any] >= cost.TotalSumOfColoredMana;
    }

    private string ManaTypeToString(ManaType type)
    {
        var dictionary = new Dictionary<ManaType, string>();
        dictionary.Add(ManaType.Blue, "U");
        dictionary.Add(ManaType.Green, "G");
        dictionary.Add(ManaType.Red, "R");
        dictionary.Add(ManaType.Black, "B");
        dictionary.Add(ManaType.White, "W");
        dictionary.Add(ManaType.Any, "*");

        return dictionary[type];
    }

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

    public void AddFromString(string manaStr)
    {
        var manaToAdd = new Mana(manaStr);

        foreach (var type in manaToAdd.ColoredMana.Keys)
        {
            ColoredMana[type] += manaToAdd.ColoredMana[type];
        }
        ColorlessMana += manaToAdd.ColorlessMana;
    }


}

public class ManaPool
{

    private Mana _totalMana;
    private Mana _currentMana;

    //TODO - need to be able to differentiate between spent mana and saved mana.
    //i.e. we should have a CurrentManaByType and a CurrentTotalMana
    //the TotalMana and ManaByType properties will do the overall (i.e. mana we would have if we didn't spend any yet)
    //the CurrentManaByType and CurrentTotalMana will give us the mana we have left after spending.
    //To Reset we just set CurrentManaByType to the same values as ManaByType.
    //If we want temp mana we can just add to the CurrentManaByType without addinf to the ManaByType.

    public Mana CurrentMana => _currentMana;
    public Mana TotalMana => _totalMana;

    /// <summary>
    /// The total converted mana in the pool.
    /// </summary>
    public int TotalColorlessMana
    {
        get { return _totalMana.ColorlessMana; }
        set { _totalMana.ColorlessMana = value; }

    }
    public int CurrentColorlessMana
    {
        get { return _currentMana.ColorlessMana; }
        set { _currentMana.ColorlessMana = value; }
    }
    /// <summary>
    /// Returns the total available mana NOT accounting for any spent.
    /// </summary>
    public Dictionary<ManaType, int> TotalColoredMana
    {
        get { return _totalMana.ColoredMana; }
        set { _totalMana.ColoredMana = value; }
    }
    /// <summary>
    /// Returns the available mana we have accounting for any spent.
    /// </summary>
    public Dictionary<ManaType, int> CurrentColoredMana
    {
        get { return _currentMana.ColoredMana; }
        set { _currentMana.ColoredMana = value; }
    }

    public ManaPool()
    {
        _totalMana = new Mana();
        _currentMana = new Mana();
    }

    //How do we do this?
    public void SpendColorlessMana(int amount)
    {
        CurrentColorlessMana -= amount;
    }

    public void AddMana(string manaToAdd)
    {
        var manaToAdd2 = new Mana(manaToAdd);

        AddColorlessMana(manaToAdd2.ColorlessMana);

        foreach (var color in manaToAdd2.ColoredMana.Keys)
        {
            AddColor(color, manaToAdd2.ColoredMana[color]);
        }
    }

    public void AddTotalMana(string manaToAdd)
    {
        var manaToAdd2 = new Mana(manaToAdd);

        _totalMana.ColorlessMana += manaToAdd2.ColorlessMana;

        foreach (var color in manaToAdd2.ColoredMana.Keys)
        {
            _totalMana.ColoredMana[color] += manaToAdd2.ColoredMana[color];
        }
    }

    public void AddTemporary(string manaToAdd)
    {
        var manaToAdd2 = new Mana(manaToAdd);

        AddTemporaryColorless(manaToAdd2.ColorlessMana);

        foreach (var color in manaToAdd2.ColoredMana.Keys)
        {
            AddTemporaryColor(color, manaToAdd2.ColoredMana[color]);
        }
    }

    public void SpendColoredMana(ManaType type, int amount)
    {
        //Spend as much in the amount as possible, the rest should go into Any.
        int remainder = Math.Max(amount - CurrentColoredMana[type], 0);
        CurrentColoredMana[type] -= Math.Min(amount, CurrentColoredMana[type]);
        if (remainder > 0)
        {
            CurrentColoredMana[ManaType.Any] -= remainder;
        };
    }

    private void AddColorAndColorless(ManaType type, int manaAmount)
    {
        TotalColorlessMana += manaAmount;
        CurrentColorlessMana += manaAmount;
        CurrentColoredMana[type] += manaAmount;
        TotalColoredMana[type] += manaAmount;
    }

    private void AddColorlessMana(int amount)
    {
        //Need to add to both so they have access to it in the same turn.
        //If they don't need access to it, then 
        TotalColorlessMana += amount;
        CurrentColorlessMana += amount;

    }

    private void AddTemporaryColorless(int amount)
    {
        CurrentColorlessMana += amount;
    }

    private void AddTemporaryColorAndColorless(ManaType type, int amount)
    {
        CurrentColorlessMana += amount;
        CurrentColoredMana[type] += amount;
    }

    private void AddColor(ManaType type, int amount)
    {
        CurrentColoredMana[type] += amount;
        TotalColoredMana[type] += amount;
    }

    private void AddTemporaryColor(ManaType type, int amount)
    {
        CurrentColoredMana[type] += amount;
    }

    public void ResetMana()
    {
        //Reset the total mana
        CurrentColorlessMana = TotalColorlessMana;
        //init the mana by type and temp mana by type dictionaries
        foreach (var manaType in Mana.GetManaTypes())
        {
            CurrentColoredMana[manaType] = TotalColoredMana[manaType];
        }
    }

}

public enum ManaType
{
    White,
    Blue,
    Black,
    Red,
    Green,
    Any //is a placeholder for mana of any color.
}