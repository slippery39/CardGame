public class LessThanManaFilter : IManaFilter
{
    public int Amount { get; set; }

    public string RulesTextString(bool plural = false)
    {
        return $"less than {Amount} colorless mana cost";
    }
    public bool Check(CardInstance cardToCheck)
    {
        var mana = new Mana(cardToCheck.ManaCost);
        return mana.ColorlessMana < Amount;
    }
}




