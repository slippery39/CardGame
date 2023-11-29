using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public interface IEffectsProcessor
{
    void ApplyEffect(Player player, CardInstance source, Effect effects, List<CardGameEntity> targets);
    void ApplyEffects(Player player, CardInstance source, List<Effect> effects, List<CardGameEntity> targets);
}


public class DefaultEffectsProcessor : CardGameSystem, IEffectsProcessor
{
    public DefaultEffectsProcessor(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void ApplyEffect(Player player, CardInstance source, Effect effect, List<CardGameEntity> targets)
    {
        if (source.Name == "Vapor Snag")
        {
            var debug = 0;
        }
        List<CardGameEntity> entitiesToEffect;
        if (!effect.NeedsTargets())
        {
            entitiesToEffect = cardGame.TargetSystem.GetEntitiesToApplyEffect(player, source, effect);
        }
        else
        {
            entitiesToEffect = targets;
        }

        effect.Apply(cardGame, player, source, entitiesToEffect);
    }
    public void ApplyEffects(Player player, CardInstance source, List<Effect> effects, List<CardGameEntity> targets)
    {
        foreach (var effect in effects)
        {
            ApplyEffect(player, source, effect, targets);
        }
    }
}
