using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TMP_Dropdown;

public class DeckSelectionScreen : MonoBehaviour
{
    private List<Decklist> _decklists = new List<Decklist>();
    // Start is called before the first frame update
    [SerializeField]
    private TMP_Dropdown _player1DeckSelect;
    [SerializeField]
    private TMP_Dropdown _player2DeckSelect;

    [SerializeField]
    private Button _button;

    [SerializeField]
    private GameObject _gameRoom;


    private void Awake()
    {
        _decklists.AddRange(new FamousDecks().GetAll());
        _button.onClick.AddListener(StartGame);

        List<OptionData> deckOptions = _decklists.Select(d => new OptionData { text = d.Name }).ToList();

        Debug.Log(deckOptions.Count);
        _player1DeckSelect.AddOptions(deckOptions);
        _player2DeckSelect.AddOptions(deckOptions);
    }


    private void StartGame()
    {
        gameObject.SetActive(false);
        _gameRoom.SetActive(true);
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

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
