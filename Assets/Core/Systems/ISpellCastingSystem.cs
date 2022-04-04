using System;
using System.Collections.Generic;

public interface ISpellCastingSystem
{
    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard, List<CardGameEntity> targets);
    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard, CardGameEntity target);
    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard);
}

public class DefaultSpellCastingSystem : ISpellCastingSystem
{
    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard, CardGameEntity target)
    {
        CastSpell(cardGame, player, spellCard, new List<CardGameEntity> { target });
    }

    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard, List<CardGameEntity> targets)
    {
        //TODO - Handle mana costs better
        cardGame.ManaSystem.SpendMana(cardGame, player, Convert.ToInt32(spellCard.ManaCost));

        var effects = ((SpellCardData)spellCard.CurrentCardData).Effects;

        if (targets.Count > 1)
        {
            throw new Exception("Multiple Targets not supported for casting spells yet");
        }

        cardGame.EffectsProcessor.ApplyEffects(cardGame, player, spellCard, effects, targets);

        cardGame.ZoneChangeSystem.MoveToZone(cardGame, spellCard, cardGame.GetOwnerOfCard(spellCard).DiscardPile);
    }
    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard)
    {
        if (cardGame.TargetSystem.SpellNeedsTargets(cardGame, player, spellCard))
        {
            throw new Exception("Error: The spell that is being cast needs targets but is calling the CastSpell method without targets... make sure it is using the correct overloaded CastSpell method");
        }
        CastSpell(cardGame, player, spellCard, new List<CardGameEntity>());
    }
}
