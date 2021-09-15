using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIPanelEmergenceTween))]
public class ScreenDimmer : MonoBehaviour
{
    private UIPanelEmergenceTween panelTweener;
    
    private void Awake()
    {
        panelTweener = GetComponent<UIPanelEmergenceTween>();
        
    }

    private void Start()
    {
        panelTweener.Fade();
    }

    private void OnEnable()
    {

        LevelLoader.OnLevelLoadStarted += EnableBlackScreen;
        LevelLoader.OnLevelReloadStarted += EnableBlackScreen;
    }

    private void OnDisable()
    {
        LevelLoader.OnLevelLoadStarted -= EnableBlackScreen;
        LevelLoader.OnLevelReloadStarted -= EnableBlackScreen;        
    }

    private void EnableBlackScreen()
    {
        panelTweener.Emerge();
    }
}
