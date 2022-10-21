using System.Collections.Generic;
using System.Linq;

public interface IDiscardSystem
{
    void Discard(Player player, CardInstance card);
    void Discard(Player player, List<CardInstance> cards);
}

public class DefaultDiscardSystem : CardGameSystem, IDiscardSystem
{
    public DefaultDiscardSystem(CardGame cardGame)
    {
        this.cardGame = cardGame;
    }
    public void Discard(Player player, CardInstance card)
    {
        if (card.GetAbilitiesAndComponents<MadnessAbility>().Any())
        {
            var madnessAbility = card.GetAbilitiesAndComponents<MadnessAbility>().First();
            if (cardGame.ManaSystem.CanPayManaCost(player,madnessAbility.ManaCost))
            {   
                //TODO - Spells with madness?
                cardGame.UnitSummoningSystem.SummonUnit(player, card, player.GetEmptyLanes().First().EntityId);
                return;
            }
        }

        cardGame.ZoneChangeSystem.MoveToZone(card, player.DiscardPile);
        //Hacky way to do madness

    }

    public void Discard(Player player, List<CardInstance> cards)
    {
        foreach(var card in cards)
        {
            Discard(player, card);
            
        }
    }
}