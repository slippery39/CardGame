public class CreateTokenEffect<T> : Effect where T : BaseCardData
{
    //TODO - change the rules text to take into account the amount of tokens created.
    public override string RulesText
    {
        get
        {
            if (TokenData is UnitCardData unitData)
            {
                return $"Create a {unitData.Power}/{unitData.Toughness} {unitData.Name} unit token with {unitData.RulesText}";
            }
            else if (TokenData is ItemCardData itemData)
            {
                return $"Create an item token with {itemData.RulesText}";
            }
            else
            {
                return "ERROR : Need to specify proper rulex text for this type of token";
            }
        }
    }
    public T TokenData { get; set; }
    public override TargetType TargetType { get; set; } = TargetType.OpenLane;
    public int AmountOfTokens { get; set; } = 1;

    public CreateTokenEffect()
    {

    }

    public CreateTokenEffect(T cardData)
    {
        TokenData = cardData;
    }
}


