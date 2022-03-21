using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [SerializeField]
    private GameObject _uiCardTemplate;
    [SerializeField]
    private CardGame _cardGame;
    [SerializeField]
    private Transform _player1Lanes;
    [SerializeField]
    private Transform _player2Lanes;
    [SerializeField]
    private Transform _player1Hand;
    [SerializeField]
    private TextMeshPro _player1HealthText;
    [SerializeField]
    private TextMeshPro _player2HealthText;
    [SerializeField]
    private TextMeshPro _turnIndicator;

    [SerializeField]
    private TextMeshPro _player1Mana;
    [SerializeField]
    private TextMeshPro _player2Mana;


    [SerializeField]
    private TextMeshPro _actionStateIndicator;


    [SerializeField]
    private GameUIStateMachine _stateMachine;


    void Start()
    {
        _cardGame = new CardGame();
        _stateMachine = new GameUIStateMachine(_cardGame);
        UpdateBoard();
    }

    private void Update()
    {
        //Test Hotkey for testing our Battle System.
        if (Input.GetKeyDown(KeyCode.B))
        {
            _cardGame.BattleSystem.ExecuteBattles(_cardGame);
        }
        //Testing card drawing
        if (Input.GetKeyDown(KeyCode.D))
        {
            _cardGame.CardDrawSystem.DrawCard(_cardGame, _cardGame.Player1);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            _cardGame.ManaSystem.AddMana(_cardGame, _cardGame.Player1, 1);
        }

        _stateMachine.CurrentState.HandleInput();

        UpdateBoard();
    }

    #region Private Methods

    private void UpdateBoard()
    {
        _actionStateIndicator.text = _stateMachine.GetMessage();
        UpdateLanes(_player1Lanes, _cardGame.Player1.Lanes);
        UpdateLanes(_player2Lanes, _cardGame.Player2.Lanes);
        UpdateHand(_player1Hand, _cardGame.Player1.Hand);
        UpdateMana();
        _player1HealthText.text = $"Player 1 Health : {_cardGame.Player1.Health}";
        _player2HealthText.text = $"Player 2 Health : {_cardGame.Player2.Health}";
        _turnIndicator.text = $"Player {_cardGame.ActivePlayerId}'s Turn";
    }
    private void UpdateLanes(Transform laneInScene, List<Lane> lanes)
    {
        var uiCards = laneInScene.GetComponentsInChildren<UICard>(true);
        for (int i = 0; i < lanes.Count; i++)
        {
            //If there is no card in the game state for a lane, just hide the card.
            if (lanes[i].IsEmpty())
            {
                uiCards[i].gameObject.SetActive(false);
                continue;
            }
            else
            {
                uiCards[i].gameObject.SetActive(true);
            }

            uiCards[i].GetComponent<UICard>().SetFromCardData(lanes[i].UnitInLane.CurrentCardData);
        }
    }

    private void UpdateHand(Transform handInScene, Hand hand)
    {
        var uiCards = handInScene.GetComponentsInChildren<UICard>(true);
        for (int i = 0; i < uiCards.Length; i++)
        {
            //If there is no card in the game state for a lane, just hide the card.
            if (hand.Cards.Count <= i || hand.Cards[i] == null)
            {
                uiCards[i].gameObject.SetActive(false);
                continue;
            }
            else
            {
                uiCards[i].gameObject.SetActive(true);
            }
            uiCards[i].GetComponent<UICard>().SetFromCardData(hand.Cards[i].CurrentCardData);
        }
    }
    private void UpdateMana()
    {
        _player1Mana.text = "Player 1 Mana : " + _cardGame.Player1.Mana;
        _player2Mana.text = "Player 2 Mana : " + _cardGame.Player2.Mana;
    }
    #endregion
}

