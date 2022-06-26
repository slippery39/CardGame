 
//TemporaryInPlayEffects
//  -could just have a "remove at end of turn flag"
//

//Modifications - Card Instances can now have "modifications" which will allow us to add temporary or permanent modifications to CardInstances.

public class Modification
{
    public bool OneTurnOnly { get; set; } = true;
}

public interface IModifyPower
{
    int ModifyPower(CardGame cardGame, CardInstance card, int originalPower);
}

public interface IModifyToughness
{
    int ModifyToughness(CardGame cardGame,CardInstance card, int originalToughness);
}


public class ModAddToPowerToughness : Modification, IModifyPower, IModifyToughness
{
    public int Power { get; set; }
    public int Toughness { get; set; }

    public int ModifyPower(CardGame cardGame, CardInstance card, int originalPower)
    {
        return originalPower + Power;
    }

    public int ModifyToughness (CardGame cardGame, CardInstance card, int originalToughness)
    {
        return originalToughness + Toughness;
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