using System;
using System.Collections.Generic;

public interface ISpellCastingSystem
{
    public void CastSpell(Player player, CardInstance spellCard, List<CardGameEntity> targets, ResolveInfo resolveInfo);
    public void CastSpell(Player player, CardInstance spellCard, CardGameEntity target,ResolveInfo resolveInfo);
    public void CastSpell(Player player, CardInstance spellCard,ResolveInfo resolveInfo);
}

public class DefaultSpellCastingSystem : ISpellCastingSystem
{
    private CardGame cardGame;

    public DefaultSpellCastingSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void CastSpell(Player player, CardInstance spellCard, CardGameEntity target,ResolveInfo resolveInfo)
    {
        CastSpell(player, spellCard, new List<CardGameEntity> { target },resolveInfo);
    }

    //Needs information on how the spell was cast (i.e. which zone it came from and stuff).
    public void CastSpell(Player player, CardInstance spellCard, List<CardGameEntity> targets,ResolveInfo resolveInfo)
    {
        var effects = ((SpellCardData)spellCard.CurrentCardData).Effects;

        if (targets.Count > 1)
        {
            throw new Exception("Multiple Targets not supported for casting spells yet");
        }

        cardGame.EffectsProcessor.ApplyEffects(player, spellCard, effects, targets);

        if (resolveInfo.SourceZone.ZoneType == ZoneType.Discard)
        {
            cardGame.ZoneChangeSystem.MoveToZone(spellCard, cardGame.GetOwnerOfCard(spellCard).Exile);
        } 
        else
        {
            cardGame.ZoneChangeSystem.MoveToZone(spellCard, cardGame.GetOwnerOfCard(spellCard).DiscardPile);
        }
    }
    public void CastSpell(Player player, CardInstance spellCard, ResolveInfo resolveInfo)
    {
        if (cardGame.TargetSystem.SpellNeedsTargets(player, spellCard))
        {
            throw new Exception("Error: The spell that is being cast needs targets but is calling the CastSpell method without targets... make sure it is using the correct overloaded CastSpell method");
        }
        CastSpell(player, spellCard, new List<CardGameEntity>(),resolveInfo);
    }
}
