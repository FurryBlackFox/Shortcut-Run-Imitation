using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIPanelEmergenceTween : MonoBehaviour
{
    [SerializeField]
    private float animationDuration = 0.5f;

    [SerializeField]
    private UpdateType 
        emergeUpdateType = UpdateType.Normal,
        fadeUpdateType = UpdateType.Normal;
    
    [SerializeField] 
    private float
        emergeDelay = 0f,
        fadeDelay = 0f;
    
    [SerializeField] 
    private UnityEvent 
        emergeCallback,
        fadeCallback;

    private Image image;
    private Color startColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        startColor = image.color;
    }

    public void MakeTransparent() 
    {
        var transparentColor = image.color;
        transparentColor.a = 0f;
        image.color = transparentColor;
    }

    public void Emerge()
    {
        image.color = startColor;
        image.DOFade(0, animationDuration).From().SetDelay(emergeDelay).SetUpdate(emergeUpdateType)
            .OnComplete(() => emergeCallback?.Invoke());
    }

    public void Fade()
    {
        image.color = startColor;
        image.DOFade(0, animationDuration).SetDelay(fadeDelay).SetUpdate(fadeUpdateType)
            .OnComplete(() => fadeCallback?.Invoke());
    }


}
