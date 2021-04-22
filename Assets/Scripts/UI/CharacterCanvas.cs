using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CharacterCanvas : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        GameManager.OnPlayerDeath += PlayerDeathHandle;
        Finish.OnPlayerFinish += PlayerFinishHandle;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerDeath -= PlayerDeathHandle;
        Finish.OnPlayerFinish -= PlayerFinishHandle;
    }

    private void PlayerDeathHandle()
    {
        CustomTools.EnableCanvasGroup(canvasGroup, false);
    }

    private void PlayerFinishHandle()
    {
        CustomTools.EnableCanvasGroup(canvasGroup, false);
    }
}
