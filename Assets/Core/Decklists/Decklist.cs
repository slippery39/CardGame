﻿using System;
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
