using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Character))]
public class CharacterVFXController : MonoBehaviour
{
    protected static Transform nonChildEffectsRoot;
    
    [SerializeField] private ParticleSystem emergenceEffect = default;
    [SerializeField] private ParticleSystem waterSplashEffect = default;
    [SerializeField] private ParticleSystem waterRipplesEffect = default;
    [SerializeField] private float waterPosY = -1f;
    [SerializeField] private ParticleSystem speedBoostSmokeEffect = default;
    [SerializeField] private ParticleSystem speedBoostRingsEffect = default;
    [SerializeField] private ParticleSystem firstPositionEffect = default;
    [SerializeField] private ParticleSystem crownSmokeEffect = default;
    [SerializeField] protected float speedBoostThreshold = 1.2f;
    [SerializeField] protected GameObject leaderCrown = default;


    public GameObject LeaderCrown
    {
        get => leaderCrown;
        set
        {
            leaderCrown = value;
            leaderCrown.SetActive(false);
        }
    }
    
    protected Character character;

    private CharacterSkinController skinController;
    
    protected virtual void Awake()
    {
        character = GetComponent<Character>();

        if (TryGetComponent(out skinController))
        {
            skinController.OnSkinChange += OnEmergeHandler;
        }

        if (!nonChildEffectsRoot)
            nonChildEffectsRoot = new GameObject("VFX Root").transform;

        waterSplashEffect.transform.parent = nonChildEffectsRoot;
        waterRipplesEffect.transform.parent = nonChildEffectsRoot;
        
        leaderCrown.SetActive(false);
        

    }

    protected virtual void OnEnable()
    {
        character.OnEmerge += OnEmergeHandler;
        character.OnDrowning += OnDrawningHandler;
        character.CharacterPlanksController.OnSpeedChange += OnSpeedChangeHandler;
        CharacterPositionChecker.OnFirstPositionObtained += OnFirstPositionObtainedHandler;
        CharacterPositionChecker.OnFirstPositionLost += OnFirstPositionLostHandler;
    }

    protected virtual void OnDisable()
    {
        character.OnEmerge -= OnEmergeHandler;
        character.OnDrowning -= OnDrawningHandler;
        character.CharacterPlanksController.OnSpeedChange -= OnSpeedChangeHandler;
        CharacterPositionChecker.OnFirstPositionObtained -= OnFirstPositionObtainedHandler;
        CharacterPositionChecker.OnFirstPositionLost -= OnFirstPositionLostHandler;
    }
    

    private void OnEmergeHandler()
    {
        emergenceEffect.Play();
    }

    private void OnDrawningHandler()
    {
        var pos = character.transform.position;
        pos.y = waterPosY;
        waterRipplesEffect.transform.position = pos;
        waterRipplesEffect.Play();
        waterSplashEffect.transform.position = pos;
        waterSplashEffect.Play();
    }

    protected virtual void OnSpeedChangeHandler(float speedMult)
    {
        if (speedMult > speedBoostThreshold)
        {
            speedBoostSmokeEffect.Play();
            speedBoostRingsEffect.Play();
        }

        // else
        // {
        //     speedBoostSmokeEffect.Stop();
        //     speedBoostRingsEffect.Stop();
        // }
    }

    private void OnFirstPositionObtainedHandler(Character firstCharacter)
    {
        if (character != firstCharacter) 
            return;
        
        leaderCrown.SetActive(true);
        firstPositionEffect.Play();
        crownSmokeEffect.Play();
    }

    private void OnFirstPositionLostHandler(Character lostCharacter)
    {
        if (character != lostCharacter) 
            return;
        
        leaderCrown.SetActive(false);
        crownSmokeEffect.Play();
    }
}
