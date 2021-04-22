using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotNavigationSphere : MonoBehaviour
{
    public List<Transform> PlanksTransforms { get; private set; }

    private GameSettings gameSettings;

    private void Awake()
    {
        PlanksTransforms = new List<Transform>();
        gameSettings = GameDataKeeper.S.GameSettings;
    }

    private void Start()
    {
        
    }

    private void OnDrawGizmos()
    {
        if(!Application.isPlaying)
            return;
        Gizmos.color = Color.cyan;
        if(PlanksTransforms.Count > 0)
            foreach (var planks in PlanksTransforms)
                Gizmos.DrawSphere(planks.position, 1f);
    }
    //
    // private void OnCollisionEnter(Collision other)
    // {
    //     var planksTransform = other.transform;
    //     if(!planksTransform.CompareTag("PlankNavigation"))
    //         return;
    //     CheckPlanksPoint(planksTransform);
    // }
    //
    // private void OnCollisionExit(Collision other)
    // {
    //     var planksTransform = other.transform;
    //     if(!planksTransform.CompareTag("PlankNavigation"))
    //         return;
    //     RemovePlanksPoint(planksTransform);
    // }

    private void OnTriggerEnter(Collider other)
    {
        var planksTransform = other.transform;
        if(!planksTransform.CompareTag("PlankNavigation"))
            return;
        CheckPlanksPoint(planksTransform);
    }
    
    private void OnTriggerExit(Collider other)
    {
        var planksTransform = other.transform;
        if(!planksTransform.CompareTag("PlankNavigation"))
            return;
        RemovePlanksPoint(planksTransform);
    }

    private void CheckPlanksPoint(Transform planksTransform)
    {
        // if(PlanksTransforms.Count > 0)
        //     return;
        if(PlanksTransforms.Contains(planksTransform))
            return;
        if ( Random.value <= gameSettings.PlanksCollectionMod)
            PlanksTransforms.Add(planksTransform);

    }

    private void RemovePlanksPoint(Transform planksTransform)
    {
        if(!PlanksTransforms.Contains(planksTransform))
            return;
        PlanksTransforms.Remove(planksTransform);
    }
}
