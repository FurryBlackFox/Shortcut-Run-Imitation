using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICoinsDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText = default;
    private void Awake()
    {
        CustomTools.IsNull(coinsText, nameof(coinsText), name);
    }

    private void OnEnable()
    {
        GameManager.OnPlayerCoinsChanged += SetCoinsCount;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerCoinsChanged -= SetCoinsCount;
    }

    private void SetCoinsCount(int count)
    {
        coinsText.SetText($"{count}");
    }
}
