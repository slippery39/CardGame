using System.Collections;
using System.Collections.Generic;
using System.Linq;


public static class TokenHelper
{
    public static UnitCardData GoblinToken()
    {
        return new UnitCardData()
        {
            Name = "Goblin Token",
            ManaCost = "0",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/Goblin Token",
            CreatureType = "Goblin",
            Colors = new List<CardColor> { CardColor.Red }
        };
    }
}


public abstract class BaseCardData
{
    public string Name { get; set; }
    public virtual string RulesText { get { return string.Join("\r\n", Abilities.Select(ab => ab.RulesText)).Replace("#this#", Name); } }
    public string ManaCost { get; set; }
    public abstract string CardType { get; }
    public string Subtype { get; set; } = "";
    public List<CardColor> Colors { get; set; }
    public string ArtPath { get; set; }
    public List<CardAbility> Abilities { get; set; }
    public AdditionalCost AdditionalCost { get; set; }
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
        Colors = new List<CardColor>();
        Abilities = new List<CardAbility>();
    }
}


public class UnitCardData : BaseCardData
{
    public int Power { get; set; }
    public int Toughness { get; set; }
    public override string CardType => "Unit";
    public string CreatureType { get; set; } = "";

    public UnitCardData() : base()
    {
    }

    public override BaseCardData Clone()
    {
        Abilities = Abilities.Select(ab => ab.Clone()).ToList();
        return new UnitCardData()
        {
            Name = Name,
            ManaCost = ManaCost,
            ArtPath = ArtPath,
            Power = Power,
            Toughness = Toughness,
            Colors = Colors,
            Abilities = Abilities.ToList(), //todo - potential deep clone.
            CreatureType = CreatureType,
            Subtype = Subtype
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
            var additionalCostText = AdditionalCost != null ? $"Additional Cost : {AdditionalCost.RulesText}\r\n" : "";
            var abilitiesText = string.Join("\r\n", Abilities.Select(ab => ab.RulesText));
            var effectsText = string.Join("\r\n", Effects.Select(ef => ef.RulesText));

            return additionalCostText + abilitiesText + effectsText;
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
            Colors = Colors,
            Abilities = Abilities.ToList(), //todo - potential deep clone.
            Effects = Effects.ToList(),
            AdditionalCost = AdditionalCost, //clone this?
            Subtype = Subtype
        };
    }
}

public class ItemCardData : BaseCardData
{
    public override string CardType => "Item";

    public override BaseCardData Clone()
    {
        Abilities = Abilities.Select(ab => ab.Clone()).ToList();
        return new ItemCardData()
        {
            Name = Name,
            ManaCost = ManaCost,
            ArtPath = ArtPath,
            Colors = Colors,
            Abilities = Abilities.ToList(), //todo - potential deep clone.
            Subtype = Subtype
        };
    }
}


public class ManaCardData : BaseCardData
{
    public override string CardType => "Mana";
    //TODO - We need to revamp this somehow
    public string ManaAdded { get; set; } = "*";
    public override string RulesText => $"Add {ManaAdded} to your mana";
    public bool ReadyImmediately { get; set; } = true;
    public IManaReadyCondition ReadyCondition { get; set; } = null;
    public override BaseCardData Clone()
    {
        return new ManaCardData()
        {
            ManaAdded = ManaAdded,
            Name = Name,
            Colors = Colors,
            ArtPath = ArtPath,
            Abilities = Abilities.ToList(),
            ReadyImmediately = ReadyImmediately,
            ReadyCondition = ReadyCondition
        };
    }
}

public interface IManaReadyCondition
{
    bool IsReady(CardGame cardGame, Player owner);
}

public class LessThan3ManaReadyCondition : IManaReadyCondition
{
    public bool IsReady(CardGame cardGame, Player owner)
    {
        return owner.ManaPool.TotalColorlessMana < 3;
    }
}

