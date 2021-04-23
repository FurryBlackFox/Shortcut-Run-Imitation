﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotMovementController : CharacterMovementController
{
    private struct AvoidanceRay
    {
        public Vector3 vector;
        public float modifier;
    }


    
    [Header("Navigation")]
    public Waypoint enterWaypoint = default;
    
    [Header("Editor Tools")] 
    [SerializeField] private bool showRays = true;
    [SerializeField] private bool showTarget = true;



    private BotSettings botSettings;
    private Bot bot;
    private AvoidanceRay[] rays;
    private float oneDivRayDistance;
    
    private float prevAngle, prevAvoidAngle;
    private float avoidRotAngle;
    private Vector3 closestPoint, target;

    private Transform cashedSpinObsTrasf = default;
    private Vector3 cashedSpinObsPos;
    private SpinningObstacle cashedSpinObs;

    private Waypoint targetWaypoint;
    private float currMinWaypSqrDistance;
    
    private Vector3 currentTarget, nextTarget, targetWaypPos;
    private float targetLerp, targetLerpDuration;


    protected override void Awake()
    {
        base.Awake();
        botSettings = GameDataKeeper.S.BotSettings;
        bot = GetComponent<Bot>();
        
        if(!enterWaypoint) 
            enterWaypoint = GameDataKeeper.S.EnterWaypoint;
        
        rays = new AvoidanceRay[botSettings.RaysCount * 2 - 1];
        cashedSpinObsPos = Vector3.zero;
        oneDivRayDistance = 1 / botSettings.RayDistance;
        CalculateRaysVectors();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.OnGameStart += SetStartTarget;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameManager.OnGameStart -= SetStartTarget;
    }

    protected override void Start()
    {
        base.Start();
        
#if UNITY_EDITOR
        
        CustomTools.IsNull(enterWaypoint, nameof(enterWaypoint), name);
        
#endif
    }

    
#if UNITY_EDITOR
    
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = gizmosColor;

        if (showTarget)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(currentTarget, 1f);
            Gizmos.DrawSphere(nextTarget, 1f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(target, Mathf.Sqrt(currMinWaypSqrDistance));
        }

        if (Application.isPlaying && showRays)
        {
            var position = transform.position;

            position.y += botSettings.RaysOffset;
            for (var i = 0; i < rays.Length; i++)
            {
                Vector3 p;
                if (i == 0)
                    p = position + transform.TransformDirection(rays[i].vector * botSettings.CentralRayDistance);
                else
                    p = position + transform.TransformDirection(rays[i].vector * botSettings.RayDistance);
                Gizmos.DrawLine(position, p);
                Handles.Label(p, $"{rays[i].modifier}");
            } 
            Gizmos.DrawSphere(cashedSpinObsPos, 1f);
        }

    }
    
