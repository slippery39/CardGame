using System;
using System.Collections.Generic;
using System.Linq;

public class ManaPool
{

    //TODO - need to be able to differentiate between spent mana and saved mana.
    //i.e. we should have a CurrentManaByType and a CurrentTotalMana
    //the TotalMana and ManaByType properties will do the overall (i.e. mana we would have if we didn't spend any yet)
    //the CurrentManaByType and CurrentTotalMana will give us the mana we have left after spending.
    //To Reset we just set CurrentManaByType to the same values as ManaByType.
    //If we want temp mana we can just add to the CurrentManaByType without addinf to the ManaByType.

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
        ManaByType = new Dictionary<ManaType, int>();
        CurrentManaByType = new Dictionary<ManaType, int>();

        //init the mana by type and temp mana by type dictionaries
        foreach (var manaType in ManaPool.GetManaTypes())
        {
            ManaByType.Add(manaType, 0);
            CurrentManaByType.Add(manaType, 0);
        }
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
        ManaByType[type] += amount;
    }

    public void ResetMana()
    {
        //init the mana by type and temp mana by type dictionaries
        foreach (var manaType in ManaPool.GetManaTypes())
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