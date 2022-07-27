
//TemporaryInPlayEffects
//  -could just have a "remove at end of turn flag"
//

//Modifications - Card Instances can now have "modifications" which will allow us to add temporary or permanent modifications to CardInstances.

using System;

public class Modification
{
    public bool OneTurnOnly { get; set; } = true;
    public StaticInfo StaticInfo { get; set; } = null;
}

public class StaticInfo
{
    public CardAbility SourceAbility { get; set; }
    public CardInstance SourceCard { get; set; }
}

public interface IModifyPower
{
    int ModifyPower(CardGame cardGame, CardInstance card, int originalPower);
}

public interface IModifyToughness
{
    int ModifyToughness(CardGame cardGame, CardInstance card, int originalToughness);
}


public class ModAddToPowerToughness : Modification, IModifyPower, IModifyToughness
{
    public int Power { get; set; }
    public int Toughness { get; set; }

    public int ModifyPower(CardGame cardGame, CardInstance card, int originalPower)
    {
        return originalPower + Power;
    }

    public int ModifyToughness(CardGame cardGame, CardInstance card, int originalToughness)
    {
        return originalToughness + Toughness;
    }
}

public class ModAddXToPowerToughness : Modification, IModifyPower, IModifyToughness
{

    private Func<CardGame, CardInstance, int, int> _powerMod;
    private Func<CardGame, CardInstance, int, int> _toughnessMod;

    public ModAddXToPowerToughness(Func<CardGame, CardInstance, int, int> powerMod, Func<CardGame, CardInstance, int, int> toughnessMod)
    {
        this._powerMod = powerMod;
        this._toughnessMod = toughnessMod;
    }

    public int ModifyPower(CardGame cardGame, CardInstance card, int originalPower)
    {
        if (_powerMod == null)
        {
            return originalPower;
        }
        return _powerMod(cardGame, card, originalPower);
    }

    public int ModifyToughness(CardGame cardGame, CardInstance card, int originalToughness)
    {
        if (_toughnessMod == null)
        {
            return originalToughness;
        }
        return _toughnessMod(cardGame, card, originalToughness);
    }
}

public class ModSwitchPowerandToughness : Modification, IModifyPower, IModifyToughness
{
    //We need to be able to calculate power and toughness a different way (an internal way)
    public int ModifyPower(CardGame cardGame, CardInstance card, int originalPower)
    {
        return card.CalculateToughness(false);
    }

    public int ModifyToughness(CardGame cardGame, CardInstance card, int originalToughness)
    {
        return card.CalculatePower(false);
    }
}


//ContinuousAbilities
//-another source is giving the unit the ability.