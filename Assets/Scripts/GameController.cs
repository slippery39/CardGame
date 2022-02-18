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
        UpdateLane(_player1Lanes, _gameState.Player1Lane);
        UpdateLane(_player2Lanes, _gameState.Player2Lane);
        _player1HealthText.text = _gameState.Player1Health.ToString();
        _player2HealthText.text = _gameState.Player2Health.ToString();
    }
    private void UpdateLane(Transform laneInScene, List<CardInstance> cardsInLane)
    {
        var uiCards = laneInScene.GetComponentsInChildren<UICard>(true);
        for (int i = 0; i < cardsInLane.Count; i++)
        {
            Debug.Log(i);
            //If there is no card in the game state for a lane, just hide the card.
            if (cardsInLane[i] == null)
            {
                uiCards[i].gameObject.SetActive(false);
                continue;
            }
            else
            {
                uiCards[i].gameObject.SetActive(true);
            }

            uiCards[i].GetComponent<UICard>().SetFromCardData(cardsInLane[i].CurrentCardData);
        }
    }
    #endregion
}

