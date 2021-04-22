using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UIPlayerPositionUpdater : MonoBehaviour
{

    private readonly string[] postfixes = {"st", "nd", "rd", "th"};
    
    private TextMeshProUGUI UIText;

    
    private void Awake() //TODO: smth better
    {
        UIText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        CharacterPositionChecker.OnPlayerPositionUpdated += ApplyPositionOnUI;
    }

    private void OnDisable()
    {
        CharacterPositionChecker.OnPlayerPositionUpdated -= ApplyPositionOnUI;
    }

    private void ApplyPositionOnUI(int playerPos)
    {
        var postfix = playerPos < 3 ? postfixes[playerPos - 1] : postfixes[3];
        UIText.SetText($"{playerPos}<sup>{postfix}</sup>");
    }


}
