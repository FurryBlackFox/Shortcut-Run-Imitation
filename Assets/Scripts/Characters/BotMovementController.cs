using System;
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
    [SerializeField] private Color raysColor = Color.blue;
    [SerializeField] private bool showTarget = true;
    [SerializeField] private Color targetColor = Color.red;
    [SerializeField] private bool showCashedObsPosition = true;
    [SerializeField] private Color cashedObsPosColor = Color.cyan;


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


        if (showTarget)
        {
            Gizmos.color = targetColor;
            Gizmos.DrawSphere(target, Mathf.Sqrt(currMinWaypSqrDistance));
        }

        if (showCashedObsPosition)
        {
            Gizmos.color = cashedObsPosColor;
            Gizmos.DrawSphere(cashedSpinObsPos, 1f);
        }

        
        if (Application.isPlaying && showRays)
        {
            Gizmos.color = raysColor;
            var position = transform.position;
            position.y += botSettings.RaysVerticalOffset;
            
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
        var sqrDistance = CalculateSqrDistance(target);
        if (sqrDistance > currMinWaypSqrDistance)
        {
            rotAngle = CalculateRotAngle(target);
            if (botSettings.AvoidEnabled)
                CalculateAvoidAngle();
            CallMovementEvents(MoveForward(),Rotate());
        }
        else
        {
            targetWaypoint = ChooseNextTarget(targetWaypoint, bot.PlanksCount);
            if (targetWaypoint)
            {
                target = targetWaypoint.GetRandomPosition();
                currMinWaypSqrDistance = targetWaypoint.IsLast()
                    ? botSettings.MinFinishSqrDistance
                    : botSettings.MinWaypSqrDistance;
            }
        }
    }

    protected void SetStartTarget()
    {
        targetWaypoint = enterWaypoint;
        target = targetWaypoint.GetRandomPosition();
        currMinWaypSqrDistance = botSettings.MinWaypSqrDistance;
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
            var index = botSettings.UseRandomBranches ? Random.Range(0, availableBranches.Count) : 0;
            return availableBranches[index].waypoint;
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
        pos.y += botSettings.RaysVerticalOffset;
        
        Ray ray;
        prevAvoidAngle = avoidRotAngle;
        avoidRotAngle = 0f;
        
        var rayDistance = botSettings.RayDistance;
        var raycastMask = botSettings.RaycastMask;
        var centralRayDistance = botSettings.CentralRayDistance;
        var modifier = 0f;
        
        var raysCount = botSettings.RaysCount * 2 - 1;
        for (var i = 1; i < raysCount; i++)
        {
            ray = new Ray(pos,  transf.TransformDirection(rays[i].vector));
            modifier += RayProcessing(ray, rayDistance, rays[i].modifier, raycastMask);
        }
        ray = new Ray(pos, transf.forward);
        if (Physics.Raycast(ray, out var hitInfo, centralRayDistance, raycastMask))
        {
            var strength = (centralRayDistance - hitInfo.distance) / centralRayDistance;
            var hitTransf = hitInfo.transform;
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
                        cashedSpinObsPos = cashedSpinObsTrasf.position;
                    }
                
                    var posSign = -Mathf.Sign(CalculateRotAngle(cashedSpinObsPos));
                    modifier = posSign * Mathf.Abs(modifier);  //TODO: recheck
                }
            }
        }

        avoidRotAngle = modifier * botSettings.MaxAvoidAngle;
        avoidRotAngle = Mathf.Lerp(prevAvoidAngle, avoidRotAngle, botSettings.AngleLerp);
    }
    
    


    private float RayProcessing(Ray ray, float rayDistance, float rayModifier , LayerMask raycastMask)
    {
        if (Physics.Raycast(ray, out var hitInfo, rayDistance, raycastMask))
        {
            var strength = (rayDistance - hitInfo.distance) * oneDivRayDistance;
            var hitTransf = hitInfo.transform;
            if (hitTransf.CompareTag("PlankNavigation"))
            {
                if(bot.NavigationSphere.PlanksTransforms.Contains(hitTransf))
                    return -rayModifier * strength;
            }
            else
            {
                return rayModifier * strength;
            }
        }
        return 0f;
    }

    #endregion



}
