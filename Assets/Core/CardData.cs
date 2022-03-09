using System.Collections;
using System.Collections.Generic;
using System.Linq;


public abstract class BaseCardData
{
    public string Name { get; set; }
    public string RulesText { get { return string.Join("\r\n", Abilities.Select(ab => ab.RulesText)); } }
    public string ManaCost { get; set; }
    public abstract string CardType { get; }
    public string ArtPath { get; set; }
    public List<CardAbility> Abilities { get; set; }
    public List<T> GetAbilities<T>()
    {
        //We are sorting so the highest priority items are at the bottom of the list.
        //This ensures they execute last, which in the case of ModAbilities, would ensure their modification,
        //takes preference over every other one.        
        //If we ever need certain abilities to fire first for whatever reason then we should give it a negative priority.
        var sortedAbilities = Abilities.ToList();
        sortedAbilities.Sort((a, b) => a.Priority - b.Priority);
        var foundAbilities = sortedAbilities.Where(ab => ab is T).Cast<T>().ToList(); //this returns card abilities.
        return foundAbilities;
    }
    public abstract BaseCardData Clone();

    public BaseCardData()
    {
        Abilities = new List<CardAbility>();
    }
}


public class UnitCardData : BaseCardData
{
    public int Power { get; set; }
    public int Toughness { get; set; }
    public override string CardType => "Unit";

    public UnitCardData() : base()
    {
    }

    public override BaseCardData Clone()
    {
        return new UnitCardData()
        {
            Name = Name,
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

    public SpellCardData(): base()
    {

    }

    public override BaseCardData Clone()
    {
        return new SpellCardData()
        {
            Name = Name,
            ManaCost = ManaCost,
            ArtPath = ArtPath,
            Abilities = Abilities.ToList() //todo - potential deep clone.
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
            ManaCost = "2",
            Power = 2,
            Toughness = 2,
            ArtPath = "CardArt/GrizzlyBear"
        });

        _cards.Add(new SpellCardData()
        {
            Name = "Lightning Bolt",
            ManaCost = "1",
            ArtPath = "CardArt/LightningBolt",
            Abilities = new List<CardAbility>()
            {
                new DamageAbility()
                {
                    Amount = 3
                }
            }
        });

        _cards.Add(new SpellCardData()
        {
            Name = "Lightning Helix",
            ManaCost = "2",
            ArtPath="CardArt/LightningHelix",
            Abilities = new List<CardAbility>()
            {
                new DamageAbility()
                {
                    Amount = 3
                },
                new LifeGainAbility()
                {
                    Amount = 3
                }
            }
        });

        _cards.Add(new SpellCardData()
        {
            Name = "Giant Growth",
            ManaCost = "1",
            ArtPath = "CardArt/GiantGrwoth",
            Abilities = new List<CardAbility>()
            {
                new PumpUnitAbility()
                {
                    Power = 3,
                    Toughness = 3
                }
            }
        });
        
        _cards.Add(new SpellCardData()
        {
            Name = "Ancestral Recall",
            ManaCost = "1",
            ArtPath = "CardArt/AncestralRecall",
            Abilities = new List<CardAbility>()
            {
                new DrawCardAbility()
                {
                    Amount = 3
                }
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Storm Crow",
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
            ManaCost = "7",
            Power = 5,
            Toughness = 7,
            ArtPath = "CardArt/HexplateGolem"
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Vampire Nighthawk",
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
            Name = "Inkfathom Infiltrator",
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

        _cards.Add(new UnitCardData()
        {
            Name = "Unblockable Flying Dude",
            ManaCost = "5",
            Power = 4,
            Toughness = 5,
            Abilities = new List<CardAbility>()
            {
                new FlyingAbility(),
                new UnblockableAbility()
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
 