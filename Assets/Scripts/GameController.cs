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
    private bool _choosingLaneToSummon = false;
    [SerializeField]
    private CardInstance _unitChosenToSummon = null;

    [SerializeField]
    private bool _choosingSpellToCast = false;
    private CardInstance _spellChosenToCast = null;

    void Start()
    {
        _cardGame = new CardGame();
        UpdateBoard();
    }

    private void Update()
    {
        //Test Hotkey for testing our Battle System.
        if (Input.GetKeyDown(KeyCode.B))
        {
            _cardGame.BattleSystem.ExecuteBattles(_cardGame);
            UpdateBoard();
        }

        //Testing card drawing
        if (Input.GetKeyDown(KeyCode.D))
        {
            _cardGame.CardDrawSystem.DrawCard(_cardGame, _cardGame.Player1);
            UpdateBoard();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            _cardGame.ManaSystem.AddMana(_cardGame, _cardGame.Player1, 1);
            UpdateBoard();
        }

        if (_choosingLaneToSummon == true)
        {
            HandleSummonUnitInput();
        }
        else if (_choosingSpellToCast == true)
        {
            HandleCastSpellInput();
        }
        else
        {
            HandlePlayingCardFromHandInput();
        }

    

    }

    #region Private Methods

    private void UpdateBoard()
    {
        if (_choosingLaneToSummon)
        {
            _actionStateIndicator.text = $"Choose a lane to summon {_unitChosenToSummon.Name} to (use numeric keys)";
        }
        else if (_choosingSpellToCast)
        {
            _actionStateIndicator.text = $"Choose a target for spell {_spellChosenToCast.Name} to (use numeric keys)";

        }
        else
        {
            _actionStateIndicator.text = $"Play a card from your hand (using numeric keys) or press B to fight";
        }
        UpdateLanes(_player1Lanes, _cardGame.Player1.Lanes);
        UpdateLanes(_player2Lanes, _cardGame.Player2.Lanes);
        UpdateHand(_player1Hand,_cardGame.Player1.Hand);
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

    private void HandleSummonUnitInput()
    {
        var keys = new List<KeyCode>()
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5
        };

        var laneIds = _cardGame.Player1.Lanes.Select(x => x.EntityId).ToList();
        

        //Undo the action.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _choosingLaneToSummon = false;
            _unitChosenToSummon = null;
            return;
        }

        for (int i = 0; i < keys.Count; i++)
        {
            KeyCode keyCode = keys[i];
            if (Input.GetKeyDown(keyCode))
            {
                _cardGame.PlayCardFromHand(_cardGame.Player1, _unitChosenToSummon,laneIds[i]);
                _choosingLaneToSummon = false;
                _unitChosenToSummon = null;
                UpdateBoard();
            }
        }
    }

    private void HandleCastSpellInput()
    {
        //Keys for each lane + 2 players.
        var keys = new List<KeyCode>()
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
            KeyCode.Q,
            KeyCode.W
        };


        //Undo the action.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _choosingSpellToCast = false;
            _spellChosenToCast = null;
            return;
        }
        var validTargets = _cardGame.TargetSystem.GetValidTargets(_cardGame, _cardGame.Player1, _spellChosenToCast);

        for (int i = 0; i < validTargets.Count; i++)
        {
            KeyCode keyCode = keys[i];
            if (Input.GetKeyDown(keyCode))
            {
                _cardGame.PlayCardFromHand(_cardGame.Player1, _spellChosenToCast,validTargets[i].EntityId);
                _choosingSpellToCast = false;
                _spellChosenToCast = null;
                UpdateBoard();
            }
        }
    }

    private void HandlePlayingCardFromHandInput()
    {
        var keys = new List<KeyCode>()
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9
        };

        var player1 = _cardGame.Player1;

        //Testing Keys for Casting our Spells.
        for (int i = 0; i < keys.Count; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                var card = _cardGame.Player1.Hand.Cards[i];

                if (player1.Hand.Cards.Count > i)
                {
                    if (card.CurrentCardData is UnitCardData)
                    {
                        _choosingLaneToSummon = true;
                        _unitChosenToSummon = card;
                        UpdateBoard();
                    }
                    else
                    {
                        //TODO - need to update this.
                        if (_cardGame.TargetSystem.SpellNeedsTargets(_cardGame, player1, card))
                        {
                            _choosingSpellToCast = true;
                            _spellChosenToCast = card;
                        }
                        else
                        {
                            _cardGame.PlayCardFromHand(player1, card, 0);
                        }
                        UpdateBoard();
                    }
                }
            }
        }
    }
    
    private void UpdateMana()
    {
        _player1Mana.text = "Player 1 Mana : " + _cardGame.Player1.Mana;
        _player2Mana.text = "Player 2 Mana : " + _cardGame.Player2.Mana;
    }
    #endregion
}

