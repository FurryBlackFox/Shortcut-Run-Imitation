using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UIFpsCounter : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float checkDelay = 0.5f;
    
    
    private TextMeshProUGUI UIText;
    private int counter = 0;
    private float passedTime = 0;

    private void Awake()
    {
        UIText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        StartCoroutine(CheckFps());
    }

    private void Update()
    {
        counter++;
        passedTime += Time.unscaledDeltaTime;
    }
    
    private IEnumerator CheckFps()
    {
        var delay = new WaitForSeconds(checkDelay);
        while (enabled)
        {
            yield return delay;
            UIText.SetText($"{counter/passedTime:F1}");
            counter = 0;
            passedTime = 0f;
            
        }

    }
}
