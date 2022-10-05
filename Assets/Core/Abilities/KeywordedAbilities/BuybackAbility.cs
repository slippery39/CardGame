
/*
 * CastModifiers should be its own special ability type.
 * 
 */


//Step 1 -> Allow us to play the Spell, but with a higher mana cost
//Step 2 -> 
public class BuybackAbility : CardAbility, ICastModifier
{
    public override string RulesText => $"Buyback : {BuybackCost}";
    public string BuybackCost { get; set; }

    public BuybackAbility(string manaCost)
    {
        BuybackCost = manaCost;
    }

    public string GetCost(CardInstance sourceCard)
    {
        //TODO - Easy way to add Mana Costs together.
        return BuybackCost;
    }

    public void OnResolve(CardGame cardGame, CardInstance source)
    {
        //Return it to hand instead of putting it into the graveyard.
        //This needs to use a replacement effect.
        cardGame.Log("Buyback Ability has resolved");
    }
}


