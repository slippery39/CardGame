public class DrawCardEffect : Effect
{
    public override string RulesText
    {
        get
        {

            if (Amount == 1)
            {
                return "Draw a card";
            }
            else
            {
                return $"Draw {Amount} Cards";
            }
        }
    }

    public int Amount { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.Self;
}


