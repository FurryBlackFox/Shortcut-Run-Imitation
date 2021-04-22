using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOnCharacterReachedFinish : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI UIText = default;
    [SerializeField, Range(0f, 5f)] private float showDuration = 1f;

    
    private Image backgroundImage;
    private WaitForSeconds delay;
    private bool playerFinished = false;
    private void Start()
    {
        #if UNITY_EDITOR
        
        CustomTools.IsNull(UIText, nameof(UIText), name);  
        
        #endif
        
        delay = new WaitForSeconds(showDuration);
        backgroundImage = GetComponent<Image>();
        ShowUIElements(false);
    }

    private void OnEnable()
    {
        Finish.OnCharacterFinish += CharacterFinished;
        Finish.OnPlayerFinish += PlayerFinished;
    }


    private void OnDisable()
    {
        Finish.OnCharacterFinish -= CharacterFinished;
        Finish.OnPlayerFinish -= PlayerFinished;
    }

    private void CharacterFinished(Character character)
    {
        if(playerFinished)
            return;
        StartCoroutine(ShowAndHideUI(character.CharacterName)); //TODO: smooth
    }

    private IEnumerator ShowAndHideUI(string characterName)
    {
        ShowUIElements(true);
        UIText.SetText($"{characterName} has reached the finish!");
        yield return delay;
        ShowUIElements(false);
    }

    private void ShowUIElements(bool val)
    {
        backgroundImage.enabled = val;
        UIText.enabled = val;
    }

    private void PlayerFinished()
    {
        playerFinished = true;
    }
}
