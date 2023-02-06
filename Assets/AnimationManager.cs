using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private GameService _gameService;

    // Start is called before the first frame update
    void Start()
    {
        _gameService.OnGameEvent.Subscribe(gameEvent =>
        {
            AddEventToQueue(gameEvent);
            //Add the event to our internal animation queue.
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (animationQueue.Count>0 && currentAnimation == null)
        {
            PlayNextAnimation();
        }
        //If the animation queue has an animation to be played && an animation is not currently playing
        //start playing that animation
        //once the animation is done, remove it from the queue
        //set the resulting state as well.
    }

   void AddEventToQueue(CardGameEvent gameEvent)
    {

    }

    void PlayNextAnimation()
    {

    }
}
