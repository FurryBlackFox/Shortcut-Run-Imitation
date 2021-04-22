using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankStack : MonoBehaviour
{
    [SerializeField] private List<TriggerPlank> planks = default;
    [SerializeField] private Collider navigationCollider = default;
    
    
    private int colliderActivationPlankThreshold;
    private int activePlanksCount = 0;
    
    private void OnEnable()
    {
        foreach (var plank in planks)
        {
            plank.OnColliderStateChanged += UpdatePlanksCount;
        }
    }

    private void OnDisable()
    {
        foreach (var plank in planks)
        {
            plank.OnColliderStateChanged -= UpdatePlanksCount;
        }
    }

    private void Start()
    {
        activePlanksCount = planks.Count;
        colliderActivationPlankThreshold = GameDataKeeper.S.GameSettings.StackNavigationColliderActivationThreshold;
    }

    private void UpdatePlanksCount(bool value)
    {
        activePlanksCount += value ? 1 : -1;
        navigationCollider.enabled = activePlanksCount >= colliderActivationPlankThreshold;
    }
}
