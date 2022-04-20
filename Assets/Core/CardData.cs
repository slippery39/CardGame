using System.Collections;
using System.Collections.Generic;
using System.Linq;


public abstract class BaseCardData
{
    public string Name { get; set; }
    public virtual string RulesText { get { return string.Join("\r\n", Abilities.Select(ab => ab.RulesText)); } }
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

    public List<Effect> Effects = new List<Effect>();
    public override string RulesText
    {
        get
        {
            var abilitiesText = string.Join("\r\n", Abilities.Select(ab => ab.RulesText));
            var effectsText = string.Join("\r\n", Effects.Select(ef => ef.RulesText));
            return abilitiesText + effectsText;
        }
    }

    public SpellCardData() : base()
    {

    }

    public override BaseCardData Clone()
    {
        return new SpellCardData()
        {
            Name = Name,
            ManaCost = ManaCost,
            ArtPath = ArtPath,
            Abilities = Abilities.ToList(), //todo - potential deep clone.
            Effects = Effects.ToList()
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
            Effects = new List<Effect>()
            {
                new DamageEffect()
                {
                    TargetType = TargetType.TargetUnitsOrPlayers,
                    Amount = 3
                }
            }
        });

        _cards.Add(new SpellCardData()
        {
            Name = "Lightning Helix",
            ManaCost = "2",
            ArtPath = "CardArt/LightningHelix",
            Effects = new List<Effect>()
            {
                new DamageEffect()
                {
                    Amount = 3
                },
                new LifeGainEffect()
                {
                    Amount = 3
                }
            }
        });

