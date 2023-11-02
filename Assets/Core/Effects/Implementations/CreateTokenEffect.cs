using System.Collections.Generic;
using System.Linq;

public class CreateTokenEffect<T> : Effect where T : BaseCardData
{
    //TODO - change the rules text to take into account the amount of tokens created.
    public override string RulesText
    {
        get
        {
            if (TokenData is UnitCardData unitData)
            {
                if (unitData.RulesText.IsEmpty())
                {
                    return $"Create a {unitData.Power}/{unitData.Toughness} {unitData.Name} unit token";
                }

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
    public override TargetType TargetType { get; set; } = TargetType.PlayerSelf;
    public int AmountOfTokens { get; set; } = 1;

    public CreateTokenEffect()
    {

    }

    public CreateTokenEffect(T cardData)
    {
        TokenData = cardData;
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        if (this is CreateTokenEffect<UnitCardData>)
        {
            //put in open lanes. 
            var playerForTokens = entitiesToApply[0] as Player;

            for (var i = 0; i < AmountOfTokens; i++)
            {
                var emptyLane = playerForTokens.GetEmptyLanes().FirstOrDefault();
                if (emptyLane != null)
                {
                    cardGame.AddCardToGame(playerForTokens, TokenData, emptyLane);
                }
            }
        }
        else if (this is CreateTokenEffect<ItemCardData>)
        {
            var playerForTokens = entitiesToApply[0] as Player;
            //This should also use EntitiesToApply
            //put in open lanes.
            for (var i = 0; i < AmountOfTokens; i++)
            {
                cardGame.AddCardToGame(playerForTokens, TokenData, playerForTokens.Items);
            }
        }

    }
}



