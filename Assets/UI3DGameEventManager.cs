using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

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
    //[SerializeField] private GameAnimation _currentAnimation;

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


    void AddEventToQueue(GameEvent gameEvent)
    {
        _eventQueue.Enqueue(gameEvent);
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
                _currentGameEvent = null;
            }
            else if (_currentGameEvent is DamageEvent)
            {
                var evt = _currentGameEvent as DamageEvent;
                var damagedEntity = GameObject.FindObjectsOfType<UIGameEntity3D>()
                     .Where(ent => ent.EntityId == evt.DamagedId)
                     .FirstOrDefault();
                if (damagedEntity == null)
                {
                    this._currentGameEvent = null;
                    return;
                }

                this._damageAnimation.PlayAnimation(damagedEntity.transform, evt.DamageAmount, () => this._currentGameEvent = null);
            }
            else if (_currentGameEvent is AttackGameEvent)
            {
                var evt = _currentGameEvent as AttackGameEvent;
                var attacker = GameObject.FindObjectsOfType<UIGameEntity3D>()
                     .Where(ent => ent.EntityId == evt.AttackerId)
                     .FirstOrDefault();
                var defender = GameObject.FindObjectsOfType<UIGameEntity3D>()
                     .Where(ent => ent.EntityId == evt.DefenderId)
                     .FirstOrDefault();

                if (attacker == null || defender == null)
                {
                    _currentGameEvent = null;
                    return;
                }

                this._fightAnimation.PlayAnimation(attacker.transform, defender.transform, () => this._currentGameEvent = null);
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
    }
}
