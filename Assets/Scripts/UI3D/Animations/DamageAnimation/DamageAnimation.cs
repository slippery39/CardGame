using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

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


    //[SerializeField] private float  = 0;


    // Start is called before the first frame update
    void Start()
    {
        PlayAnimation();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlayAnimation()
    {
        //Instantiate and set the transform.

        //For each damaged object we instantiate a screen space text object that is red and slowly rises and fades out
        var text = _damageTextPrefab.GetComponent<TextMeshPro>();
        var originalColor = text.color;

        _damageTextPrefab.GetComponent<RectTransform>().DOMoveY(_damageTextPrefab.GetComponent<RectTransform>().anchoredPosition.y + 1, _animationTime)
            .SetEase(_movementCurve);
        _damageTextPrefab.GetComponent<TextMeshPro>().DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 0), _animationTime)
            .SetEase(_fadeOutCurve);
    }
}
