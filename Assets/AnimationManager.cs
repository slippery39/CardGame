using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private GameService _gameService;
    [SerializeField] private Queue<GameEvent> _animationQueue = new Queue<GameEvent>();
    [SerializeField] private bool _isPlayingAnimation = false;
    [SerializeField] private GameEvent _currentGameEvent;
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
        if (_animationQueue.Count > 0 /*&& currentAnimation == null*/)
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
        }
        else
        {
            _isPlayingAnimation = false;
        }

        //TODO - Properly end animations.
        Debug.Log("Playing animation!" + _currentGameEvent.ToString());
    }
}