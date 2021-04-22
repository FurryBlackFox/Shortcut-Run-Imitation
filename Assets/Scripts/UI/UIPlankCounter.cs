using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class UIPlankCounter : MonoBehaviour
{
    [SerializeField] private float
        countDuration = 3f;
    
    [SerializeField] private TextMeshProUGUI uiText = default;
    private RectTransform rectTransf;
    private float countTimer = 0f;
    private int plankCount = 0;
    

    private Camera renderCamera;

    private void Awake()
    {
#if UNITY_EDITOR

        CustomTools.IsNull(uiText, nameof(uiText), name);

#endif
        
        uiText.enabled = false;
        rectTransf = GetComponent<RectTransform>();
        
        GameDataKeeper.S.Player.OnPlankCollected += CountPlanks;
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }
    
    
    private void Start()
    {
        renderCamera = GameDataKeeper.S.CinemachineBrain.OutputCamera;
    }

    private void Update()
    {
        if (countTimer >= countDuration)
        {
            plankCount = 0;
            uiText.enabled = false;
            return; 
        }
        countTimer += Time.deltaTime;
    }

    private void CountPlanks(Vector3 pos)
    {
        countTimer = 0;
        plankCount++;
        uiText.SetText($"+{plankCount}");
        var viewportPos = renderCamera.WorldToViewportPoint(pos);
        rectTransf.anchorMin = viewportPos;
        rectTransf.anchorMax = viewportPos;
        uiText.enabled = true;
    }
}
