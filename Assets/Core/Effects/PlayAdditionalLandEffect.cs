using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class PlayAdditionalLandEffect : Effect
{
    public int Amount { get; set; } = 1;
    public override string RulesText => "You may play play an additional land";
    public bool OneTurnOnly { get; set; } = true;

    public bool IsStatic = false;

    private Modification AdditionalLandModification() => new ManaPerTurnModification()
    {
        Amount = Amount,
        OneTurnOnly = true
    };
    public override void Apply(CardGame cardGame, Player player, CardInstance source, List<CardGameEntity> entitiesToApply)
    {
        var modification = AdditionalLandModification();

        /*
        if (IsStatic)
        {
            var sourceAbility = source.Abilities.Where(ab=>ab.Effects
            modification.StaticInfo = new StaticInfo
            {

            };
        }
        */

        cardGame.PlayerAbilitySystem.GiveModification(player, AdditionalLandModification());
    }
}

public interface IModifyManaPerTurn
{
    int ModifyManaPerTurn(CardGame cardGame, Player player, int originalManaPerTurn);
}

public class ManaPerTurnModification : Modification, IModifyManaPerTurn
{
    public int Amount { get; set; }
    public int ModifyManaPerTurn(CardGame cardGame, Player player, int originalManaPerTurn)
    {
        return originalManaPerTurn + Amount;
    }
}