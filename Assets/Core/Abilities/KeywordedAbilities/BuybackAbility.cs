
/*
 * CastModifiers should be its own special ability type since they only matter when the card is being cast and not any time otherwise.
 * 
 */


//Step 1 -> Allow us to play the Spell, but with a higher mana cost
//Step 2 -> 

interface IModifyZoneOnResolve
{
    public IZone ModifyZoneOnResolve(CardGame cardGame, IZone zoneTo, CardInstance spell);
}

public class BuybackAbility : CardAbility, ICastModifier, IModifyZoneOnResolve
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
    }

    public IZone ModifyZoneOnResolve(CardGame cardGame, IZone zoneTo, CardInstance spell)
    {
        return cardGame.GetOwnerOfCard(spell).Hand;
    }
}


