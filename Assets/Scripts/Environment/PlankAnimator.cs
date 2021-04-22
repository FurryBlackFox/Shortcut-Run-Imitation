using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Plank))]
public class PlankAnimator : MonoBehaviour
{
    [SerializeField] private ParticleSystem smokeEffect = default;
    [SerializeField] private ParticleSystem pickUpEffect = default;
    [SerializeField] private ParticleSystem flareEffect = default;


    private readonly int
        idSpawned = Animator.StringToHash("Spawned"),
        idPickedUp = Animator.StringToHash("PickedUp");
    
    
    private Animator animator;
    private Plank plank;
    

    private void Awake()
    {
#if UNITY_EDITOR

        CustomTools.IsNull(pickUpEffect, nameof(pickUpEffect), name);
        CustomTools.IsNull(smokeEffect, nameof(smokeEffect), name);
        CustomTools.IsNull(flareEffect, nameof(flareEffect), name);

#endif
        
        plank = GetComponent<Plank>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        plank.OnSpawn += OnPlankSpawnHandler;
        plank.OnPickUp += OnPlankPickUpHandler;
    }

    private void OnDisable()
    {
        plank.OnSpawn -= OnPlankSpawnHandler;
        plank.OnPickUp += OnPlankPickUpHandler;
    }


    private void OnPlankSpawnHandler()
    {
        
        smokeEffect.Play();
        flareEffect.Play();
        animator.SetTrigger(idSpawned);
    }

    private void OnPlankPickUpHandler()
    {
        pickUpEffect.Play();
        animator.SetTrigger(idPickedUp);
    }
    
}
