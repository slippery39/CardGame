//These could really just be CardComponents, with the way they are working.

//These really should just be a modification of a "Counter" type.
public abstract class Counter
{
}


public class PlusOnePlusOneCounter : Counter, IModifyPower, IModifyToughness
{
    public int ModifyPower(CardGame cardGame, CardInstance card, int originalPower)
    {
        return originalPower + 1;
    }

    public int ModifyToughness(CardGame cardGame, CardInstance card, int originalToughness)
    {
        return originalToughness + 1;
    }
}