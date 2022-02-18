using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class BaseCardData
{
    public string Name { get; set; }
    public string RulesText { get; set; }
    public string ManaCost { get; set; }
    public abstract string CardType { get; }
    public string ArtPath { get; set; }
    public abstract BaseCardData Clone();
}

[System.Serializable]
public class UnitCardData : BaseCardData
{
    public int Power { get; set; }
    public int Toughness { get; set; }
    public override string CardType => "Unit";

    public override BaseCardData Clone()
    {
        return new UnitCardData()
        {
            Name = Name,
            RulesText = RulesText,
            ManaCost = ManaCost,
            ArtPath = ArtPath,
            Power = Power,
            Toughness = Toughness
        };
    }
}

[System.Serializable]
public class SpellCardData : BaseCardData
{
    public override string CardType => "Spell";

    public override BaseCardData Clone()
    {
        return new SpellCardData()
        {
            Name = Name,
            RulesText = RulesText,
            ManaCost = ManaCost,
            ArtPath = ArtPath,
        };
    }
}

public interface ICardDatabase
{
    List<BaseCardData> GetAll();
    BaseCardData GetCardData(string name);
}

public class CardDatabase : ICardDatabase
{
    private List<BaseCardData> _cards;
    public CardDatabase()
    {
        _cards = new List<BaseCardData>();

        //Create cards here for now.
        _cards.Add(new UnitCardData()
        {
            Name = "Grizzly Bear",
            RulesText = "",
            ManaCost = "2",
            Power = 2,
            Toughness = 2,
            ArtPath = "CardArt/GrizzlyBear"
        });

        _cards.Add(new SpellCardData()
        {
            Name = "Lightning Bolt",
            RulesText = "Deal 3 damage to target unit or player",
            ManaCost = "1",
            ArtPath = "CardArt/LightningBolt"
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Storm Crow",
            RulesText = "Flying",
            ManaCost = "2",
            Power = 1,
            Toughness = 2,
            ArtPath = "CardArt/StormCrow"
        });
    }

    public List<BaseCardData> GetAll()
    {
        return _cards;
    }

    public BaseCardData GetCardData(string name)
    {
        return _cards.Find(c => c.Name == name);
    }
}
