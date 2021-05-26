using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerVFXController : CharacterVFXController
{

    [SerializeField] private ParticleSystem warpSpeedEffect = default;
    [SerializeField] private float warpSpeedScaleMod = 3f;

    private Player player;
    private float warpSpeedScale;
    private ParticleSystemRenderer warpSpeedEffectRenderer;
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
        warpSpeedEffectRenderer = warpSpeedEffect.GetComponent<ParticleSystemRenderer>();
        warpSpeedScale = warpSpeedEffectRenderer.velocityScale;
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void OnSpeedChangeHandler(float speedMult)
    {
        base.OnSpeedChangeHandler(speedMult);

        if (speedMult > speedBoostThreshold)
        {
            warpSpeedEffect.Play();
            warpSpeedEffectRenderer.velocityScale = warpSpeedScale * Mathf.Pow(speedMult, warpSpeedScaleMod);
        }
        //     
        // else
        // {
        //     warpSpeedEffect.Stop();
        //    
        // }

      
    }
}
