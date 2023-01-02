using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TMP_Dropdown;

public class DeckSelectionScreen : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown _player1DeckSelect;
    [SerializeField]
    private TMP_Dropdown _player2DeckSelect;

    [SerializeField]
    private Button _button;

    private void Awake()
    {
        _button.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        Debug.Log("Game is starting...");

        var deckDB = new FamousDecks();
        var player1Deck = deckDB.GetByName(_player1DeckSelect.options[_player1DeckSelect.value].text);
        var player2Deck = deckDB.GetByName(_player2DeckSelect.options[_player2DeckSelect.value].text);

        Debug.Log("Player Deck Debug..");
        Debug.Log(_player1DeckSelect.options[_player1DeckSelect.value].text);
        Debug.Log(_player2DeckSelect.options[_player2DeckSelect.value].text);
        Debug.Log(player1Deck);
        Debug.Log(player2Deck);

        UIGameController.Instance.StartGame(player1Deck, player2Deck);
    }
}
