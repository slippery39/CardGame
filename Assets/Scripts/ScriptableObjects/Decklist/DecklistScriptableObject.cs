using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Decklist", menuName = "ScriptableObjects/DecklistScriptableObject", order = 1)]
public class DecklistScriptableObject : ScriptableObject
{
    public string DeckName;
    public string Format;
    [TextArea(10,30)] public string Contents; //string so we can easily copy / paste

    public Decklist ToDecklist()
    {
        var decklist = new Decklist
        {
            Name = DeckName,
            Format = Format,
            Cards = Contents
        };
        return decklist;
    }
}
