using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Decklist
{
    public static string Affinity2004()
    {
        //Still need to implement Shrapnel Blast and Chrome Mox
        //City of brass is a replacement for Glimmervoid.
        return $@"
            4 Ornithopter
            4 Arcbound Ravager
            4 Arcbound Worker
            4 Disciple of the Vault
            2 Somber Hoverguard
            4 Frogmite
            4 Thoughtcast
            4 Shrapnel Blast
            4 Chrome Mox
            4 Welding Jar
            4 Cranial Plating
            4 Seat of the Synod
            4 Vault of Whispers
            4 Great Furnace
            3 Blinkmoth Nexus
            3 City of Brass
            ";
    }

    public static string UWDelver2012()
    {
        return $@"
        3 Restoration Angel
        4 Geist of Saint Traft
        4 Delver of Secrets
        4 Snapcaster Mage
        2 Augur of Bolas
        Sorcery(8)
        4 Ponder
        4 Gitaxian Probe
        Instant(14)
        4 Vapor Snag
        4 Thought Scour
        4 Gut Shot
        2 Mana Leak
        Artifact(2)
        2 Runechanter's Pike
        Land(19)
        8 Island
        4 Seachrome Coast
        4 Glacial Fortress
        2 Moorland Haunt
        1 Plains";
    }

    public static string RGValakut2011()
    {
        //Note we replaced Khalni Heart Expedition with Rampant Growth
        //Also replaced Evolving Wilds / Terramorphic Expanse / Verdant Catacombs with other Red Green Lands (Rootbound Crag / Stomping Ground)
        //Replaced Avenger of Zendikar with a 4th Inferno Titan
        //1 Raging Ravine replaced with copperline gorge
        return $@"Creature (10)
                        4 Inferno Titan
                        2 Oracle of Mul Daya
                        4 Primeval Titan
                        Sorcery(7)
                        3 Cultivate
                        4 Explore
                        Instant(11)
                        3 Harrow
                        4 Lightning Bolt
                        4 Summoning Trap
                        Enchantment(4)
                        4 Rampant Growth
                        Land(28)
                        4 Rootbound Crag
                        4 Stomping Ground
                        3 Copperline Gorge
                        2 Forest 
                        10 Mountain
                        4 Valakut, The Molten Pinnacle
                        ";
    }

    public static List<BaseCardData> ConvertToDeck(string decklist)
    {
        var deck = new List<BaseCardData>();
        var db = new CardDatabase();
        foreach (var line in decklist.Split("\r\n"))
        {
            var trimLine = line.Trim();
            if (trimLine == "")
            {
                continue;
            }

            int amount = 0;
            try
            {
                amount = Convert.ToInt32(trimLine.Split(' ')[0]);
            }
            catch
            {
                Debug.Log($"Invalid amount when trying to create deck list for line {trimLine}");
                continue;
            }

            var otherParts = trimLine.Split(' ').ToList().Skip(1).Take(trimLine.Split(' ').Length).ToList();

            var cardName = string.Join(" ", otherParts);

            for (var i = 0; i < amount; i++)
            {
                var card = db.GetCardData(cardName);

                if (card == null)
                {
                    Debug.Log($"Could not find card {cardName} when trying to create a deck");
                    continue;
                }
                deck.Add(db.GetCardData(cardName));
            }
        }
        return deck;
    }
}
