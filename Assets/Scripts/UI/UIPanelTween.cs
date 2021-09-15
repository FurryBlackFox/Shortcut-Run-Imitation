using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class UIPanelTween : MonoBehaviour
{
    [SerializeField]
    private float animationDuration = 1f;

    [SerializeField]
    private Ease ease;

    [SerializeField]
    private bool 
        useX = true,
        useY = true;
    
    [SerializeField] 
    private Vector2 mult = Vector2.one;

    [SerializeField]
    private UnityEvent tweenOutCallback;
    
    private RectTransform rectTransform;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private Vector3 Offset()
    {
        var offset = new Vector3(useX ? Screen.width : 0f, useY ? Screen.height : 0f);
        offset *= mult;

        return offset;
    }

    public void TweenIn()
    {
        rectTransform.anchoredPosition = Offset();
        rectTransform.DOAnchorPos(Vector2.zero, animationDuration).SetEase(ease);
    }

    public void TweenOut()
    {
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.DOAnchorPos(Offset(), animationDuration).SetEase(ease)
            .OnComplete(() => tweenOutCallback?.Invoke());
    }
}
