using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIGameController : MonoBehaviour
{
    [SerializeField]
    private UIPlayerAvatar _player1Avatar;

    [SerializeField]
    private UIPlayerAvatar _player2Avatar;

    [SerializeField]
    private GameObject _uiCardTemplate;
    [SerializeField]
    private CardGame _cardGame;
    [SerializeField]
    private Transform _player1Lanes;
    [SerializeField]
    private Transform _player2Lanes;
    [SerializeField]
    private Transform _playerHand;
    [SerializeField]
    private TextMeshPro _turnIndicator;

    [SerializeField]
    private TextMeshPro _actionStateIndicator;


    [SerializeField]
    private GameUIStateMachine _stateMachine;

    public CardGame CardGame { get => _cardGame; set => _cardGame = value; }

    //Singleton Pattern, should only be one game controller per unity scene.
    public static UIGameController Instance;


    private void Awake()
    {
        Instance = this;
        _cardGame = new CardGame();
    }

    void Start()
    {
        _stateMachine = GetComponent<GameUIStateMachine>();
        InitializeBoard();
        UpdateBoard();
    }

    private void Update()
    {
        //Test Hotkey for testing our Battle System.
        if (Input.GetKeyDown(KeyCode.B))
        {
            _cardGame.NextTurn();
            _stateMachine.ToIdle();
        }
        //Testing card drawing
        if (Input.GetKeyDown(KeyCode.D))
        {
            _cardGame.CardDrawSystem.DrawCard(_cardGame, _cardGame.ActivePlayer);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            _cardGame.ManaSystem.AddMana(_cardGame, _cardGame.ActivePlayer, 1);
        }
        _stateMachine.CurrentState.HandleInput();
        UpdateBoard();
    }

    public IEnumerable<UILane> GetUILanes()
    {
        return _player1Lanes.GetComponentsInChildren<UILane>(true).Concat(_player2Lanes.GetComponentsInChildren<UILane>(true));
    }

    public IEnumerable<UICard> GetUICardsInLane()
    {
        return _player1Lanes.GetComponentsInChildren<UICard>(true).Concat(_player2Lanes.GetComponentsInChildren<UICard>(true));
    }

    public IEnumerable<UIGameEntity> GetUIEntities()
    {
        var entities = new List<UIGameEntity>();

        entities.Add(_player1Avatar);
        entities.Add(_player2Avatar);
        entities.AddRange(GetUILanes());
        entities.AddRange(GetUICardsInLane());

        return entities;
    }

    public void HandleClick(UIGameControllerClickEvent clickInfo)
    {
        //Do something here?
        _stateMachine.HandleSelection(clickInfo.EntityId);
    }

    #region Private Methods

    private void InitializeBoard()
    {
        //Map the lanes on the board to lanes in the game.
        var uiLanesPlayer1 = _player1Lanes.GetComponentsInChildren<UILane>(true);
        for (int i = 0; i < uiLanesPlayer1.Length; i++)
        {
            uiLanesPlayer1[i].EntityId = CardGame.Player1.Lanes[i].EntityId;
        }

        var uiLanesPlayer2 = _player2Lanes.GetComponentsInChildren<UILane>(true);
        for(int i = 0; i < uiLanesPlayer2.Length; i++)
        {
            uiLanesPlayer2[i].EntityId = CardGame.Player2.Lanes[i].EntityId;
        }

        //Initialize the Player Avatars entity ids
        _player1Avatar.EntityId = CardGame.Player1.EntityId;
        _player2Avatar.EntityId = CardGame.Player2.EntityId;

        //Cards in lane entity ids are initialized via the UICard Monobehaviour.
    }

    private void UpdateBoard()
    {
        _actionStateIndicator.text = _stateMachine.GetMessage();
        UpdateLanes(_player1Lanes, _cardGame.Player1.Lanes);
        UpdateLanes(_player2Lanes, _cardGame.Player2.Lanes);
        UpdateHand(_playerHand, _cardGame.ActivePlayer.Hand);
        UpdateMana();

        _player1Avatar.SetHealth(_cardGame.Player1.Health);
        _player2Avatar.SetHealth(_cardGame.Player2.Health);

        _turnIndicator.text = $"Player {_cardGame.ActivePlayerId}'s Turn ({_cardGame.TurnSystem.TurnId})";
    }
    private void UpdateLanes(Transform laneInScene, List<Lane> lanes)
    {
        var uiLanes = laneInScene.GetComponentsInChildren<UILane>(true);
        for (int i = 0; i < lanes.Count; i++)
        {
            //If there is no card in the game state for a lane, just hide the card.
            if (lanes[i].IsEmpty())
            {
                uiLanes[i].SetEmpty();
                continue;
            }

            uiLanes[i].SetCard(lanes[i].UnitInLane);
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
            uiCards[i].GetComponent<UICard>().SetCardData(hand.Cards[i]);
        }
    }
    private void UpdateMana()
    {
        _player1Avatar.SetMana(_cardGame.Player1.Mana);
        _player2Avatar.SetMana(_cardGame.Player2.Mana);
    }
    #endregion
}