#endif
    
    internal override void OnFixedUpdateHandler()
    {
        base.OnFixedUpdateHandler();
        if (targetWaypoint && isMoving && !inWater)
        {
            WaypointMovement();
        }
        else
        {
            movement.x = 0f;
            movement.z = 0f;
        }
        ApplyGravity();
        ApplyExternalForces();
        characterController.Move(movement * Time.fixedDeltaTime);
    }

    #region Movement
 
    private void WaypointMovement()
    {
        if (targetLerp <= 1f)
        {
            targetLerp += Time.fixedDeltaTime / targetLerpDuration;
            target = Vector3.Lerp(currentTarget, nextTarget, targetLerp);
            target = Vector3.Lerp(transform.position, target, targetLerp);
        }
        
        var sqrDistance = CalculateSqrDistance(nextTarget);
        if (sqrDistance > currMinWaypSqrDistance)
        {
            rotAngle = CalculateRotAngle(target);
            if (botSettings.AvoidEnabled)
                CalculateAvoidAngle();
            CallMovementEvents(MoveForward(),Rotate());
        }
        else
        {
            currentTarget = nextTarget;
            targetWaypoint = ChooseNextTarget(targetWaypoint, bot.PlanksCount);
            if (targetWaypoint)
            {
                nextTarget = targetWaypoint.GetRandomPosition();
                    
                currMinWaypSqrDistance = targetWaypoint.IsLast() ? botSettings.MinFinishSqrDistance : 
                botSettings.MinWaypSqrDistance;
                targetLerp = 0f;
                var sqrDifferenceDistance = (nextTarget - currentTarget).sqrMagnitude;
                var targetTransitionSpeed = gameSettings.MaxSpeed * botSettings.TargetLerpMult;
                targetTransitionSpeed *= targetTransitionSpeed;
                targetLerpDuration =  sqrDifferenceDistance / targetTransitionSpeed ;
            }
        }
    }

    protected void SetStartTarget()
    {
        targetWaypoint = enterWaypoint;
        nextTarget = currentTarget = target = targetWaypoint.GetRandomPosition();
        currMinWaypSqrDistance = botSettings.MinWaypSqrDistance;
        targetLerp = 1f;
    }

    private Waypoint ChooseNextTarget(Waypoint current, int planksCount)
    {
        var rand = Random.value;

        if (current.branches.Count < 1 || rand > current.BranchRatio)
            return current.nextWaypoint;

        var availableBranches = new List<Branch>();

        foreach (var branch in current.branches)
        {
            if (branch.cost <= planksCount)
                availableBranches.Add(branch);
        }

        if (availableBranches.Count > 0)
        {
            return availableBranches[Random.Range(0, availableBranches.Count)].waypoint;

        }

        return current.nextWaypoint;
    }

    private float CalculateSqrDistance(Vector3 position)
    {
        var direction = position - transform.position;
        direction.y = 0;
        return direction.sqrMagnitude;
    }
    

    protected float MoveForward()
    {
        Vector3 forward;
        if (isMoving)
        {
            forward = transform.forward;
            var speed = gameSettings.DefaultSpeed * currentSpeedMult;
            forward *= speed > gameSettings.MaxSpeed ? gameSettings.MaxSpeed : speed;
        }
        else
        {
            forward = Vector3.zero;
        }
        
        movement.x = forward.x;
        movement.z = forward.z;

        return forward.sqrMagnitude;
    }
    
    private new float Rotate()
    {
        float finAngle;
        rotAngle *= botSettings.NavigationAngleMult;
        finAngle = Mathf.Abs(avoidRotAngle) > 0 ? Mathf.Lerp(rotAngle, avoidRotAngle, botSettings.RayStrMod) : rotAngle;
        finAngle = Mathf.Lerp(prevAngle, finAngle, botSettings.AngleLerp);
        if(Mathf.Abs(finAngle) <= 0)    
            return 0;
        var sign = Mathf.Sign(finAngle);
        prevAngle = finAngle;
        var deltaRotation = gameSettings.TurnSpeed * Time.deltaTime * sign;
        if (Mathf.Abs(finAngle) <= Mathf.Abs(deltaRotation))
        {
            deltaRotation = finAngle;
        }
        transform.Rotate(Vector3.up, deltaRotation);
        return deltaRotation;
    }



    #endregion


    #region Avoidance

    private void CalculateRaysVectors()
    {
        var raysCount = botSettings.RaysCount;
        var anglePerRay = botSettings.AnglePerRay;
        
        var rayMaxAngle =  raysCount * anglePerRay;
        if (rayMaxAngle > 90f)
            anglePerRay = 90f / (raysCount - 1);

        rays[0].vector = new Vector3(0, 0, 1);
        rays[0].modifier = 0.5f;

        var count = 0f;
        for (var i = 1; i < raysCount; i++)
            count += i;

        var oneDivCount = 1f / count;
        
        for (var i = 1; i < raysCount; i++)
        {
            var angle = i * anglePerRay;
            var radAngle = Mathf.Deg2Rad * (90f + angle);
            var sin = Mathf.Sin(radAngle);
            var cos = Mathf.Cos(radAngle);
            
            var modifier = (raysCount - i) * oneDivCount;
            
            rays[i * 2 - 1].vector = new Vector3(cos, 0, sin);
            rays[i * 2 - 1].modifier = modifier;
            rays[i * 2].vector = new Vector3(-cos, 0, sin);
            rays[i * 2].modifier = -modifier;

        }
    }

    private void CalculateAvoidAngle()
    {
        var transf = transform;
        var pos = transf.position;
        pos.y += botSettings.RaysOffset;
        
        Ray ray;
        prevAvoidAngle = avoidRotAngle;
        avoidRotAngle = 0f;
        RaycastHit hitInfo;

        float strength;
        var rayDistance = botSettings.RayDistance;
        var raycastMask = botSettings.RaycastMask;
        var centralRayDistance = botSettings.CentralRayDistance;
        var modifier = 0f;
        
        Transform hitTransf;
        for (var i = 1; i < botSettings.RaysCount; i++)
        {
            ray = new Ray(pos, transf.TransformDirection(rays[i * 2 - 1].vector));
            if (Physics.Raycast(ray, out hitInfo, rayDistance, raycastMask))
            {
                strength = (rayDistance - hitInfo.distance) * oneDivRayDistance;
                hitTransf = hitInfo.transform;
                if (hitTransf.CompareTag("PlankNavigation"))
                {
                    if(bot.NavigationSphere.PlanksTransforms.Contains(hitTransf))
                        modifier -= rays[i * 2 - 1].modifier * strength;
                }
                else
                {
                    modifier += rays[i * 2 - 1].modifier * strength;
                }
            }
            ray = new Ray(pos,  transf.TransformDirection(rays[i*2].vector));
            if (Physics.Raycast(ray, out hitInfo, rayDistance, raycastMask))
            {
                strength = (rayDistance - hitInfo.distance) * oneDivRayDistance;
                hitTransf = hitInfo.transform;
                if (hitInfo.transform.CompareTag("PlankNavigation"))
                {
                    if(bot.NavigationSphere.PlanksTransforms.Contains(hitTransf))
                        modifier -= rays[i * 2].modifier * strength;
                }
                else
                {
                    modifier += rays[i * 2].modifier * strength;
                }
            }
        }
        ray = new Ray(pos, transf.forward);
        if (Physics.Raycast(ray, out hitInfo, centralRayDistance, raycastMask))
        {
            strength = (centralRayDistance - hitInfo.distance) / centralRayDistance;
            hitTransf = hitInfo.transform;
            if (hitInfo.transform.CompareTag("PlankNavigation"))
            {
                if(bot.NavigationSphere.PlanksTransforms.Contains(hitTransf))
                    modifier -= (Mathf.Sign(modifier) * rays[0].modifier) * strength;
            }
            else
            {
                modifier += (Mathf.Sign(modifier) * rays[0].modifier) * strength;
                if (hitTransf.CompareTag("Spinning Obstacle"))
                {    
                    if (cashedSpinObsTrasf != hitInfo.transform)
                    {
                        cashedSpinObsTrasf = hitInfo.transform;
                        //cashedSpinObs = hitTransf.GetComponent<SpinningObstacle>(); //TODO: replace //TODO: recheck
                        cashedSpinObsPos = cashedSpinObsTrasf.position;
                    }
                
                    var posSign = -Mathf.Sign(CalculateRotAngle(cashedSpinObsPos));
                    //var obsSign = -Mathf.Sign(cashedSpinObs.RotPerSec);
                    var obsSign = 1f;
                    modifier = posSign * obsSign * Mathf.Abs(modifier);  //TODO: recheck
                }

            }
            
        }

        avoidRotAngle = modifier * botSettings.MaxAvoidAngle;
        avoidRotAngle = Mathf.Lerp(prevAvoidAngle, avoidRotAngle, botSettings.AngleLerp);

    }

    #endregion



}