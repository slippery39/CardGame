public class GetRandomCardFromDeckEffect : Effect
{

    public override string RulesText
    {
        get
        {
            var str = "draw a random #cardType# from your deck";

            if (Filter?.CreatureType != null)
            {
                return str.Replace("#cardType#", Filter.CreatureType);
            }
            else
            {
                return str.Replace("#cardType#", "card");
            }
        }
    }
    public override TargetType TargetType { get; set; } = TargetType.None;

    public CardFilter Filter { get; set; }
}


