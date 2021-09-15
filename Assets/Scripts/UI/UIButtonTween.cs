using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class UIButtonTween : MonoBehaviour
{
    [SerializeField]
    private float onClickScale = 0.95f;
    [SerializeField]
    private float onClickAnimDuration = 0.1f;

    [SerializeField] 
    private Ease ease;
    
    [SerializeField]
    private UnityEvent onClick;


    private Vector3 onClickScaleVector3;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        onClickScaleVector3 = Vector3.one * onClickScale;
    }

    
    public void OnClick()
    {
        var anim = rectTransform.DOScale(onClickScaleVector3, onClickAnimDuration)
            .SetEase(ease);
        anim.OnComplete(() =>
        {
            onClick?.Invoke();
        });
        
    }

    public void Test()
    {
        Debug.Log("test");
    }
}
