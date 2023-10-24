using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TMP_Dropdown;

[RequireComponent(typeof(TMP_Dropdown))]
public class DeckSelectionDropdown : MonoBehaviour
{
 
    public ListOfDecklistsScriptableObject Decklists;
    public DecklistScriptableObject Selected => Decklists.Decklists.First(d => d.DeckName == _dropdown.options[_dropdown.value].text);

    private TMP_Dropdown _dropdown;

    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        if (Decklists == null)
        {
            Debug.LogError($"No decklists set for DeckSelectionDropDown : ${this.name} ", this);
        }

        List<OptionData> deckOptions = Decklists.Decklists.Select(d => new OptionData { text = d.DeckName }).ToList();
        GetComponent<TMP_Dropdown>().AddOptions(deckOptions);
    }
}
