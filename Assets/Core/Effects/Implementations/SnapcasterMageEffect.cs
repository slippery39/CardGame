using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SnapcasterMageEffect : Effect
{
    public override string RulesText => "you may cast a spell from your graveyard this turn";

    private Modification CastZoneMod = new Modification();

    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        //Two ways we can implement this.
        //Give all spells in the graveyard Flashback,
        //Give the player some sort of "Emblem" that allows them to cast one spell from their graveyard.
        var modification = new SnapcasterMageModification()
        {
            OneTurnOnly = true
        };
        cardGame.PlayerAbilitySystem.GiveModification(player, modification);//snapcaster mage emblem)
    }
}



public interface IOnSpellCast
{
    void OnSpellCast(CardGame cardGame, CardInstance spellCast, IZone sourceZone);
}


public class SnapcasterMageModification : Modification, IModifyCastZones, IOnSpellCast
{
    public List<ZoneType> ModifyCastZones(CardGame cardGame, CardInstance card, List<ZoneType> originalCastZones)
    {
        if (card.IsOfType<SpellCardData>())
        {
            return originalCastZones.Union(new List<ZoneType> { ZoneType.Discard }).ToList();
        }
        else
        {
            return originalCastZones;
        }
    }

    public void OnSpellCast(CardGame cardGame, CardInstance spellCast, IZone sourceZone)
    {
        if (sourceZone.ZoneType == ZoneType.Discard)
        {
            cardGame.GetOwnerOfCard(spellCast).Modifications.Remove(this);
        }
    }
}



