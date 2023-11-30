using System.Collections.Generic;
using System.Linq;


public class CreateUnitTokenEffect : CreateTokenEffect<UnitCardData>
{
    public CreateUnitTokenEffect() : base()
    {
        
    }

    public CreateUnitTokenEffect(UnitCardData cardData) : base(cardData)
    {

    }

    public override string RulesText
    {
        get
        {
            string amountTxt = AmountOfTokens == 1 ? "a" : AmountOfTokens.ToString();
            string tokenTxt = AmountOfTokens > 1 ? "tokens" : "token";
            if (TokenData.RulesText.IsEmpty())
            {
                return $"Create {amountTxt} {TokenData.Power}/{TokenData.Toughness} {TokenData.Name} unit {tokenTxt}";
            }

            return $"Create {amountTxt} {TokenData.Power}/{TokenData.Toughness} {TokenData.Name} unit {tokenTxt} with {TokenData.RulesText}";
        }
    }

    protected override void AddTokenToGame(CardGame cardGame, Player player)
    {
        var emptyLane = player.GetEmptyLanes().FirstOrDefault();
        if (emptyLane != null)
        {
            cardGame.AddCardToGame(player, TokenData, emptyLane);
        }
    }
}


public class CreateItemTokenEffect : CreateTokenEffect<ItemCardData>
{
    public CreateItemTokenEffect() : base()
    {

    }

    public CreateItemTokenEffect(ItemCardData cardData) : base(cardData)
    {

    }

    public override string RulesText
    {
        get
        {
            string amountTxt = AmountOfTokens == 1 ? "an" : AmountOfTokens.ToString();
            string tokenTxt = AmountOfTokens > 1 ? "tokens" : "token";
            return $"Create {amountTxt} item {tokenTxt} with {TokenData.RulesText}";
        }
    }

    protected override void AddTokenToGame(CardGame cardGame, Player player)
    {
        cardGame.AddCardToGame(player, TokenData, player.Items);
    }
}



public abstract class CreateTokenEffect<T> : Effect where T : BaseCardData
{
    public T TokenData { get; set; }
    public int AmountOfTokens { get; set; } = 1;

    protected CreateTokenEffect()
    {
        TargetInfo = TargetInfo.PlayerSelf();
    }

    protected CreateTokenEffect(T cardData)
    {
        TargetInfo = TargetInfo.PlayerSelf();
        TokenData = cardData;
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        for (var i=0; i<AmountOfTokens; i++)
        {
            AddTokenToGame(cardGame, entitiesToApply[0] as Player);
        }
    }

    protected abstract void AddTokenToGame(CardGame cardGame, Player player);
}



