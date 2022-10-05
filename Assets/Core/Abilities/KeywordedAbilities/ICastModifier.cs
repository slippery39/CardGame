
/// <summary>
/// Cast Modifiers are a way to cast a spell or creature with additional effects included.
/// Examples : Buyback, Kicker
/// A card can have 0 to many cast modifiers.
/// </summary>
public interface ICastModifier
{
    public string GetCost(CardInstance sourceCard);
    public void OnResolve(CardGame cardGame, CardInstance source);
}


