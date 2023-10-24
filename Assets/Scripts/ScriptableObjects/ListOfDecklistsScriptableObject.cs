using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ListOfDecklists", menuName = "ScriptableObjects/ListOfDecklistsScriptableObject", order = 2)]
public class ListOfDecklistsScriptableObject : ScriptableObject
{
    public string ListName = "";
    public List<DecklistScriptableObject> Decklists = new List<DecklistScriptableObject>();
}