public class AlreadyHasManaCondition : IManaReadyCondition
{
    public string ManaNeeded { get; set; }
    public bool IsReady(CardGame cardGame, Player owner)
    {
        var isReady = false;
        foreach (var manaChar in ManaNeeded)
        {
            if (owner.ManaPool.TotalMana.ToManaString().Contains(manaChar))
            {
                isReady = true;
                break;
            }
        }
        return isReady;
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
            ManaCost = "1G",
            Power = 2,
            Toughness = 2,
            Colors = new List<CardColor> { CardColor.Green },
            ArtPath = "CardArt/GrizzlyBear"
        });

        _cards.Add(new SpellCardData()
        {
            Name = "Lightning Bolt",
            ManaCost = "R",
            ArtPath = "CardArt/LightningBolt",
            Colors = new List<CardColor> { CardColor.Red },
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
            ManaCost = "RW",
            ArtPath = "CardArt/LightningHelix",
            Colors = new List<CardColor>() { CardColor.Red, CardColor.White },
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
            ManaCost = "G",
            ArtPath = "CardArt/GiantGrowth",
            Colors = new List<CardColor>() { CardColor.Green },
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
            ManaCost = "U",
            ArtPath = "CardArt/AncestralRecall",
            Colors = new List<CardColor>() { CardColor.Blue },
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
            Colors = new List<CardColor>() { CardColor.Colorless },
            Effects = new List<Effect>()
            {
                new AddTempManaEffect()
                {
                    ManaToAdd = "***"
                }
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Storm Crow",
            ManaCost = "1U",
            Power = 1,
            Toughness = 2,
            ArtPath = "CardArt/StormCrow",
            Colors = new List<CardColor>() { CardColor.Blue },
            Abilities = new List<CardAbility>()
            {
                new FlyingAbility()
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Goblin Raider",
            ManaCost = "1R",
            Power = 2,
            Toughness = 2,
            ArtPath = "CardArt/GoblinRaider",
            CreatureType = "Goblin",
            Colors = new List<CardColor>() { CardColor.Red },
            Abilities = new List<CardAbility>()
            {
                new CantBlockAbility()
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Ajani's Sunstriker",
            ManaCost = "1W",
            Power = 2,
            Toughness = 2,
            ArtPath = "CardArt/AjanisSunstriker",
            Colors = new List<CardColor>() { CardColor.White },
            Abilities = new List<CardAbility>()
            {
                new LifelinkAbility()
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Typhoid Rats",
            ManaCost = "B",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/TyphoidRats",
            Colors = new List<CardColor>() { CardColor.Black },
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
            Colors = new List<CardColor> { CardColor.Colorless },
            ArtPath = "CardArt/HexplateGolem"
        }); ;

        _cards.Add(new UnitCardData()
        {
            Name = "Vampire Nighthawk",
            ManaCost = "1BB",
            Power = 2,
            Toughness = 3,
            ArtPath = "CardArt/VampireNighthawk",
            Colors = new List<CardColor>() { CardColor.Black },
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
            ManaCost = "UB",
            Power = 2,
            Toughness = 1,
            ArtPath = "CardArt/InkfathomInfiltrator",
            Colors = new List<CardColor>() { CardColor.Blue, CardColor.Black },
            Abilities = new List<CardAbility>()
            {
                new UnblockableAbility(),
                new CantBlockAbility()
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Kalonian Behemoth",
            ManaCost = "5GG",
            Power = 9,
            Toughness = 9,
            ArtPath = "CardArt/KalonianBehemoth",
            Colors = new List<CardColor>() { CardColor.Green },
            Abilities = new List<CardAbility>()
               {
                   new ShroudAbility()
               }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Invisible Stalker",
            ManaCost = "1U",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/InvisibleStalker",
            Colors = new List<CardColor> { CardColor.Blue },
            Abilities = new List<CardAbility>()
               {
                   new HexproofAbility(),
                   new UnblockableAbility()
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Rorix, Bladewing",
            ManaCost = "3RRR",
            Power = 6,
            Toughness = 5,
            ArtPath = "CardArt/RorixBladewing",
            Colors = new List<CardColor> { CardColor.Red },
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
            ManaCost = "1G",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/ElvishVisionary",
            Colors = new List<CardColor>() { CardColor.Green },
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
            ManaCost = "4U",
            Power = 2,
            Toughness = 2,
            ArtPath = "CardArt/Mulldrifter",
            Colors = new List<CardColor>() { CardColor.Blue },
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
            ManaCost = "1B",
            Power = 2,
            Toughness = 1,
            ArtPath = "CardArt/DarkConfidant",
            Colors = new List<CardColor> { CardColor.Black },
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
            ManaCost = "4GG",
            Power = 6,
            Toughness = 6,
            ArtPath = "CardArt/ColossalDreadmaw",
            Colors = new List<CardColor>() { CardColor.Green },
            Abilities = new List<CardAbility>()
            {
                new TrampleAbility()
            }
        });

        //Ball Lightning
        _cards.Add(new UnitCardData()
        {
            Name = "Ball Lightning",
            ManaCost = "RRR",
            Power = 6,
            Toughness = 1,
            ArtPath = "CardArt/BallLightning",
            Colors = new List<CardColor>() { CardColor.Red },
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
            ManaCost = "U",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/DelverOfSecrets",
            Colors = new List<CardColor> { CardColor.Blue },
            Abilities = new List<CardAbility>()
            {
                new TriggeredAbility
                (
                    TriggerType.AtTurnStart,
                    new DelverTransformEffect
                    {
                        TransformData = new UnitCardData
                        {
                            Name = "Insectile Aberration",
                            ManaCost = "U",
                            Power = 3,
                            Toughness = 2,
                            ArtPath = "CardArt/InsectileAberration",
                            Colors = new List<CardColor>{CardColor.Blue},
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
            ManaCost = "1BB",
            ArtPath = "CardArt/Murder",
            Colors = new List<CardColor>() { CardColor.Black },
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
            ManaCost = "2WW",
            ArtPath = "CardArt/DayofJudgment",
            Colors = new List<CardColor>() { CardColor.White },
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
            ManaCost = "1R",
            ArtPath = "CardArt/Pyroclasm",
            Colors = new List<CardColor> { CardColor.Red },
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
            ManaCost = "WB",
            ArtPath = "CardArt/ZealousPersecution",
            Colors = new List<CardColor> { CardColor.White, CardColor.Black },
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
            ManaCost = "WWW",
            Power = 3,
            Toughness = 3,
            ArtPath = "CardArt/BenalishMarshall",
            Colors = new List<CardColor> { CardColor.White },
            Abilities = new List<CardAbility>
            {
                new StaticAbility
                {
                    Effects = new List<Effect>
                    {
                        new StaticPumpEffect
                        {
                            Power = 1,
                            Toughness = 1,
                            TargetType = TargetType.OtherCreaturesYouControl,
                        }
                    }
                }
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Shivan Dragon",
            ManaCost = "4RR",
            Power = 5,
            Toughness = 5,
            ArtPath = "CardArt/ShivanDragon",
            Colors = new List<CardColor> { CardColor.Red },
            Abilities = new List<CardAbility>
            {
                new FlyingAbility(),
                new ActivatedAbility()
                {
                    ManaCost = "1",
                    Effects = new List<Effect>{ new PumpUnitEffect()
                    {
                        Power = 1,
                        Toughness = 0,
                        TargetType = TargetType.Self
                    }
                    }
                }
            }
        });

        _cards.Add(new ManaCardData()
        {
            Name = "Wastes",
            ManaAdded = "1",
            ArtPath = "CardArt/Wastes",
        });

        _cards.Add(new ManaCardData()
        {
            Name = "City of Brass",
            ManaAdded = "1*",
            Colors = new List<CardColor>() { CardColor.Red, CardColor.White, CardColor.Blue, CardColor.Black, CardColor.Green },
            ArtPath = "CardArt/CityOfBrass"
        });

        _cards.Add(new ManaCardData()
        {
            Name = "Plains",
            ManaAdded = "1W",
            ArtPath = "CardArt/Plains",
            Colors = new List<CardColor> { CardColor.White }
        });

        _cards.Add(new ManaCardData()
        {
            Name = "Island",
            ManaAdded = "1U",
            Colors = new List<CardColor> { CardColor.Blue },
            ArtPath = "CardArt/Island"
        });

        _cards.Add(new ManaCardData()
        {
            Name = "Swamp",
            ManaAdded = "1B",
            Colors = new List<CardColor> { CardColor.Black },
            ArtPath = "CardArt/Swamp"
        });

        _cards.Add(new ManaCardData()
        {
            Name = "Mountain",
            ManaAdded = "1R",
            Colors = new List<CardColor> { CardColor.Red },
            ArtPath = "CardArt/Mountain"
        });

        _cards.Add(new ManaCardData()
        {
            Name = "Forest",
            ManaAdded = "1G",
            Colors = new List<CardColor> { CardColor.Green },
            ArtPath = "CardArt/Forest"
        });


        _cards.Add(new UnitCardData()
        {
            Name = "Mogg Fanatic",
            ManaCost = "R",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/Mogg Fanatic",
            CreatureType = "Goblin",
            Colors = new List<CardColor> { CardColor.Red },
            Abilities = new List<CardAbility>
                {
                     new ActivatedAbility()
                    {
                        ManaCost = "0",
                        //this needs to have rules text automatically generated.
                        AdditionalCost = new SacrificeSelfAdditionalCost(),
                        Effects = new List<Effect>{new DamageEffect()
                        {
                           Amount = 1,
                           TargetType = TargetType.TargetUnitsOrPlayers
                        }
                        }
                    }
                }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Unspeakable Symbolite",
            ManaCost = "1BB",
            Power = 2,
            Toughness = 2,
            ArtPath = "CardArt/UnspeakableSymbolite",
            Colors = new List<CardColor> { CardColor.Black },
            Abilities = new List<CardAbility>
            {
                    new ActivatedAbility()
                    {
                        ManaCost = "0",
                        //this needs to have rules text automatically generated.
                        AdditionalCost = new PayLifeAdditionalCost()
                        {
                            Amount = 3,
                        },
                        Effects = new List<Effect>{ new PumpUnitEffect()
                        {
                           Power = 1,
                           Toughness = 1
                        }
                        }
                    }
            }

        });

        _cards.Add(new UnitCardData()
        {
            Name = "Nantuko Husk",
            ManaCost = "2B",
            Power = 2,
            Toughness = 2,
            ArtPath = "CardArt/Nantuko Husk",
            Colors = new List<CardColor> { CardColor.Black },
            Abilities = new List<CardAbility>
            {
                new ActivatedAbility()
                {
                    ManaCost = "0",
                    AdditionalCost = new SacrificeCreatureAdditionalCost(),
                    Effects = new List<Effect>{ new PumpUnitEffect()
                    {
                        Power = 2,
                        Toughness = 2,
                        TargetType = TargetType.UnitSelf
                    }
                    }
                 }
            }
        });

        //Things needed - creature types.

        //Skirk Prospector
        _cards.Add(new UnitCardData()
        {
            Name = "Skirk Prospector",
            ManaCost = "R",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/Skirk Prospector",
            CreatureType = "Goblin",
            Colors = new List<CardColor> { CardColor.Red },
            Abilities = new List<CardAbility>
            {
                new ActivatedAbility()
                {
                    ManaCost = "0",
                    AdditionalCost = new SacrificeCreatureAdditionalCost()
                    {
                        Filter = new CardFilter
                        {
                            CreatureType = "Goblin"
                        }
                    },
                    Effects = new List<Effect>{new AddTempManaEffect()
                    {
                        ManaToAdd = "1R"
                    }
                    },
                 }
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Goblin Sledder",
            ManaCost = "R",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/Goblin Sledder",
            CreatureType = "Goblin",
            Colors = new List<CardColor> { CardColor.Red },
            Abilities = new List<CardAbility>
            {
                new ActivatedAbility()
                {
                    ManaCost = "0",
                    AdditionalCost = new SacrificeCreatureAdditionalCost()
                    {
                        Filter = new CardFilter
                        {
                            CreatureType = "Goblin"
                        }
                    },
                    Effects = new List<Effect>{ new PumpUnitEffect()
                    {
                        Power = 1,
                        Toughness =1
                    }
                    },
                 }
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Siege Gang Commander",
            ManaCost = "3RR",
            Power = 2,
            Toughness = 2,
            ArtPath = "CardArt/Siege Gang Commander",
            CreatureType = "Goblin",
            Colors = new List<CardColor> { CardColor.Red },
            Abilities = new List<CardAbility>
            {
                new TriggeredAbility(TriggerType.SelfEntersPlay
                ,new CreateTokenEffect<UnitCardData>(new UnitCardData()
                            {
                                Name = "Goblin Token",
                                ManaCost = "0",
                                Power = 1,
                                Toughness =1,
                                ArtPath = "CardArt/Goblin Token",
                                CreatureType = "Goblin",
                                Colors = new List<CardColor>{CardColor.Red }
                            })
                        {
                            AmountOfTokens = 3,

                        }
                   ),
                new ActivatedAbility()
                {
                    ManaCost = "1R",
                    AdditionalCost = new SacrificeCreatureAdditionalCost()
                    {
                        Filter = new CardFilter
                        {
                            CreatureType = "Goblin"
                        }
                    },
                    Effects = new List<Effect>{ new DamageEffect()
                    {
                        Amount = 2,
                        TargetType = TargetType.TargetUnitsOrPlayers
                    }
                    },
                 }
            }
        });
        //Mogg War Marshall -things needed - OnDeath - Create Token
        _cards.Add(new UnitCardData()
        {
            Name = "Mogg War Marshall",
            ManaCost = "1R",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/Mogg War Marshall",
            CreatureType = "Goblin",
            Colors = new List<CardColor> { CardColor.Red },
            Abilities = new List<CardAbility>
            {
                new TriggeredAbility(
                    TriggerType.SelfEntersPlay,
                    new CreateTokenEffect<UnitCardData>(TokenHelper.GoblinToken())),

                new TriggeredAbility(TriggerType.SelfDies,
                new CreateTokenEffect<UnitCardData>(TokenHelper.GoblinToken()))
            }
        });

        //Goblin Piledriver - things needed - count creature types / (protection?) (we will do protection later)
        _cards.Add(new UnitCardData()
        {
            Name = "Goblin Piledriver",
            ManaCost = "1R",
            Power = 1,
            Toughness = 2,
            ArtPath = "CardArt/Goblin Piledriver",
            CreatureType = "Goblin",
            Colors = new List<CardColor> { CardColor.Red },
            Abilities = new List<CardAbility>
            {
                 new TriggeredAbility(
                    TriggerType.SelfAttacks,
                    new GoblinPiledriverEffect()
                    )
            }
        });

        //Goblin Warchief - things needed - mana cost reduction, static ability gainers
        _cards.Add(new UnitCardData()
        {
            Name = "Goblin Warchief",
            ManaCost = "1RR",
            Power = 2,
            Toughness = 2,
            ArtPath = "CardArt/Goblin Warchief",
            CreatureType = "Goblin",
            Colors = new List<CardColor> { CardColor.Red },
            Abilities = new List<CardAbility>
            {
                new HasteAbility(),
                new StaticAbility()
                {
                    Effects = new List<Effect>
                    {
                        new StaticManaReductionEffect
                        {
                            ReductionAmount = "1",
                            TargetType = TargetType.CardsInHand,
                            Filter = new CardFilter{CreatureType = "Goblin"}
                        }
                    }
                },
                new StaticAbility()
                {
                    Effects = new List<Effect>
                    {
                        new StaticGiveAbilityEffect
                        {
                            TargetType = TargetType.OtherCreaturesYouControl,
                            Ability = new HasteAbility(),
                            Filter = new CardFilter{CreatureType = "Goblin"}
                        }
                    }
                }
            }
        });

        _cards.Add(
            new UnitCardData
            {
                Name = "Goblin Matron",
                ManaCost = "2R",
                Power = 1,
                Toughness = 1,
                ArtPath = "CardArt/Goblin Matron",
                CreatureType = "Goblin",
                Colors = new List<CardColor> { CardColor.Red },
                Abilities = new List<CardAbility>
                {
                    new TriggeredAbility
                    {
                        TriggerType = TriggerType.SelfEntersPlay,
                        Effects = new List<Effect>
                        {
                            new GetRandomCardFromDeckEffect
                            {
                                Filter = new CardFilter
                                {
                                    CreatureType = "Goblin"
                                }
                            }
                        }
                    }
                }
            });


        _cards.Add(
               new UnitCardData
               {
                   Name = "Goblin Ringleader",
                   ManaCost = "3R",
                   Power = 2,
                   Toughness = 2,
                   ArtPath = "CardArt/Goblin Ringleader",
                   CreatureType = "Goblin",
                   Colors = new List<CardColor> { CardColor.Red },
                   Abilities = new List<CardAbility>
                   {
                    new HasteAbility(),
                    new TriggeredAbility
                    {
                        TriggerType = TriggerType.SelfEntersPlay,
                        Effects = new List<Effect>
                        {
                            new GrabFromTopOfDeckEffect
                            {
                                CardsToLookAt = 5,
                                Filter = new CardFilter{ CreatureType = "Goblin" },
                                Amount = 9999 //need a way to say as many as possible.

                            }
                        }
                    }
                   }
               });



        //Goblin Sharpshooter - things needed - activate ability only once per turn + reset?
        //Seething Song - nothing
        //Goblin Ringleader - things needed - conditional draw from deck
        //Gempalm Incinerator - things needed (cycling?? - maybe don't implement this) - counting goblins for damage.
        //Goblin King - things needed - static effects


        //Implementing the UG Madness Deck:
        /*
                Creature(21)
        3 Wonder - applying static effects from the graveyard / other zones.
        4 Aquamoeba - discarding cards as a choice / switching power and toughness
        2 Merfolk Looter - discarding cards as a choice as resolving an ability. -Need to have an in between state where costs are still being resolved.
        4 Wild Mongrel - discarding cards as a choice as a cost for an ability.
        4 Basking Rootwalla - playing when discarded
        4 Arrogant Wurm - playing when discarded
        Sorcery(11) 
        4 Deep Analysis - playing cards from graveyard
        4 Careful Study - discarding cards as a choice
        3 Roar of the Wurm - playing cards from graveyard
        Instant(6) 
        4 Circular Logic - N/A
        2 Unsummon - bouncing cards (new effect, but should be ok)
        Land(22)

        2 Centaur Garden
        6 Forest
        10 Island
        4 Yavimaya Coast - Dual Lands.
        60 Cards
        */

        _cards.Add(
            new UnitCardData
            {
                Name = "Merfolk Looter",
                ManaCost = "1U",
                Power = 1,
                Toughness = 1,
                ArtPath = "CardArt/Merfolk Looter",
                CreatureType = "Merfolk",
                Colors = new List<CardColor> { CardColor.Blue },
                Abilities = new List<CardAbility>
                {
                    new ActivatedAbility()
                    {
                        OncePerTurn = true,
                        Effects = new List<Effect>
                        {
                            new DrawCardEffect
                            {
                                TargetType = TargetType.Self,
                                Amount = 1
                            },
                            new DiscardCardEffect
                            {
                                TargetType= TargetType.Self,
                                Amount = 1
                            }
                        }
                    }
                }
            });

        _cards.Add(
       new UnitCardData
       {
           Name = "Wonder",
           ManaCost = "3U",
           Power = 2,
           Toughness = 2,
           ArtPath = "CardArt/Wonder",
           CreatureType = "Incarnation",
           Colors = new List<CardColor> { CardColor.Blue },
           Abilities = new List<CardAbility>
           {
                    new FlyingAbility(),
                    new StaticAbility
                    {
                        ApplyWhenIn = ZoneType.Discard,
                        Effects = new List<Effect>()
                        {
                            new StaticGiveAbilityEffect
                            {
                                Ability = new FlyingAbility(),
                                TargetType = TargetType.OtherCreaturesYouControl
                            }
                        }
                    }
           }
       });


        _cards.Add(new UnitCardData()
        {
            Name = "Wild Mongrel",
            ManaCost = "1G",
            Power = 2,
            Toughness = 2,
            ArtPath = "CardArt/Wild Mongrel",
            Colors = new List<CardColor> { CardColor.Green },
            Abilities = new List<CardAbility>
    {
        new ActivatedAbility()
        {
            ManaCost = "0",
            AdditionalCost = new DiscardCardAdditionalCost
            {
            },
            Effects = new List<Effect>{  new PumpUnitEffect
            {
                Power = 1,
                Toughness = 1,
                TargetType = TargetType.UnitSelf
            }
            }
        }
    }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Aquamoeba",
            ManaCost = "1U",
            Power = 1,
            Toughness = 3,
            ArtPath = "CardArt/Aquamoeba",
            Colors = new List<CardColor> { CardColor.Blue },
            Abilities = new List<CardAbility>
            {
                new ActivatedAbility(){
                ManaCost = "0",
                AdditionalCost = new DiscardCardAdditionalCost
                {
                },
                Effects = new List<Effect>{  new SwitchPowerToughnessEffect
            {
                TargetType = TargetType.UnitSelf
                }
            }
            }
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Basking Rootwalla",
            ManaCost = "G",
            Power = 1,
            Toughness = 1,
            ArtPath = "CardArt/Aquamoeba",
            Colors = new List<CardColor> { CardColor.Green },
            Abilities = new List<CardAbility>
            {
                new ActivatedAbility(){
                ManaCost = "1G",
                Effects = new List<Effect>{  new PumpUnitEffect{Power = 2, Toughness = 2, TargetType = TargetType.UnitSelf},
                }
                },
                new MadnessAbility()
                {
                    ManaCost = "0"
                }
            }
        });

        _cards.Add(new SpellCardData()
        {
            Name = "Deep Analysis",
            ManaCost = "3U",
            ArtPath = "CardArt/Deep Analysis",
            Colors = new List<CardColor> { CardColor.Blue },
            Effects = new List<Effect>
            {
                new DrawCardEffect
                {
                    Amount = 2
                }
            },
            Abilities = new List<CardAbility>
            {
                new FlashbackAbility
                {
                    ManaCost = "1U",
                    AdditionalCost = new PayLifeAdditionalCost
                    {
                        Amount = 3
                    }
                }
            }
        });

        _cards.Add(new SpellCardData
        {
            Name = "Goblin Grenade",
            ManaCost = "R",
            Colors = new List<CardColor> { CardColor.Red },
            AdditionalCost = new SacrificeCreatureAdditionalCost
            {
                Filter = new CardFilter
                {
                    CreatureType = "Goblin"
                }
            },
            Effects = new List<Effect>
            {
                new DamageEffect
                {
                    Amount = 5,
                    TargetType = TargetType.TargetUnitsOrPlayers
                }
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Griselbrand",
            ManaCost = "5BBB",
            Power = 7,
            Toughness = 7,
            ArtPath = "CardArt/Griselbrand",
            Colors = new List<CardColor> { CardColor.Black },
            Abilities = new List<CardAbility>
    {
        new LifelinkAbility(),
        new ActivatedAbility()
        {
            ManaCost = "0",
            AdditionalCost =  new PayLifeAdditionalCost()
            {
                Type = AdditionalCostType.PayLife,
                Amount = 7
            },
            Effects = new List<Effect>{ new DrawCardEffect()
            {
                Amount = 7
            }
            }
        }
    }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Greedy Merchant",
            ManaCost = "BBB",
            Power = 3,
            Toughness = 3,
            ArtPath = "CardArt/Greedy Merchant",
            Colors = new List<CardColor> { CardColor.Black },
            Abilities = new List<CardAbility>
    {
        new ActivatedAbility()
        {
            ManaCost = "1B",
            AdditionalCost = new PayLifeAdditionalCost()
            {
                Type = AdditionalCostType.PayLife,
                Amount = 1,
            },
            Effects = new List<Effect>{ new DrawCardEffect()
            {
                Amount = 1
            }
            }
        }
    }
        });


        _cards.Add(new SpellCardData()
        {
            Name = "Careful Study",
            ManaCost = "U",
            ArtPath = "CardArt/Careful Study",
            Colors = new List<CardColor> { CardColor.Blue },
            Effects = new List<Effect>
            {
                new DrawCardEffect()
                {
                    TargetType = TargetType.Self,
                    Amount = 2
                },
                new DiscardCardEffect()
                {
                    TargetType= TargetType.Self,
                    Amount = 2
                }
            }
        });

        _cards.Add(new ItemCardData
        {
            Name = "Chromatic Sphere",
            ManaCost = "1",
            ArtPath = "CardArt/Chromatic Sphere",
            Colors = new List<CardColor> { }, //Colorless,
            Subtype = "Artifact",
            Abilities = new List<CardAbility>
            {
                new ActivatedAbility()
                {
                    ManaCost = "1",
                    AdditionalCost = new SacrificeSelfAdditionalCost(),
                    Effects = new List<Effect>
                    {
                        new DrawCardEffect
                        {
                            Amount = 1
                        },
                        new AddTempManaEffect
                        {
                            ManaToAdd = "1*"
                        },
                    }
                }
            }
        });

        //Cranial Plating should be some sort of equipment, but we can just make it an item.
        _cards.Add(new ItemCardData
        {
            Name = "Cranial Plating",
            ManaCost = "2",
            Colors = new List<CardColor> { }, // Colorless,
            Subtype = "Artifact",
            Abilities = new List<CardAbility>
            {
                new ActivatedAbility()
                {
                    ManaCost = "1",
                    OncePerTurn = true,
                    Effects = new List<Effect>
                    {
                        new PumpPowerByNumberOfArtifactsEffect()
                        {
                            TargetType = TargetType.TargetUnits
                        }
                    }
                }
            }
        });

        _cards.Add(new UnitCardData
        {
            Name = "Ornithopter",
            ManaCost = "0",
            Colors = new List<CardColor> { },
            Subtype = "Artifact",
            Abilities = new List<CardAbility>
            {
                new FlyingAbility()
            },
            Power = 0,
            Toughness = 2
        });

        //Instead of regen, we will give our cards shields.
        _cards.Add(new ItemCardData
        {
            Name = "Welding Jar",
            ManaCost = "0",
            Colors = new List<CardColor> { },
            Subtype = "Artifact",
            Abilities = new List<CardAbility>
            {
                new ActivatedAbility()
                {
                    ManaCost ="0",
                    AdditionalCost = new SacrificeSelfAdditionalCost(),
                    Filter = new CardFilter
                    {
                        Subtype = "artifact"
                    },
                    Effects = new List<Effect>
                    {
                        new GiveShieldEffect()
                        {
                        TargetType = TargetType.TargetUnits
                        }
                    }
                }
            }
        });

        _cards.Add(new SpellCardData
        {
            Name = "Thoughtcast",
            ManaCost = "4U",
            Colors = new List<CardColor> { CardColor.Blue },
            Abilities = new List<CardAbility>
            {
                new AffinityAbility()
            },
            Effects = new List<Effect>
            {
                new DrawCardEffect
                {
                    Amount = 2
                }
            }
        });

        _cards.Add(new UnitCardData
        {
            Name = "Frogmite",
            ManaCost = "4",
            Subtype = "Artifact",
            Colors = new List<CardColor> { },
            Abilities = new List<CardAbility>
            {
                new AffinityAbility()
            },
            Power = 2,
            Toughness = 2
        });

        _cards.Add(new UnitCardData
        {
            Name = "Myr Enforcer",
            ManaCost = "7",
            Subtype = "Artifact",
            Colors = new List<CardColor> { },
            Abilities = new List<CardAbility>
            {
                new AffinityAbility()
            },
            Power = 4,
            Toughness = 4
        });

        _cards.Add(new UnitCardData
        {
            Name = "Arcbound Worker",
            ManaCost = "1",
            Subtype = "Artifact",
            ArtPath = "CardArt/ArcboundWorker",
            Colors = new List<CardColor> { },
            Abilities = new List<CardAbility>
            {
                new ModularAbility
                {
                    Amount = 1
                }
            },
            Power = 0,
            Toughness = 0
        });

        _cards.Add(new UnitCardData
        {
            Name = "Arcbound Ravager",
            ManaCost = "2",
            Subtype = "Artifact",
            ArtPath = "CardArt/ArcboundRavager",
            Colors = new List<CardColor> { },
            Abilities = new List<CardAbility>
            {
                new ModularAbility
                {
                    Amount = 1
                },
                new ActivatedAbility
                {
                    ManaCost = "0",
                    AdditionalCost = new SacrificeAdditionalCost
                    {
                        Filter = new CardFilter{Subtype = "artifact"}
                    },
                    Effects = new List<Effect>
                    {
                        new AddPlusOnePlusOneCounterEffect
                        {
                            Amount = 1,
                            TargetType = TargetType.UnitSelf
                        }
                    }
                }
            },
            Power = 0,
            Toughness = 0
        });

        _cards.Add(new UnitCardData
        {
            Name = "Atog",
            ManaCost = "1R",
            Colors = new List<CardColor> { CardColor.Red },
            Power = 1,
            Toughness = 2,
            ArtPath = "CardArt/Atog",
            Abilities = new List<CardAbility>
            {
                new ActivatedAbility
                {
                    ManaCost = "0",
                    AdditionalCost = new SacrificeAdditionalCost
                    {
                        Filter = new CardFilter{Subtype = "Artifact"}
                    },
                    Effects = new List<Effect>
                    {
                        new PumpUnitEffect
                        {
                            Power = 2,
                            Toughness = 2,
                            TargetType = TargetType.UnitSelf
                        }
                    }
                }
            }
        });

        _cards.Add(new UnitCardData
        {
            Name = "Somber Hoverguard",
            ManaCost = "5U",
            Colors = new List<CardColor> { CardColor.Blue },
            Power = 3,
            Toughness = 2,
            ArtPath = "CardArt/SomberHoverguard",
            Abilities = new List<CardAbility>
            {
                new FlyingAbility(),
                new AffinityAbility()
            }
        });

        _cards.Add(new UnitCardData
        {
            Name = "Disciple of the Vault",
            ManaCost = "B",
            Colors = new List<CardColor> { CardColor.Black },
            Power = 1,
            Toughness = 1,
            Abilities = new List<CardAbility>
            {
                new TriggeredAbility
                {
                    TriggerType = TriggerType.SomethingDies,
                    Filter = new CardFilter {Subtype = "Artifact"},
                    Effects = new List<Effect>
                    {
                        new DamageEffect
                        {
                            Amount = 1,
                            TargetType = TargetType.Opponent
                        }
                    }
                }
            }
        });

        _cards.Add(new ManaCardData()
        {
            Name = "Blinkmoth Nexus",
            ManaAdded = "1",
            Abilities = new List<CardAbility>()
            {
                new TriggeredAbility()
                {
                    TriggerType = TriggerType.SelfEntersPlay,
                    Effects = new List<Effect>
                    {
                        new CreateTokenEffect<UnitCardData>
                        {
                            TokenData = new UnitCardData()
                            {
                                Name = "Blinkmoth",
                                Power = 1,
                                Toughness = 1,
                                Subtype = "Artifact",
                                Abilities = new List<CardAbility>
                                {
                                    new FlyingAbility()
                                }
                            },
                            AmountOfTokens = 1

                        }
                    }
                }
            }

        });

        _cards.Add(new ManaCardData()
        {
            Name = "Great Furnace",
            ManaAdded = "1",
            Colors = new List<CardColor> { CardColor.Red },
            Abilities = new List<CardAbility>()
            {
                new TriggeredAbility()
                {
                    TriggerType = TriggerType.SelfEntersPlay,
                    Effects = new List<Effect>
                    {
                        new CreateTokenEffect<ItemCardData>
                        {
                            TokenData = new ItemCardData()
                            {
                                Name = "Great Furnace",
                                Subtype = "Artifact",
                                Abilities = new List<CardAbility>
                                {
                                    new ActivatedAbility()
                                    {
                                        OncePerTurn = true,
                                        ManaCost = "1",
                                        Effects = new List<Effect>
                                        {
                                            new AddTempManaEffect
                                            {
                                                ManaToAdd = "R"
                                            }
                                        }
                                    }
                                }
                            },
                            AmountOfTokens = 1

                        }
                    }
                }
            }

        });

        _cards.Add(new ManaCardData()
        {
            Name = "Seat of the Synod",
            ManaAdded = "1",
            Colors = new List<CardColor> { CardColor.Blue },
            Abilities = new List<CardAbility>()
            {
                new TriggeredAbility()
                {
                    TriggerType = TriggerType.SelfEntersPlay,
                    Effects = new List<Effect>
                    {
                        new CreateTokenEffect<ItemCardData>
                        {
                            TokenData = new ItemCardData()
                            {
                                Name = "Seat of the Synod",
                                Subtype = "Artifact",
                                Abilities = new List<CardAbility>
                                {
                                    new ActivatedAbility()
                                    {
                                        OncePerTurn = true,
                                        ManaCost = "1",
                                        Effects = new List<Effect>
                                        {
                                            new AddTempManaEffect
                                            {
                                                ManaToAdd = "U"
                                            }
                                        }
                                    }
                                }
                            },
                            AmountOfTokens = 1

                        }
                    }
                }
            }

        });

        _cards.Add(new ManaCardData()
        {
            Name = "Vault of Whispers",
            ManaAdded = "1",
            Colors = new List<CardColor> { CardColor.Black },
            Abilities = new List<CardAbility>()
            {
                new TriggeredAbility()
                {
                    TriggerType = TriggerType.SelfEntersPlay,
                    Effects = new List<Effect>
                    {
                        new CreateTokenEffect<ItemCardData>
                        {
                            TokenData = new ItemCardData()
                            {
                                Name = "Vault of Whispers",
                                Subtype = "Artifact",
                                Abilities = new List<CardAbility>
                                {
                                    new ActivatedAbility()
                                    {
                                        OncePerTurn = true,
                                        ManaCost = "1",
                                        Effects = new List<Effect>
                                        {
                                            new AddTempManaEffect
                                            {
                                                ManaToAdd = "B"
                                            }
                                        }
                                    }
                                }
                            },
                            AmountOfTokens = 1

                        }
                    }
                }
            }

        });

        _cards.Add(new SpellCardData
        {
            Name = "Shrapnel Blast",
            ManaCost = "1R",
            Colors = new List<CardColor> { CardColor.Red },
            AdditionalCost = new SacrificeAdditionalCost { Filter = new CardFilter { Subtype = "Artifact" } },
            Effects = new List<Effect>
            {
                new DamageEffect
                {
                   Amount = 5
                }
            }
        });

        _cards.Add(new ItemCardData
        {
            Name = "Chrome Mox",
            ManaCost = "0",
            Colors = new List<CardColor> { },
            Subtype = "Artifact",
            Abilities = new List<CardAbility>
            {
                //Imprint has two parts, // not too sure how to do this yet.
                //When we discard the card, check to see if the source has imprint, if it does, save a reference of that card
                //to the cards ability components as an ImprintedCardComponent?
                //then our imprint effect can modify the AddManaEffect as needed. 
                new ImprintAbility()
                {
                }
            }
        });

        _cards.Add(new UnitCardData
        {
            Name = "Restoration Angel",
            ManaCost = "3W",
            Colors = new List<CardColor> { CardColor.White },
            CreatureType = "Angel",
            Power = 3,
            Toughness = 4,
            Abilities = new List<CardAbility>
            {
                new FlyingAbility(),
                new TriggeredAbility()
                {
                    TriggerType = TriggerType.SelfEntersPlay,
                    Effects = new List<Effect>
                    {
                        new BlinkEffect
                        {
                            Filter = new CardFilter
                            {
                                Not = true,
                                CreatureType = "Angel"
                            },
                            TargetType = TargetType.RandomOurUnits
                        }
                    }
                }
            }
        });

        _cards.Add(new UnitCardData
        {
            Name = "Geist of Saint Traft",
            ManaCost = "1UW",
            Colors = new List<CardColor> { CardColor.Blue, CardColor.White },
            Power = 2,
            Toughness = 2,
            Abilities = new List<CardAbility>
            {
                new HexproofAbility(),
                new TriggeredAbility()
                {
                    TriggerType= TriggerType.SelfAttacks,
                    Effects = new List<Effect>
                    {
                        new CreateTokenEffect<UnitCardData>{
                            TokenData = new UnitCardData
                            {
                                Name = "Angel",
                                CreatureType = "Angel",
                                Power = 4,
                                Toughness = 4,
                                Abilities = new List<CardAbility>
                                {
                                    new FlyingAbility(),
                                    new HasteAbility(),
                                    new TriggeredAbility()
                                    {
                                        TriggerType = TriggerType.AtTurnEnd,
                                        Effects = new List<Effect>
                                        {
                                            new SacrificeSelfEffect()
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });

        _cards.Add(new UnitCardData
        {
            Name = "Snapcaster Mage",
            ManaCost = "1U",
            Colors = new List<CardColor> { CardColor.Blue },
            Power = 2,
            Toughness = 1,
            Abilities = new List<CardAbility>
            {
                new TriggeredAbility()
                {
                    TriggerType =TriggerType.SelfEntersPlay,
                    Effects = new List<Effect>
                    {
                        //ModificationsOnPlayersNow!
                        new SnapcasterMageEffect
                        {

                        }
                    }
                }
            }
        });

        _cards.Add(new SpellCardData
        {
            Name = "Gitaxian Probe",
            ManaCost = "0",
            Colors = new List<CardColor> { CardColor.Blue },
            AdditionalCost = new PayLifeAdditionalCost
            {
                Amount = 2
            },
            Effects = new List<Effect>
            {
                new DrawCardEffect
                {
                    Amount = 1
                }
            }
            //TODO - Reveal Hand
        });

        _cards.Add(new SpellCardData
        {
            Name = "Ponder",
            ManaCost = "U",
            Colors = new List<CardColor> { CardColor.Blue },
            Effects = new List<Effect>
            {
                new DrawCardEffect {Amount = 1},
                new PutSpellOnTopOfDeckEffect()
            }
        });

        _cards.Add(new UnitCardData
        {
            Name = "Augur of Bolas",
            ManaCost = "1U",
            Colors = new List<CardColor> { CardColor.Blue },
            Power = 1,
            Toughness = 3,
            Abilities = new List<CardAbility>
            {
                new TriggeredAbility{
                TriggerType = TriggerType.SelfEntersPlay,
                Effects = new List<Effect>{
                new GrabFromTopOfDeckEffect
                {
                    CardsToLookAt = 3,
                    Amount = 1,
                    Filter = new CardFilter
                    {
                       CardType = "Spell"
                    }
                }
                }
                }
            }
        });

        _cards.Add(new SpellCardData
        {
            Name = "Vapor Snag",
            ManaCost = "U",
            Colors = new List<CardColor> { CardColor.Blue },
            Effects = new List<Effect>
            {
                new BounceUnitEffect
                {
                    TargetType = TargetType.TargetUnits
                },
                new DamageEffect
                {
                    Amount = 1,
                    TargetType = TargetType.Opponent
                }
            }
        });

        _cards.Add(new SpellCardData
        {
            Name = "Thought Scour",
            ManaCost = "U",
            Colors = new List<CardColor> { CardColor.Blue },
            Effects = new List<Effect>
            {
                new MillEffect
                {
                    Amount = 2,
                    TargetType = TargetType.Self
                },
                new DrawCardEffect
                {
                    Amount = 1,
                    TargetType = TargetType.Self
                }
            }
        });

        _cards.Add(new SpellCardData
        {
            Name = "Gut Shot",
            ManaCost = "0",
            AdditionalCost = new PayLifeAdditionalCost
            {
                Amount = 2
            },
            Colors = new List<CardColor> { CardColor.Red },
            Effects = new List<Effect>
            {
                new DamageEffect {TargetType = TargetType.TargetUnitsOrPlayers, Amount = 1}
            }
        });

        _cards.Add(new SpellCardData
        {
            Name = "Mana Leak",
            ManaCost = "1U",
            Colors = new List<CardColor> { CardColor.Blue },
            Abilities = new List<CardAbility>
            {
                new RespondToCastAbility
                {
                }
            }
        });

        _cards.Add(new ItemCardData
        {
            Name = "Runechanter's Pike",
            ManaCost = "2",
            Subtype = "Artifact",
            Colors = new List<CardColor> { },
            Abilities = new List<CardAbility>
            {
                new ActivatedAbility
                {
                    ManaCost = "2",
                    OncePerTurn = true,
                    Effects = new List<Effect>
                    {
                        new PumpPowerByNumberOfSpellsInGraveyardEffect
                        {
                              TargetType = TargetType.TargetUnits
                        }
                    }
                }
            }
        });


        _cards.Add(new ManaCardData
        {
            Name = "Seachrome Coast",
            ManaAdded = "1WU",
            ReadyImmediately = false,
            ReadyCondition = new LessThan3ManaReadyCondition() { },
            Colors = new List<CardColor> { CardColor.Blue, CardColor.White }
        });

        _cards.Add(new ManaCardData
        {
            Name = "Glacial Fortress",
            ManaAdded = "1WU",
            ReadyImmediately = false,
            ReadyCondition = new AlreadyHasManaCondition { ManaNeeded = "UW" },
            Colors = new List<CardColor> { CardColor.Blue, CardColor.White }
        });

        _cards.Add(new ManaCardData
        {
            Name = "Moorland Haunt",
            ManaAdded = "1",
            Abilities = new List<CardAbility>()
            {
                new TriggeredAbility()
                {
                    TriggerType = TriggerType.SelfEntersPlay,
                    Effects = new List<Effect>
                    {
                        new CreateTokenEffect<ItemCardData>
                        {
                            TokenData = new ItemCardData()
                            {
                                Name = "Moorland Haunt",
                                Subtype = "Artifact",
                                Abilities = new List<CardAbility>
                                {
                                    new ActivatedAbility()
                                    {
                                        OncePerTurn = true,
                                        ManaCost = "UW",
                                        AdditionalCost = new ExileRandomCreatureFromDiscardAdditionalCost(),
                                        Effects = new List<Effect>
                                        {
                                            new CreateTokenEffect<UnitCardData>
                                            {
                                                AmountOfTokens = 1,
                                                TokenData = new UnitCardData
                                                {
                                                    Name = "Spirit",
                                                    Power = 1,
                                                    Toughness = 1,
                                                    Abilities = new List<CardAbility>
                                                    {
                                                        new FlyingAbility()
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            AmountOfTokens = 1

                        }
                    }
                }
            }


        });

        _cards.Add(new ManaCardData
        {
            Name = "Stomping Ground",
            ManaAdded = "1RG",
            Colors = new List<CardColor> { CardColor.Green, CardColor.Red },
            Abilities = new List<CardAbility>
            {
                new TriggeredAbility()
                {
                    TriggerType = TriggerType.SelfEntersPlay,
                    Effects = new List<Effect>
                    {
                        new DamageEffect
                        {
                            Amount = 2,
                            TargetType = TargetType.Self
                        }
                    }
                }
            }
        });

        _cards.Add(new ManaCardData
        {
            Name = "Rootbound Crag",
            ManaAdded = "1RG",
            ReadyImmediately = false,
            ReadyCondition = new AlreadyHasManaCondition { ManaNeeded = "RG" },
            Colors = new List<CardColor> { CardColor.Green, CardColor.Red },
        });

        _cards.Add(new ManaCardData
        {
            Name = "Copperline Gorge",
            ManaAdded = "1RG",
            ReadyImmediately = false,
            Colors = new List<CardColor> { CardColor.Green, CardColor.Red },
            ReadyCondition = new LessThan3ManaReadyCondition() { },
        });

        _cards.Add(new ManaCardData
        {
            Name = "Valakut, The Molten Pinnacle",
            ManaAdded = "1",
            ReadyImmediately = false,
            Colors = new List<CardColor> { },
            Abilities = new List<CardAbility>
            {
                new TriggeredAbility
                {
                    TriggerType = TriggerType.SelfEntersPlay,
                    Effects = new List<Effect>
                    {
                        new CreateTokenEffect<ItemCardData>()
                        {
                            TokenData = new ItemCardData
                            {
                                Name = "Valakut Item",
                                Abilities = new List<CardAbility>
                                {
                                       new TriggeredAbility
                                        {
                                        TriggerType = TriggerType.SelfManaPlayed,
                                            Effects = new List<Effect>
                                            {
                                                new ValakutEffect()
                                                {
                                                TargetType = TargetType.RandomOpponentOrUnits
                                                }
                                            }
                                        }
                                }

                            }
                        }
                    }
                },

            }
        });

        _cards.Add(new SpellCardData
        {
            Name = "Rampant Growth",
            ManaCost = "1G",
            Colors = new List<CardColor> { CardColor.Green },
            Effects = new List<Effect>
            {
                new PutManaFromDeckIntoPlayEffect
                {
                    Amount = 1,
                    ForceEmpty = true
                }
            }
        });

        _cards.Add(new UnitCardData
        {
            Name = "Primeval Titan",
            ManaCost = "4GG",
            Colors = new List<CardColor> { CardColor.Green },
            Power = 6,
            Toughness = 6,
            Abilities = new List<CardAbility>
            {
                new TrampleAbility(),
                new TriggeredAbility
                {
                    TriggerType = TriggerType.SelfEntersPlay,
                    Effects = new List<Effect>
                    {
                      new PutManaFromDeckIntoPlayEffect
                        {
                            Amount = 2,
                            ForceEmpty = true
                        }
                    }
                },
                new TriggeredAbility
                {
                    TriggerType = TriggerType.SelfAttacks,
                    Effects = new List<Effect>
                    {
                      new PutManaFromDeckIntoPlayEffect
                        {
                            Amount = 2,
                            ForceEmpty = true
                        }
                    }
                }
            }
        });

        _cards.Add(new UnitCardData
        {
            Name = "Inferno Titan",
            ManaCost = "4RR",
            Colors = new List<CardColor> { CardColor.Red },
            Power = 6,
            Toughness = 6,
            Abilities = new List<CardAbility>
            {
                new ActivatedAbility
                {
                    ManaCost = "R",
                    Effects = new List<Effect>
                    {
                        new PumpUnitEffect
                        {
                            Power = 1,
                            Toughness =0,
                            TargetType = TargetType.UnitSelf
                        }
                    }
                },
                new TriggeredAbility
                {
                    TriggerType = TriggerType.SelfEntersPlay,
                    Effects = new List<Effect>
                    {
                        new DamageEffect
                        {
                            Amount = 3,
                            TargetType = TargetType.RandomOpponentOrUnits
                        }
                    }
                },
                new TriggeredAbility
                {
                    TriggerType = TriggerType.SelfAttacks,
                    Effects = new List<Effect>
                    {
                        new DamageEffect
                        {
                            Amount = 3,
                            TargetType = TargetType.RandomOpponentOrUnits
                        }
                    }
                }
            }
        });

        _cards.Add(new SpellCardData
        {
            Name = "Cultivate",
            ManaCost = "2G",
            Colors = new List<CardColor>() { CardColor.Green },
            Effects = new List<Effect>
            {
                new GetRandomCardFromDeckEffect()
                {
                    Filter = new CardFilter
                    {
                        CardType = "Mana"
                    },
                    Amount = 1
                },
                new PutManaFromDeckIntoPlayEffect()
                {
                    Amount = 1,
                    ForceEmpty = true
                }
            }
        });

        _cards.Add(new SpellCardData
        {
            Name = "Harrow",
            ManaCost = "2G",
            AdditionalCost = new SacrificeManaAdditionalCost
            {
                Amount = 1
            },
            Colors = new List<CardColor> { CardColor.Green },
            Effects = new List<Effect>
            {
                new PutManaFromDeckIntoPlayEffect
                {
                    Amount = 2,
                    ForceEmpty = false
                }
            }
        });

        _cards.Add(new SpellCardData
        {
            Name = "Explore",
            ManaCost = "1G",
            Colors = new List<CardColor> { CardColor.Green },
            Effects = new List<Effect>
            {
                new PlayAdditionalLandEffect
                {
                    Amount = 1,
                    OneTurnOnly = true
                },
                new DrawCardEffect
                {
                    Amount = 1
                }
            }
        });

        _cards.Add(new UnitCardData
        {
            Name = "Oracle of Mul Daya",
            ManaCost = "3G",
            Colors = new List<CardColor> { CardColor.Green },
            Power = 2,
            Toughness = 2,
            Abilities = new List<CardAbility>
            {
                new StaticAbility
                {
                    Effects = new List<Effect>
                    {
                       new StaticPlayAdditionalLandEffect
                       {
                            Amount = 1,
                            TargetType = TargetType.Self
                       }
                    }
                },
                new StaticAbility
                {
                    Effects = new List<Effect>
                    {
                        new OracleOfMulDayaEffect()
                        {
                            TargetType = TargetType.Self
                        }
                    }
                },
                new StaticAbility
                {
                    Effects = new List<Effect>
                    {
                        new StaticRevealTopCardEffect
                        {
                            TargetType = TargetType.Self
                        }
                    }
                }
            }
        });

        _cards.Add(new SpellCardData
        {
            Name = "Summoning Trap",
            ManaCost = "4GG",
            Colors = new List<CardColor> { CardColor.Green },
            Abilities = new List<CardAbility>
            {
                new RespondToOpponentEndOfTurnAbility()
            },
            Effects = new List<Effect>
            {
                new PutUnitsFromTopOfDeckIntoPlay()
                {
                    CardsToLookAt = 7,
                    Amount = 1,
                }
            }
        });

        _cards.Add(new SpellCardData
        {
            Name = "Collected Company",
            ManaCost = "3G",
            Colors = new List<CardColor> { CardColor.Green },
            Abilities = new List<CardAbility>
            {
               new RespondToOpponentEndOfTurnAbility()
            },
            Effects = new List<Effect>
            {
                new PutUnitsFromTopOfDeckIntoPlay
                {
                    CardsToLookAt = 6,
                    Amount = 2,
                    Filter = new CardFilter
                    {
                        ManaCheck = new LessThanManaFilter
                        {
                            Amount = 4
                        }
                    }
                }
            }
        });




        //Affinity Deck

        /*
         * Creature (24)
        4 Ornithopter
        4 Arcbound Ravager
        4 Arcbound Worker
        4 Disciple of the Vault
        2 Somber Hoverguard
        4 Frogmite
        4 Chrome Mox
        4 Thoughtcast
        4 Welding Jar
        4 Shrapnel Blast
        4 Cranial Plating
        4 Seat of the Synod
        4 Vault of Whispers
        4 Great Furnace
        3 Blinkmoth Nexus
        3 Glimmervoid
        Cards 60
         * 
         */



        //Sacrifice a Unit,
        //Pay
        //Discard
        //Exile

        /*  
          _cards.Add(new UnitCardData()
          {
              Name = "Fume Spitter",
              ManaCost = "B",
              Power = 1,
              Toughness = 1,
              ArtPath = "CardArt/Fume Spitter",
              Colors = new List<CardColor> { CardColor.Black },
              Abilities = new List<CardAbility>
              {
                  new ActivatedAbility(){
                  ManaCost = "0",
                  OtherCost = "Sacrifice #this#",
                  AbilityEffect = new PumpUnitEffect
                  {
                      Power = -1,
                      Toughness = -1,
                      TargetType = TargetType.TargetUnits
                  }
          });*/

        //need to create temp ability effects.

        _cards.Add(new UnitCardData()
        {
            Name = "Crimson Mage",
            ManaCost = "1R",
            Power = 2,
            Toughness = 1,
            ArtPath = "CardArt/CrimsonMage",
            Colors = new List<CardColor> { CardColor.Red },
            Abilities = new List<CardAbility>
            {
                new ActivatedAbility()
                {
                    ManaCost = "R",
                    Effects = new List<Effect>{new AddTempAbilityEffect(new HasteAbility()) }
                }
            }
        });

        _cards.Add(new UnitCardData()
        {
            Name = "Masticore",
            ManaCost = "4",
            Power = 4,
            Toughness = 4,
            ArtPath = "CardArt/Masticore",
            Colors = new List<CardColor> { CardColor.Colorless },
            Abilities = new List<CardAbility>
            {
                new ActivatedAbility()
                {
                  ManaCost = "2",
                  Effects = new List<Effect>{ new DamageEffect
                  {
                      TargetType = TargetType.TargetUnits,
                      Amount = 1
                  }
                  }
                }
            }
        });

        /*
        _cards.Add(new UnitCardData()
        {
            Name = "Doomed Traveler",
            ManaCost = "1"
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
