using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using System;

public class UI3DGameEventManager : MonoBehaviour
{
    [SerializeField] private UI3DController _uiController;
    [SerializeField] private GameService _gameService;
    [SerializeField] private Queue<GameEvent> _eventQueue = new Queue<GameEvent>();
    [SerializeField] private bool _isPlayingAnimation = false;
    //For debug purposes
    [SerializeField] private GameEvent _currentGameEvent;

    [Header("Animations")]
    [SerializeField]
    private FightAnimation _fightAnimation;
    [SerializeField]
    private DamageAnimation _damageAnimation;
    [SerializeField]
    private DrawCardAnimation _drawCardAnimation;
    //using a stack 3d for now
    [SerializeField]
    private PerformActionAnimation _performActionAnimation;
    //[SerializeField] private GameAnimation _currentAnimation;
    [SerializeField]
    private TurnStartAnimation _turnStartAnimation;

    [SerializeField]
    private GameOverAnimation _gameOverAnimation;

    [SerializeField]
    private float animationTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        _gameService.GetGameEventObservable().Subscribe(gameEvent =>
        {
            AddEventToQueue(gameEvent);
            //Add the event to our internal animation queue.
        });

        SetAnimationTimes();
    }

    // Update is called once per frame
    void Update()
    {
        //If the animation queue has an animation to be played && an animation is not currently playing
        //start playing that animation
        //once the animation is done, remove it from the queue
        //set the resulting state as well.


        if (_eventQueue.Count > 0 && _currentGameEvent == null)
        {
            HandleGameEvent();
        }
        //Update every frame in case it gets changed in the inspector.
        SetAnimationTimes();
    }

    public void Reset()
    {
        //TODO - Need to be able to cancel any current animation going on so that no errors occur
        this._eventQueue.Clear();
        _currentGameEvent = null;
        this._gameOverAnimation.gameObject.SetActive(false);
        this._turnStartAnimation.gameObject.SetActive(false);
    }


    void AddEventToQueue(GameEvent gameEvent)
    {
        _eventQueue.Enqueue(gameEvent);
    }

    void NextEvent()
    {
        _currentGameEvent = null;
    }

    void HandleGameEvent()
    {
        _isPlayingAnimation = true;
        if (_eventQueue.Count > 0)
        {
            _currentGameEvent = _eventQueue.Dequeue();
            if (_currentGameEvent is GameStateUpdatedEvent)
            {
                //Update the current game state
                var evt = _currentGameEvent as GameStateUpdatedEvent;
                _uiController.SetUIGameState(evt.ResultingState);
                NextEvent();
            }
            else if (_currentGameEvent is GameStartEvent)
            {
                var evt = _currentGameEvent as GameStartEvent;
                _turnStartAnimation.PlayTextAnimation("Game Start!", NextEvent);
            }
            else if (_currentGameEvent is GameEndEvent)
            {
                var evt = _currentGameEvent as GameEndEvent;                
                _gameOverAnimation.PlayAnimation($"Game Over!{Environment.NewLine}{evt.WinnerName} has won!", () => {
                    _uiController.ShowGameOverScreen();
                });
            }
            else if (_currentGameEvent is PlayCardEvent)
            {
                var evt = _currentGameEvent as PlayCardEvent;
                //Need a way to grab the card information from the card id
                var unit = GameObject.FindObjectsOfType<UIGameEntity3D>(true)
                .Where(ent => ent.EntityId == evt.CardId)
                .FirstOrDefault();
                var card3D = unit.GetComponent<Card3D>();

                _performActionAnimation.PlayAnimation(card3D, NextEvent);
            }
            else if (_currentGameEvent is TurnStartEvent)
            {
                var evt = _currentGameEvent as TurnStartEvent;
                _turnStartAnimation.PlayTextAnimation(evt.PlayerName + "'s turn", NextEvent);

            }
            else if (_currentGameEvent is UnitSummonedEvent)
            {
                var evt = _currentGameEvent as UnitSummonedEvent;
                var unit = GameObject.FindObjectsOfType<UIGameEntity3D>(true)
                            .FirstOrDefault(ent => ent.EntityId == evt.UnitId);

                if (unit == null)
                {
                    Debug.LogWarning("Could not find unit with entity id:" + evt.UnitId);
                    NextEvent();
                    return;
                }
                
                unit.gameObject.SetActive(true);
                var card3D = unit.GetComponent<Card3D>();

                var lane = GameObject.FindObjectsOfType<UIGameEntity3D>()
                      .FirstOrDefault(ent => ent.EntityId == evt.LaneId);

                var newCard = Instantiate(card3D);

                //Hide the card
                card3D.gameObject.SetActive(false);

                newCard.transform.position = lane.transform.position;
                newCard.transform.rotation = lane.transform.rotation;
                newCard.GetComponent<Card3D>().PlaySummon(() =>
                {
                    Destroy(newCard.gameObject);
                    NextEvent();
                });

            }
            else if (_currentGameEvent is UnitDiedEvent)
            {
                var evt = _currentGameEvent as UnitDiedEvent;

                var entity = GameObject.FindObjectsOfType<UIGameEntity3D>()
                            .FirstOrDefault(ent => ent.EntityId == evt.UnitId);

                if (entity == null)
                {
                    NextEvent();
                    return;
                }

                var card = entity.GetComponent<Card3D>();

                //TODO - Add animation time.
                card.PlayDissolve(NextEvent);

            }
            else if (_currentGameEvent is DrawCardEvent)
            {
                var evt = _currentGameEvent as DrawCardEvent;
                /*
                 * Need to find the board which corresponds to the id of the player drawing a card
                 * From there we can find its "Deck" and "Hand" game objects
                 * And run the draw card animation.
                 */
                var playerBoard = _uiController.GetPlayerBoards().FirstOrDefault(pb => pb.PlayerId == evt.PlayerId);

                /*
                 * How do we get the drawn card?
                 */
                //We might have issues below if we grab it directly from the game state... maybe we should pass in the current card state in to the
                //event? Although the way things are set up we might have some issues. 

                //Another possiblity is to save the important parts of the card in the event itself, such as the type, name rules text etc.. and then
                //pass it into the card raw animation through the Card3DOptions interface. 
                //since our card draw animation creates a temporary copy anyways, it probably doesn't matter too much if it isn't 100% accurate 
                //for the 0.5 to 1 second time the animation is running.

                var cardInstanceDrawn = _uiController.GetComponent<GameService>().CardGame.GetEntities<CardInstance>().FirstOrDefault(c => c.EntityId == evt.DrawnCardId);
                _drawCardAnimation.PlayAnimation(playerBoard.Deck, playerBoard.Hand, cardInstanceDrawn, NextEvent);


            }
            else if (_currentGameEvent is DamageEvent)
            {
                var evt = _currentGameEvent as DamageEvent;
                var damagedEntity = GameObject.FindObjectsOfType<UIGameEntity3D>()
                     .FirstOrDefault(ent => ent.EntityId == evt.DamagedId);

                if (damagedEntity == null)
                {
                    NextEvent();
                    return;
                }

                this._damageAnimation.PlayAnimation(damagedEntity.transform, evt.DamageAmount, NextEvent);
            }
            else if (_currentGameEvent is AttackGameEvent)
            {
                var evt = _currentGameEvent as AttackGameEvent;
                var attacker = GameObject.FindObjectsOfType<UIGameEntity3D>()
                     .FirstOrDefault(ent => ent.EntityId == evt.AttackerId);

                var defender = GameObject.FindObjectsOfType<UIGameEntity3D>()
                     .FirstOrDefault(ent => ent.EntityId == evt.DefenderId);

                if (attacker == null || defender == null)
                {
                    NextEvent();
                    return;
                }

                this._fightAnimation.PlayAnimation(attacker.transform, defender.transform, NextEvent);
            }
        }
        else
        {
            _isPlayingAnimation = false;
        }
    }

    private void SetAnimationTimes()
    {
        _fightAnimation.AnimationTime = animationTime;
        _damageAnimation.AnimationTime = animationTime;
        _drawCardAnimation.AnimationTime = animationTime;        
    }
}
