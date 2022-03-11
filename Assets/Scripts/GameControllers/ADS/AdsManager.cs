using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;



public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    public event Action OnAdsFinished;
    public event Action OnAdsSkipped;
    
    [SerializeField] private string androidGameId = "4164661";
    [SerializeField] private bool testMode = true;
    [SerializeField] private bool enablePerPlacementLoad = false;
    [SerializeField] private bool enableLogs = true;

    [SerializeField] private string revardedAndroidAds = "Rewarded_Android";
    [SerializeField] private string interstitialAndroidAds = "Interstitial_Android";
    [SerializeField] private string bannerAndroidAds = "Banner_Android";
    
    private void Awake()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(androidGameId, testMode, enablePerPlacementLoad);
    }

    public void ShowRewardedAds()
    {
        ShowAds(revardedAndroidAds);
    }
    
    public void ShowInterstitialAds()
    {
        ShowAds(interstitialAndroidAds);
    }

    private void ShowAds(string placementId)
    {
        if (Advertisement.IsReady(placementId))
        {
            Advertisement.Show(placementId);
        }
        else if(enableLogs)
            Debug.LogWarning($"Ads {placementId} is not ready!");
    }


    public void OnUnityAdsReady(string placementId)
    {
        if(enableLogs)
            Debug.Log($"Ads {placementId} is ready!");
    }

    public void OnUnityAdsDidError(string message)
    {
       
        Debug.LogError(message);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        if(enableLogs)
            Debug.Log($"Ads {placementId} is started!");
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        switch (showResult)
        {
            case ShowResult.Failed:
                Debug.LogError($"Ads {placementId} is failed in finish!");
                break;
            case ShowResult.Skipped:
                if(enableLogs)
                    Debug.Log($"Ads {placementId} is skipped!");
                OnAdsSkipped?.Invoke();
                break;
            case ShowResult.Finished:
                if(enableLogs)
                    Debug.Log($"Ads {placementId} is finished!");
                OnAdsFinished?.Invoke();
                break;
            default:
                Debug.LogError($"Ads {placementId} is gone!");
                throw new ArgumentOutOfRangeException(nameof(showResult), showResult, null);
        }
    }
}