        _cards.Add(new SpellCardData()
        {
            Name = "Giant Growth",
            ManaCost = "1",
            ArtPath = "CardArt/GiantGrowth",
            Effects = new List<Effect>()
            {
                new PumpUnitEffect()
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
            Effects = new List<Effect>()
            {
                new DrawCardEffect()
                {
                    Amount = 3
                }
            }
        });

        _cards.Add(new SpellCardData()
        {
            Name = "Black Lotus",
            ManaCost = "0",
            ArtPath = "CardArt/BlackLotus",
            Effects = new List<Effect>()
            {
                new AddTempManaEffect()
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
            Name = "Kalonian Behemoth",
            ManaCost = "7",
            Power = 9,
            Toughness = 9,
            ArtPath = "CardArt/KalonianBehemoth",
            Abilities = new List<CardAbility>()
               {
                   new ShroudAbility()
               }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Invisible Stalker",
            ManaCost = "2",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/InvisibleStalker",
            Abilities = new List<CardAbility>()
               {
                   new HexproofAbility(),
                   new UnblockableAbility()
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Rorix, Bladewing",
            ManaCost = "6",
            Power = 6,
            Toughness = 5,
            ArtPath = "CardArt/RorixBladewing",
            Abilities = new List<CardAbility>()
            {
                new FlyingAbility(),
                new HasteAbility()
            }
        });


        /*
        //Cards To Create for Activated Abilities

        _cards.Add(new UnitCardData()
        {
            Name = "Llanowar Elves",
            ManaCost = "1",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/LlanowarElves",
            Abilities = new List<CardAbility>()
            {
                new ActivatedAbility("0",new TemporaryManaEffect{Amount=1})
            }
        }); 

        _cards.Add(new UnitCardData()
        {
            Name = "Prodigal Sorcerer",
            ManaCost = "3",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/ProdigalSorcerer",
            Abilities = new List<CardAbility>()
            {
                new ActivatedAbility("0", new DamageEffect{Amount=1})
            }
        });

        //TODO - think of more cards with activated abilities.
        */





        //Cards To Create for Triggered Abilities

        _cards.Add(new UnitCardData()
        {
            Name = "Elvish Visionary",
            ManaCost = "2",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/ElvishVisionary",
            Abilities = new List<CardAbility>()
            {
                new TriggeredAbility(
                    TriggerType.SelfEntersPlay,
                    new DrawCardEffect()
                    {
                        Amount = 1
                    }
                )
            }
        });

        //Mulldrifter


        _cards.Add(new UnitCardData()
        {
            Name = "Mulldrifter",
            ManaCost = "5",
            Power = 2,
            Toughness = 2,
            ArtPath = "CardArt/Mulldrifter",
            Abilities = new List<CardAbility>()
            {
                new TriggeredAbility(
                    TriggerType.SelfEntersPlay,
                    new DrawCardEffect()
                    {
                        Amount = 2
                    }
                    ),
                new FlyingAbility()
            }
        });

        //Dark Confidant
        _cards.Add(new UnitCardData()
        {
            Name = "Dark Confidant",
            ManaCost = "2",
            Power = 2,
            Toughness = 1,
            ArtPath = "CardArt/DarkConfidant",
            Abilities = new List<CardAbility>()
            {
                new TriggeredAbility(
                    TriggerType.AtTurnStart,
                    new DarkConfidantEffect()
                )
            }
        });

        //Colossal Dreadmaw
        _cards.Add(new UnitCardData()
        {
            Name = "Colossal Dreadmaw",
            ManaCost = "6",
            Power = 6,
            Toughness = 6,
            ArtPath = "CardArt/ColossalDreadmaw",
            Abilities = new List<CardAbility>()
            {
                new TrampleAbility()
            }
        });

        //Ball Lightning
        _cards.Add(new UnitCardData()
        {
            Name = "Ball Lightning",
            ManaCost = "3",
            Power = 6,
            Toughness = 1,
            ArtPath = "CardArt/Ball Lightning",
            Abilities = new List<CardAbility>()
            {
                new HasteAbility(),
                new TrampleAbility(),
                new TriggeredAbility(TriggerType.AtTurnEnd, new SacrificeSelfEffect())
            }
        });


        //Delver of Secrets
        _cards.Add(new UnitCardData()
        {
            Name = "Delver of Secrets",
            ManaCost = "1",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/DelverOfSecrets",
            Abilities = new List<CardAbility>()
            {
                new TriggeredAbility
                (
                    TriggerType.AtTurnStart,
                    new TransformEffect
                    {
                        TransformData = new UnitCardData
                        {
                            Name = "Insectile Aberration",
                            ManaCost = "1",
                            Power = 3,
                            Toughness = 2,
                            ArtPath = "CardArt/InsectileAberration",
                            Abilities = new List<CardAbility>()
                            {
                                new FlyingAbility()
                            }
                        }
                    }
                )
            }
        });

        _cards.Add(new SpellCardData()
        {
            Name = "Murder",
            ManaCost = "3",
            ArtPath = "CardArt/Murder",
            Effects = new List<Effect>()
            {
                new DestroyEffect()
                {
                    TargetType = TargetType.TargetUnits
                }
            }
        });

        _cards.Add(new SpellCardData()
        {
            Name = "Day of Judgment",
            ManaCost = "4",
            ArtPath = "CardArt/DayofJudgment",
            Effects = new List<Effect>()
            {
                new DestroyEffect()
                {
                   TargetType = TargetType.AllUnits
                }
            }
        });


        _cards.Add(new SpellCardData()
        {
            Name = "Pyroclasm",
            ManaCost = "2",
            ArtPath = "CardArt/Pyroclasm",
            Effects = new List<Effect>()
            {
                new DamageEffect()
                {
                    Amount = 2,
                    TargetType = TargetType.AllUnits
                }
            }
        });

        _cards.Add(new SpellCardData()
        {
            Name = "Zealous Persecution",
            ManaCost = "2",
            ArtPath = "CardArt/ZealousPersecution",
            Effects = new List<Effect>
            {
                new PumpUnitEffect()
                {
                    Power = -1,
                    Toughness = -1,
                    TargetType = TargetType.OpponentUnits
                },
                new PumpUnitEffect()
                {
                    Power = 1,
                    Toughness = 1,
                    TargetType = TargetType.OurUnits
                }
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Benalish Marshall",
            ManaCost = "3",
            Power = 3,
            Toughness = 3,
            ArtPath = "CardArt/BenalishMarshall",
            Abilities = new List<CardAbility>
            {
                new StaticAbility
                {
                    EffectType = StaticAbilityType.OtherCreaturesYouControl,
                    Effects = new List<StaticAbilityEffect>
                    {
                        new StaticPumpEffect
                        {
                            Power = 1,
                            Toughness = 1
                        }
                    }
                }
            }
        });

        /*
        _cards.Add(new UnitCardData()
        {
            Name = "Doomed Traveler",
            ManaCost = "1",
            ArtPath = "CardArt/Doomed Traveler",
            Power = 1,
            Toughness = 1,
            Abilities = new List<CardAbility>()
            {
                new TriggeredAbility(TriggerType.SelfDies)
            }
        });
        */

        /*
                //Snapcaster Mage
                _cards.Add(new UnitCardData()
                {
                    Name = "Snapcaster Mage",
                    ManaCost = "2",
                    Power = 2,
                    Toughness = 1,
                    ArtPath = "CardArt/SnapcasterMage",
                    Abilities = new List<CardAbility>()
                    {
                        new TriggeredAbility
                        (
                            TriggerType.SelfEntersPlay,
                            new CastSpellFromDiscardEffect()
                        )
                    }
                });
                */


        /*
        //Geist of Saint Traft
        _cards.Add(new UnitCardData()
        {
            Name = "Geist of Saint Traft",
            ManaCost = "3",
            Power = 2,
            Toughness = 2,
            ArtPath = "CardArt/GeistOfSaintTraft",
            Abilities = new List<CardAbility>()
                {
                    new HexproofAbility(),
                    new TriggeredAbility(TriggerType.SelfAttacks,
                    new CreateTokenEffect( new UnitCardData(){
                        Name = "Angel",
                        ManaCost = "0",
                        Power = 4,
                        Toughness = 4,
                        ArtPath = "CardArt/Tokens/Angel",
                        Abilities = new List<CardAbility>()
                        {
                            new FlyingAbility(),
                            new HasteAbility(), //haste is needed to make sure it attacks.
                            new TriggeredAbility(TriggerType.AtTurnEnd, new SacrificeSelfEffect())
                        }
                    }, TargetType.OpenLane
                    )
                    )
                }
        });;
        */


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
