using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimulationScreen : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown _player1DeckSelect;
    [SerializeField]
    private TMP_Dropdown _player2DeckSelect;
    [SerializeField]
    private SimGameController _simGameControler;

    public void SimulateGame()
    {
        Debug.Log("Game is simulating...");

        var deckDB = new FamousDecks();
        var player1Deck = deckDB.GetByName(_player1DeckSelect.options[_player1DeckSelect.value].text);
        var player2Deck = deckDB.GetByName(_player2DeckSelect.options[_player2DeckSelect.value].text);

        _simGameControler.StartSimulateGame(player1Deck, player2Deck);
    }
}
