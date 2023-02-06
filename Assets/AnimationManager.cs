using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private UI3DController _uiController;
    [SerializeField] private GameService _gameService;
    [SerializeField] private Queue<GameEvent> _animationQueue = new Queue<GameEvent>();
    [SerializeField] private bool _isPlayingAnimation = false;
    [SerializeField] private GameEvent _currentGameEvent;


    [SerializeField]
    private FightAnimation _fightAnimation;
    //[SerializeField] private GameAnimation _currentAnimation;

    // Start is called before the first frame update
    void Start()
    {
        _gameService.GetGameEventObservable().Subscribe(gameEvent =>
        {
            AddEventToQueue(gameEvent);
            //Add the event to our internal animation queue.
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (_animationQueue.Count > 0 && _currentGameEvent == null)
        {
            PlayNextAnimation();
        }
        //If the animation queue has an animation to be played && an animation is not currently playing
        //start playing that animation
        //once the animation is done, remove it from the queue
        //set the resulting state as well.
    }

    void AddEventToQueue(GameEvent gameEvent)
    {
        _animationQueue.Enqueue(gameEvent);
    }

    void PlayNextAnimation()
    {
        _isPlayingAnimation = true;
        if (_animationQueue.Count > 0)
        {
            _currentGameEvent = _animationQueue.Dequeue();
            if (_currentGameEvent is GameStateUpdatedEvent)
            {
                //Update the current game state
                var evt = _currentGameEvent as GameStateUpdatedEvent;
                _uiController.SetUIGameState(evt.ResultingState);
                _currentGameEvent = null;
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
}
