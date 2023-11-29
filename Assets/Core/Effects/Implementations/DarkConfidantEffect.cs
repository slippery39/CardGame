using System;
using System.Collections.Generic;

public class DarkConfidantEffect : Effect
{
    public override string RulesText => $"Draw a card and lose life equal to its mana cost";

    public DarkConfidantEffect()
    {
        TargetInfo = TargetInfo.PlayerSelf();
    }

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        foreach (var entity in entitiesToApply)
        {
            if (!(entity is Player))
            {
                throw new Exception("Error : only players can be effected with the dark confidant effect");
            }
            var cardDrawn = cardGame.CardDrawSystem.DrawCard(player);
            cardGame.DamageSystem.DealDamage(source, player, new Mana(cardDrawn.ManaCost).ColorlessMana);
            cardGame.Log($@"Dark confidant effect : Drawn a card and you have lost {new Mana(cardDrawn.ManaCost).ColorlessMana} life.");
        }
    }
}


