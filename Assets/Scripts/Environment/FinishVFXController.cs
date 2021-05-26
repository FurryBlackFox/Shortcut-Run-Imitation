using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishVFXController : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] confettiEffects = default;

    private void OnEnable()
    {
        Finish.OnCharacterFinish += OnCharacterFinishHandler;
    }

    private void OnDisable()
    {
        Finish.OnCharacterFinish -= OnCharacterFinishHandler;
    }

    private void OnCharacterFinishHandler(Character character)
    {
        foreach (var confettiEffect in confettiEffects)
        {
            confettiEffect.Play();
        }
    }
}
