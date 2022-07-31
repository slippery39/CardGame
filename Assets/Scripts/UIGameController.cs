using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIGameController : MonoBehaviour
{
    [SerializeField]
    private GameObject _uiCardTemplate;

    [SerializeField]
    private CardGame _cardGame;

    [SerializeField]
    private TextMeshPro _turnIndicator;

    [SerializeField]
    private TextMeshPro _actionStateIndicator;

    [SerializeField]
    private GameUIStateMachine _stateMachine;

    [SerializeField]
    private UIPlayerBoard _player1Board;

    [SerializeField]
    private UIPlayerBoard _player2Board;

    [SerializeField]
    private UICard _cardPopup;

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
        //Check to see if any cards exist that don't have images.
        CheckForCardsWithoutImages();
        CheckForCardsWithoutManaCosts();

        _stateMachine = GetComponent<GameUIStateMachine>();

        var player1HideStuff = _cardGame.ActivePlayer != _cardGame.Player1;
        var player2HideStuff = _cardGame.ActivePlayer != _cardGame.Player2;
        _player1Board.SetPlayer(_cardGame.Player1, player1HideStuff);
        _player2Board.SetPlayer(_cardGame.Player2, player2HideStuff);

        UpdateUI();
    }

    private void Update()
    {

        if (_cardGame.CurrentGameState == GameState.WaitingForAction)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _cardGame.BattleSystem.Battle(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _cardGame.BattleSystem.Battle(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _cardGame.BattleSystem.Battle(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _cardGame.BattleSystem.Battle(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                _cardGame.BattleSystem.Battle(4);
            }

            //Test Hotkey for testing our Battle System.
            if (Input.GetKeyDown(KeyCode.B))
            {
                _cardGame.NextTurn();
                _stateMachine.ToIdle();
            }
            //Testing card drawing
            if (Input.GetKeyDown(KeyCode.D))
            {
                _cardGame.CardDrawSystem.DrawCard(_cardGame.ActivePlayer);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                _cardGame.ManaSystem.AddMana(_cardGame.ActivePlayer, "1*");
            }

            _stateMachine.CurrentState.HandleInput();

        }
        else if (_cardGame.CurrentGameState == GameState.WaitingForChoice)
        {
            //Temporary Hack to make it not automatically happen.
            if (!(_stateMachine.CurrentState is GameUIDiscardAsPartOfSpellState))
            {
                _stateMachine.ChangeState(new GameUIDiscardAsPartOfSpellState(_stateMachine, _cardGame.ChoiceInfoNeeded as DiscardCardEffect));
            }
        }

        UpdateUI();
        _stateMachine.CurrentState.OnUpdate();
    }

    public IEnumerable<UIGameEntity> GetUIEntities()
    {
        var entities = new List<UIGameEntity>();

        entities.AddRange(_player1Board.GetUIEntities());
        entities.AddRange(_player2Board.GetUIEntities());

        return entities;
    }

    public void HandleClick(UIGameControllerClickEvent clickInfo)
    {
        //Do something here?
        _stateMachine.HandleSelection(clickInfo.EntityId);
    }

    public void HandlePointerEnter(int entityId)
    {
        var entity = _cardGame.GetEntities<CardInstance>().Where(c => c.EntityId == entityId).FirstOrDefault();
        if (entity == null)
        {
            return;
        }

        _cardPopup.SetCardData(entity);
    }

    #region Private Methods

    private void UpdateUI()
    {
        _actionStateIndicator.text = _stateMachine.GetMessage();
        _turnIndicator.text = $"Player {_cardGame.ActivePlayerId}'s Turn ({_cardGame.TurnSystem.TurnId})";

        _player1Board.HideHiddenInfo = _cardGame.ActivePlayer != _cardGame.Player1;
        _player2Board.HideHiddenInfo = _cardGame.ActivePlayer != _cardGame.Player2;
    }

    //TEST METHODS
    private void CheckForCardsWithoutImages()
    {
        var allCards = new CardDatabase().GetAll();
        foreach (var card in allCards)
        {
            Sprite art = Resources.Load<Sprite>(card.ArtPath);

            if (art == null)
            {
                Debug.LogWarning($@"Warning: Could not find card art for {card.Name}");
            }
        }
    }

    private void CheckForCardsWithoutManaCosts()
    {
        var allCards = new CardDatabase().GetAll();
        foreach (var card in allCards)
        {
            if (card.ManaCost == null)
            {
                Debug.LogWarning($@"Warning: Mana cost is null for {card.Name}");
            }
        }
    }

    #endregion
}

