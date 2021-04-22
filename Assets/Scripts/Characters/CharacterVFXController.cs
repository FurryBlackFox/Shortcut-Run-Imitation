using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Character))]
public class CharacterVFXController : MonoBehaviour
{
    private static Transform nonChildEffectsRoot;
    
    [SerializeField] private ParticleSystem emergenceEffect = default;
    [SerializeField] private ParticleSystem waterSplashEffect = default;
    [SerializeField] private ParticleSystem waterRipplesEffect = default;
    [SerializeField] private float waterPosY = -1f;
    [SerializeField] private ParticleSystem speedBoostSmokeEffect = default;
    [SerializeField] private ParticleSystem speedBoostRingsEffect = default;
    [SerializeField] private float speedBoostThreshold = 1.2f;
    [SerializeField] private ParticleSystem firstPositionEffect = default;
    [SerializeField] private ParticleSystem crownSmokeEffect = default;
    [SerializeField] protected GameObject firstPlaceCrownGO = default;
    
    
    private Character character;
    
    private void Awake()
    {
#if UNITY_EDITOR

        CustomTools.IsNull(emergenceEffect, nameof(emergenceEffect), name);
        CustomTools.IsNull(waterSplashEffect, nameof(waterSplashEffect), name);
        CustomTools.IsNull(waterRipplesEffect, nameof(waterRipplesEffect), name);
        CustomTools.IsNull(speedBoostSmokeEffect, nameof(speedBoostSmokeEffect), name);
        CustomTools.IsNull(speedBoostRingsEffect, nameof(speedBoostRingsEffect), name);
        CustomTools.IsNull(firstPositionEffect, nameof(firstPositionEffect), name);
        CustomTools.IsNull(crownSmokeEffect, nameof(crownSmokeEffect), name);
        CustomTools.IsNull(firstPlaceCrownGO, nameof(firstPlaceCrownGO), name); 

#endif
        character = GetComponent<Character>();

        if (!nonChildEffectsRoot)
            nonChildEffectsRoot = new GameObject("VFX Root").transform;

        waterSplashEffect.transform.parent = nonChildEffectsRoot;
        waterRipplesEffect.transform.parent = nonChildEffectsRoot;
        
        firstPlaceCrownGO.SetActive(false);
        

    }

    private void OnEnable()
    {
        character.OnEmerge += OnEmergeHandler;
        character.OnDrowning += OnDrawningHandler;
        character.CharacterPlanksController.OnSpeedChange += OnSpeedChangeHandler;
        CharacterPositionChecker.OnFirstPositionObtained += OnFirstPositionObtainedHandler;
        CharacterPositionChecker.OnFirstPositionLost += OnFirstPositionLostHandler;
    }

    private void OnDisable()
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

    private void OnSpeedChangeHandler(float speedMult)
    {
        if (speedMult > speedBoostThreshold)
        {
            speedBoostSmokeEffect.Play();
            speedBoostRingsEffect.Play();
        }

        else
        {
            speedBoostSmokeEffect.Stop();
            speedBoostRingsEffect.Stop();
        }
    }

    private void OnFirstPositionObtainedHandler(Character firstCharacter)
    {
        if (character != firstCharacter) 
            return;
        
        firstPlaceCrownGO.SetActive(true);
        firstPositionEffect.Play();
        crownSmokeEffect.Play();
    }

    private void OnFirstPositionLostHandler(Character lostCharacter)
    {
        if (character != lostCharacter) 
            return;
        
        firstPlaceCrownGO.SetActive(false);
        crownSmokeEffect.Play();
    }
}
