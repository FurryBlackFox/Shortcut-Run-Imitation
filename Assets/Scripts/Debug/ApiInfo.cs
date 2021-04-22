using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using System.Diagnostics;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ApiInfo : MonoBehaviour
{
    private TextMeshProUGUI textUI;
    
    private void Awake()
    {
        textUI = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        textUI.SetText($"{SystemInfo.graphicsDeviceName} \n {SystemInfo.graphicsDeviceVersion} \n GPU Mem: {SystemInfo.graphicsMemorySize}");

    }
}
