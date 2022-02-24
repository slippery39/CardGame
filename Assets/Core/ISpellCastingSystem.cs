using System.Collections.Generic;
using System.Linq;

public interface ISpellCastingSystem
{
    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard);
    //public List<CardInstance> GetValidTargets(Player player, CardInstance spellCard);
}

public class DefaultSpellCastingSystem : ISpellCastingSystem
{
    public void CastSpell(CardGame cardGame, Player player, CardInstance spellCard)
    {
        //TODO - check if spell card is a spell card.
        foreach(var ab in spellCard.Abilities)
        {
            if (ab is DamageAbility)
            {
                //TODO - we need TargetInfo.
                //Need to able to process our targets.

                //For now let target everything.
                //TODO - this is only going after player 2 right now.
                var targetInfo = new AbilityTargetInfo()
                {
                    Targets = cardGame.Player2.Lanes.Where(lane => lane.IsEmpty() == false).Select(lane => lane.UnitInLane).ToList()
                };

                foreach (var target in targetInfo.Targets) {
                    cardGame.DamageSystem.DealAbilityDamage(cardGame, (DamageAbility)ab, spellCard, target);
                }
            }
            //Figure out how to resolve abilities.
        }
    }

    public List<CardInstance> GetValidTargets(Player player, SpellCardData spellCard)
    {
        throw new System.NotImplementedException();
    }
}

public class AbilityTargetInfo
{
    public List<CardInstance> Targets;
}