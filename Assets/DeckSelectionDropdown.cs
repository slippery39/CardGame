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
    private void Awake()
    {
        var decks = new FamousDecks().GetAll();
        List<OptionData> deckOptions = decks.Select(d => new OptionData { text = d.Name }).ToList();
        GetComponent<TMP_Dropdown>().AddOptions(deckOptions);
    }
}
