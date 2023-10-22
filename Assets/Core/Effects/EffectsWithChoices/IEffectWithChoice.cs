using System.Collections.Generic;
using System.Linq;

public interface IEffectWithChoice
{
    List<CardInstance> GetValidChoices(CardGame cardGame, Player player);

    bool IsValid(CardGame cardGame, Player player);
    void ChoiceSetup(CardGame cardGame, Player player, CardInstance source);
    void OnChoicesSelected(CardGame cardGame, Player player, List<CardGameEntity> choices);
    string ChoiceMessage { get; }
    int NumberOfChoices { get; set; }
    /// <summary>
    /// Can be used to temporarily store choices in case there is choice specific logic needed.
    /// For example, in Telling Time, the Choice message would change depend on the choices selected.
    /// </summary>
    List<CardInstance> Choices { get; }
}

public abstract class EffectWithChoice : Effect, IEffectWithChoice
{
    public virtual string ChoiceMessage { get; private set; }

    public virtual int NumberOfChoices { get; set; }

    public List<CardInstance> Choices { get; set; } = new List<CardInstance>();

    public abstract void ChoiceSetup(CardGame cardGame, Player player, CardInstance source);
    public abstract List<CardInstance> GetValidChoices(CardGame cardGame, Player player);

    /// <summary>
    /// By default checks if the choices made match the valid choices available.
    /// Note more complex choice cards may require different validation, so this should be overrided in that case.
    /// For an example look at a Multi Choice Effect like the TellingTimeEffect
    /// </summary>
    /// <param name="cardGame"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public virtual bool IsValid(CardGame cardGame, Player player)
    {
        //This validation works with single select choice cards, but not with multi select choice cards.
        //We only have 1 multi select choice card right now in "Telling Time".
        //But if that changes in the future, and the validation looks the same we could probably set some sort of flag
        //on the ChoiceEffect to tell it to use Single CHoice or Multi Choice validation.
        var validChoices = GetValidChoices(cardGame, player).Select(c => c.EntityId).ToList();
        var choicesAsInts = Choices.Select(c => c.EntityId).ToList();
        var isOK = !choicesAsInts.Except(validChoices).Any();
        return isOK;
    }

    public abstract void OnChoicesSelected(CardGame cardGame, Player player, List<CardGameEntity> choices);

}

