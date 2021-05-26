using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class UIPlankCounter : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI uiText = default;
    [SerializeField] private float countDuration = 3f;

    [SerializeField] private bool enableFading = true;
    [SerializeField, Range(0f, 0.5f)] private float fadingDistanceScrPercent = 0.1f;
    [SerializeField] private float fadingDuration = 1f;
    
    
    private RectTransform rectTransf;
    private int plankCount = 0;
    private Coroutine countingTimer, fadingTimer;

    private Camera renderCamera;

    private void Awake()
    {
#if UNITY_EDITOR

        CustomTools.IsNull(uiText, nameof(uiText), name);

#endif
        
        uiText.enabled = false;
        rectTransf = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        GameDataKeeper.S.Player.OnPlankCollected += CountPlanks;
    }

    private void OnDisable()
    {
        GameDataKeeper.S.Player.OnPlankCollected -= CountPlanks;
    }
    
    
    private void Start()
    {
        renderCamera = GameDataKeeper.S.CinemachineBrain.OutputCamera;
    }
    

    private void CountPlanks(Vector3 pos)
    {
        if(countingTimer != null)
            StopCoroutine(countingTimer);

        if (fadingTimer != null)
        {
            StopCoroutine(fadingTimer);
            RestoreUITextDefaults();
        }

        plankCount++;
        uiText.SetText($"+{plankCount}");
        var viewportPos = renderCamera.WorldToViewportPoint(pos);
        rectTransf.anchorMin = viewportPos;
        rectTransf.anchorMax = viewportPos;
        uiText.enabled = true;
        
        countingTimer = StartCoroutine(CountingTimer());
    }

    private IEnumerator CountingTimer()
    {
        yield return new WaitForSeconds(countDuration);
        
        countingTimer = null;

        if(enableFading)
            fadingTimer = StartCoroutine(FadingTimer());
        else
            ClearCounter();
    }

    private IEnumerator FadingTimer()
    {
        var currentOffset = Vector2.zero;
        
        var fadingDistance = Screen.height * fadingDistanceScrPercent;
        var offsetSpeed = fadingDistance / fadingDuration;
        
        
        var currentAlpha = 1f;
        var currentColor = uiText.color;

        var alphaSpeed = 1f / fadingDuration;
        
        while (currentOffset.y <= fadingDistance)
        {
            currentOffset.y += Time.deltaTime * offsetSpeed;
            rectTransf.anchoredPosition = currentOffset;


            currentAlpha -= Time.deltaTime * alphaSpeed;
            currentColor.a = currentAlpha;
            uiText.color = currentColor;
            
            yield return new WaitForEndOfFrame();
        }
        
        RestoreUITextDefaults();
        
        ClearCounter();
    }

    private void RestoreUITextDefaults()
    {
        rectTransf.anchoredPosition = Vector2.zero;

        var currentColor = uiText.color;
        currentColor.a = 1f;
        uiText.color = currentColor;
        fadingTimer = null;
    }

    private void ClearCounter()
    {
        plankCount = 0;
        uiText.enabled = false;
    }
}
