using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bot Settings", menuName = "Scriptable Objects/Bot Settings", order = 7)]
public class BotSettings : ScriptableObject
{
    [Header("Navigation")]
    [SerializeField] private float minWaypSqrDistance = 50f;
    [SerializeField] private float minFinishSqrDistance = 10f;
    [SerializeField, Range(0f, 1f)] private float angleLerp = 0.6f;
    [SerializeField, Range(0f, 1f)] private float navigationAngleMult = 1f;
    [SerializeField] private bool useRandomBranches = false;

    [Header("Obstacle Avoidance")] 
    [SerializeField] private bool avoidEnabled = true;
    [SerializeField, Range(0, 11)] private int raysCount = 5;
    [SerializeField, Range(-1f, 1f)] private float raysVerticalOffset = 0.3f; 
    [SerializeField] private float anglePerRay = 12.5f;
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private float centralRayDistance = 12.5f;
    [SerializeField] private LayerMask raycastMask = default;
    [SerializeField, Range(0f, 1f)] private float avoidLerp = 0.6f;
    [SerializeField, Range(0f, 1f)] private float rayStrMod = 0.6f;
    [SerializeField] private float maxAvoidAngle = 90f;
    
    [Header("VFX")] 
    [SerializeField] private bool enablePlanksTransp = true;
    [SerializeField, Range(0f, 1f)] private float planksTranspMult = 0.3f;
    
    public float MinWaypSqrDistance => minWaypSqrDistance;

    public float MinFinishSqrDistance => minFinishSqrDistance;

    public float AngleLerp => angleLerp;

    public float NavigationAngleMult => navigationAngleMult;


    public bool UseRandomBranches => useRandomBranches;

    public bool AvoidEnabled => avoidEnabled;

    public int RaysCount => raysCount;

    public float RaysVerticalOffset => raysVerticalOffset;

    public float AnglePerRay => anglePerRay;

    public float RayDistance => rayDistance;

    public float CentralRayDistance => centralRayDistance;

    public LayerMask RaycastMask => raycastMask;

    public float AvoidLerp => avoidLerp;

    public float RayStrMod => rayStrMod;

    public float MaxAvoidAngle => maxAvoidAngle;

    public bool EnablePlanksTransp => enablePlanksTransp;

    public float PlanksTranspMult => planksTranspMult;


}
