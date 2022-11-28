using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UniRxTest : MonoBehaviour
{

    public IObservable<string> testBs = Observable.Create<string>((obs) =>
    {
        obs.OnNext("Testing UNI RX.Observable");
        obs.OnNext("1");
        obs.OnNext("2");
        obs.OnNext("3");
        return Disposable.Empty;
    });

    public IObservable<long> timesObservable = Observable.Interval(TimeSpan.FromSeconds(2));

    public ISubject<string> observableEvent = new Subject<string>();
    // Start is called before the first frame update
    void Start()
    {
        testBs.Subscribe((str) => Debug.LogWarning(str));
        timesObservable.Subscribe(time => Debug.LogWarning(time));
        observableEvent.Subscribe(ev => Debug.LogWarning(ev));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            observableEvent.OnNext("key code g has been pressed");
        }
    }
}
