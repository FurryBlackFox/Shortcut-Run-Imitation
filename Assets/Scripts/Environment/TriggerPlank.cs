using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlank : MonoBehaviour
{
    public event Action<bool> OnColliderStateChanged;
    
    public static bool RespawnPlanks { get; set; }
    
    public static float RespawnTime { get; set; }

    [SerializeField] private ParticleSystem smokeParticleSystem;
    
    private Collider triggerCollider;
    private MeshRenderer meshRenderer;
    
    private void Awake()
    {
#if UNITY_EDITOR

        CustomTools.IsNull(smokeParticleSystem, smokeParticleSystem.name, name);

#endif
        
        triggerCollider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Trigger()
    {
        meshRenderer.enabled = false;
        triggerCollider.enabled = false;
        OnColliderStateChanged?.Invoke(false);
        if (RespawnPlanks)
            StartCoroutine(RefreshPlank());
    }

    private IEnumerator RefreshPlank()
    {
        yield return new WaitForSeconds(RespawnTime);
        meshRenderer.enabled = true;
        triggerCollider.enabled = true;
        OnColliderStateChanged?.Invoke(true);
        smokeParticleSystem.Play();
    }
}
