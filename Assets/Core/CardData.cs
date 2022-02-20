using System.Collections;
using System.Collections.Generic;
using System.Linq;


public abstract class BaseCardData
{
    public string Name { get; set; }
    public string RulesText { get; set; }
    public string ManaCost { get; set; }
    public abstract string CardType { get; }
    public string ArtPath { get; set; }
    public List<CardAbility> Abilities { get; set; }

    //Unsafe here, basing this method off of Unity's own GetComponent call.
    public List<T> GetAbilities<T>()
    {
        var foundAbilities = Abilities.Where(ab => ab is T).Cast<T>().ToList(); //this returns card abilities.
        return foundAbilities;
    }
    public abstract BaseCardData Clone();
}


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
            Name = "Ajani's Sunstriker",
            RulesText = "Lifelink",
            ManaCost = "2",
            Power = 2,
            Toughness = 2,
            ArtPath = "CardArt/AjanisSunstriker",
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

        _cards.Add(new UnitCardData()
        {
            Name = "Vampire Nighthawk",
            RulesText = "Flying, Deathtouch",
            ManaCost = "3",
            Power = 2,
            Toughness = 3,
            ArtPath = "CardArt/VampireNighthawk",
            Abilities = new List<CardAbility>()
            {
                new FlyingAbility(),
                new DeathtouchAbility(),
                new LifelinkAbility()
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name="Inkfathom Infiltrator",
            RulesText = "Unblockable, Can't Block",
            ManaCost = "2",
            Power = 2,
            Toughness = 1,
            ArtPath = "CardArt/InkfathomInfiltrator",
            Abilities = new List<CardAbility>()
            {
                new UnblockableAbility(),
                new CantBlockAbility()
            }
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
