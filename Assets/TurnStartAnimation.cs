using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

public class TurnStartAnimation : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _turnStartText;

    [SerializeField]
    private float _animationTime = 1f;

    [SerializeField]
    private AnimationCurve _movementCurve;

    [SerializeField]
    private Vector3 initialPosition;

    [SerializeField]
    private Vector3 endPosition;

    public void Awake()
    {
        this.initialPosition = this.transform.position;
    }

    public void PlayTextAnimation(string label, Action onComplete = null)
    {
        this.gameObject.SetActive(true);
        _turnStartText.text = label;
        this.transform.position = this.initialPosition;
        this.transform.DOMove(endPosition, _animationTime)
            .SetEase(_movementCurve)
            .OnComplete(() =>
             {
                 if (onComplete != null)
                 {
                     onComplete();
                 }
             });
    }
}
