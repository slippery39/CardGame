using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [SerializeField]
    private GameObject _uiCardTemplate;
    [SerializeField]
    private CardGame _gameState;
    [SerializeField]
    private Transform _player1Lanes;
    [SerializeField]
    private Transform _player2Lanes;
    [SerializeField]
    private TextMeshPro _player1HealthText;
    [SerializeField]
    private TextMeshPro _player2HealthText;
    [SerializeField]
    private TextMeshPro _turnIndicator;

    void Start()
    {
        _gameState = new CardGame();
        UpdateBoard();
    }

    private void Update()
    {
        //Test Hotkey for testing our Battle System.
        if (Input.GetKeyDown(KeyCode.B))
        {
            _gameState.BattleSystem.ExecuteBattles(_gameState);
            UpdateBoard();
        }
    }

    #region Private Methods

    private void UpdateBoard()
    {
        UpdateLanes(_player1Lanes, _gameState.Player1.Lanes);
        UpdateLanes(_player2Lanes, _gameState.Player2.Lanes);
        _player1HealthText.text = $"Player 1 Health : {_gameState.Player1.Health}";
        _player2HealthText.text = $"Player 2 Health : {_gameState.Player2.Health}";
        _turnIndicator.text = $"Player {_gameState.ActivePlayerId}'s Turn";
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
    #endregion
}

