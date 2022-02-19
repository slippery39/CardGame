using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public abstract class BaseCardData
{
    public string Name { get; set; }
    public string RulesText { get; set; }
    public string ManaCost { get; set; }
    public abstract string CardType { get; }
    public string ArtPath { get; set; }
    public List<CardAbility> Abilities { get; set; }
    public abstract BaseCardData Clone();
}

[System.Serializable]
public class UnitCardData : BaseCardData
{
    public int Power { get; set; }
    public int Toughness { get; set; }
    public override string CardType => "Unit";

    public UnitCardData()
    {
        Abilities = new List<CardAbility>();
    }

    public override BaseCardData Clone()
    {
        return new UnitCardData()
        {
            Name = Name,
            RulesText = RulesText,
            ManaCost = ManaCost,
            ArtPath = ArtPath,
            Power = Power,
            Toughness = Toughness,
            Abilities = Abilities.ToList() //todo - potential deep clone.
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
            ArtPath = "CardArt/StormCrow",
            Abilities = new List<CardAbility>()
            {
                new FlyingAbility()
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Goblin Raider",
            RulesText = "Goblin Raider can't block",
            ManaCost = "2",
            Power = 2,
            Toughness = 2,
            ArtPath = "CardArt/GoblinRaider",
            Abilities = new List<CardAbility>()
            {
                new CantBlockAbility()
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Sunstriker",
            RulesText = "Lifelink",
            ManaCost = "2",
            Power = 2,
            Toughness = 2,
            ArtPath = "CardArt/Sunstriker",
            Abilities = new List<CardAbility>()
            {
                new LifelinkAbility()
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Typhoid Rats",
            RulesText = "Deathtouch",
            ManaCost = "1",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/TyphoidRats",
            Abilities = new List<CardAbility>()
            {
               new DeathtouchAbility()
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Hexplate Golem",
            RulesText = "",
            ManaCost = "7",
            Power = 5,
            Toughness = 7,
            ArtPath = "CardArt/HexplateGolem"
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
