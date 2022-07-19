public class DiscardCardEffect : Effect
{
    public override string RulesText
    {
        get
        {
            if (Amount == 1)
            {
                return "Discard a card";
            }
            else
            {
                return $@"Discard {Amount} Cards";
            }
        }
    }
    public int Amount { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.Self;

}


