using Assets.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Fake Unit Tests
public static class Tests
{
    public static bool Test_CloningReferencesAreCorrect()
    {
        var result = false;

        //Create a game with 2 Affinity Decks
        //Start The Game
        //Clone the Game

        //Check to see if any references match

        //For entities we can check based on the Entity Id.

        //Also need to check other things
        //Abilities, Effects, Components, Counters, Etc..
        var decklistDB = new FamousDecks();
        var testGame = new CardGame();
        testGame.Setup(
            new GameSetupOptions
            {
                Player1Deck = decklistDB.GetByName("Affinity AI TEST - 2004"),
                Player2Deck = decklistDB.GetByName("Affinity AI TEST - 2004"),
                StartingLifeTotal = 20
            }
            );
        testGame.StartGame();

        var testCopy = testGame.Copy();

        //Create a list of references that we want to check

        List<dynamic> originalRefs = testGame.Players.SelectMany(p => p.GetAllEntities()).Cast<dynamic>().ToList();
        List<dynamic> copyRefs = testCopy.Players.SelectMany(p => p.GetAllEntities()).Cast<dynamic>().ToList();

        originalRefs.AddRange(originalRefs.GetOfType<CardInstance>().Select(c => c.Abilities));
        originalRefs.AddRange(originalRefs.GetOfType<CardInstance>().Select(c => c.Modifications));
        originalRefs.AddRange(originalRefs.GetOfType<CardInstance>().Select(c => c.Counters));
        originalRefs.AddRange(originalRefs.GetOfType<CardInstance>().Select(c => c.ContinuousEffects));

        originalRefs.AddRange(originalRefs.GetOfType<Player>().Select(p => p.ManaPool));


        copyRefs.AddRange(copyRefs.GetOfType<CardInstance>().Select(c => c.Abilities));
        copyRefs.AddRange(copyRefs.GetOfType<CardInstance>().Select(c => c.Modifications));
        copyRefs.AddRange(copyRefs.GetOfType<CardInstance>().Select(c => c.Counters));
        copyRefs.AddRange(copyRefs.GetOfType<CardInstance>().Select(c => c.ContinuousEffects));

        copyRefs.AddRange(copyRefs.GetOfType<Player>().Select(p => p.ManaPool));


        var interSections = originalRefs.Intersect(copyRefs).ToList();

        if (interSections.Any())
        {
            var abilities = interSections.GetOfType<List<CardAbility>>();
            var mods = interSections.GetOfType<List<Modification>>();
            var counter = interSections.GetOfType<List<Counter>>();
            var continuousEffects = interSections.GetOfType<List<ContinuousEffect>>();
            Debug.LogError($" Warning : The CardGame.Copy() method has detected {interSections.Count()} references which were not properly cloned");

        }
        else
        {
            Debug.Log("Success! Test Passed!");
        }

        return result;
    }
}
