using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverAnimation : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _gameOverText;

    [SerializeField]
    private float _animationTime = 2.5f;

    [SerializeField]
    private AnimationCurve _fadeInCurve;

    [SerializeField]
    private Color _initialColor;

    private void Awake()
    {
        _initialColor = _gameOverText.color;
    }

    public void PlayAnimation(string label, Action onComplete = null)
    {
        this.gameObject.SetActive(true);
        _gameOverText.text = label;

        Debug.Log("Our game over animation is playing");

        _gameOverText.DOColor(new Color(_initialColor.r, _initialColor.g, _initialColor.b, 1), _animationTime)
            .OnComplete(() =>
            {

                if (onComplete != null)
                {
                    onComplete();
                }
            });
    }
}
