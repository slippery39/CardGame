using System;
using System.Collections.Generic;

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

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            for (int i = 0; i < Amount; i++)
            {
                if (!(entity is Player))
                {
                    throw new Exception("Error : only players can draw cards");
                }
                cardGame.CardDrawSystem.DrawCard((Player)entity);
            }
        }
    }
}



