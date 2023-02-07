using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

public class DamageAnimation : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro _damageTextPrefab;

    [SerializeField]
    private float _animationTime = 1f;

    [SerializeField]
    private AnimationCurve _movementCurve;

    [SerializeField]
    private AnimationCurve _fadeOutCurve;

    public float AnimationTime { get => _animationTime; set => _animationTime = value; }

    public void PlayAnimation(Transform transform, int damageAmount, Action onComplete = null)
    {
        _damageTextPrefab.gameObject.SetActive(true);
        //Instantiate and set the transform.

        //For each damaged object we instantiate a screen space text object that is red and slowly rises and fades out
        var text = _damageTextPrefab.GetComponent<TextMeshPro>();
        text.text = (-damageAmount).ToString();

        var parent = _damageTextPrefab.transform.parent;

        parent.position = transform.position;
        var originalPosition = parent.position;

        var originalColor = text.color;

        _damageTextPrefab.transform.parent.transform.DOLocalMoveZ(originalPosition.z + 0.5f, _animationTime)
            .SetEase(_movementCurve);
        _damageTextPrefab.GetComponent<TextMeshPro>().DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 0), _animationTime)
            .SetEase(_fadeOutCurve)
            .OnComplete(() =>
            {
                _damageTextPrefab.gameObject.SetActive(false);
                parent .position = originalPosition;
                //_damageTextPrefab.gameObject.SetActive(false);
                _damageTextPrefab.GetComponent<TextMeshPro>().color = originalColor;

                if (onComplete != null)
                {
                    onComplete();
                }
            });
    }
}
