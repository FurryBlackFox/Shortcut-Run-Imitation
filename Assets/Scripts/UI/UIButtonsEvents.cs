using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonsEvents : MonoBehaviour
{
    public static event Action OnRestart;
    public static event Action<bool> OnHomePopUp;
    public static event Action OnPlay;
    public static event Action<int> OnClaimPoints;
    public static event Action<bool> OnSettings;
    public static event Action OnHome;
    public static event Action OnShop;

    private void OnEnable()
    {
        //UIClaimPointsButton.OnClaimPoints += OnClaimPointsHandler;
    }

    private void OnDisable()
    {
        //UIClaimPointsButton.OnClaimPoints -= OnClaimPointsHandler;
    }

    public void OnRetryButton()
    {
        OnRestart?.Invoke();
    }

    public void OnHomePopUpButton()
    {
        OnHomePopUp?.Invoke(true);
    }

    public void OnHomePopUpCloseButton()
    {
        OnHomePopUp?.Invoke(false);
    }

    public void OnHomeButton()
    {
        OnHome?.Invoke();
    }

    public void OnSettingsButton()
    {
        OnSettings?.Invoke(true);
    }

    public void OnSettingsCloseButton()
    {
        OnSettings?.Invoke(false);
    }

    public void OnPlayButton()
    {
        OnPlay?.Invoke();
    }

    public void OnShopButton()
    {
        OnShop?.Invoke();
    }
    
    private int cashedRewardPoints = 0;
    
    public void OnClaimPointsButton(int points, bool hasBonus)
    {
        if (hasBonus)
        {
            GameDataKeeper.S.AdsManager.ShowRewardedAds();
            cashedRewardPoints = points;
            GameDataKeeper.S.AdsManager.OnAdsFinished += OnAdsFinishedHandler;
        }
        else
            OnClaimPoints?.Invoke(points);
    }
    
    private void OnAdsFinishedHandler()
    {
        GameDataKeeper.S.AdsManager.OnAdsFinished -= OnAdsFinishedHandler;
        OnClaimPoints?.Invoke(cashedRewardPoints);
    }
}
