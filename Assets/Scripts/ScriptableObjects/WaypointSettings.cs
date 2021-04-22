using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Waypoint Settings", menuName = "Scriptable Objects/Waypoint Settings", order = 11)]
public class WaypointSettings : ScriptableObject
{
    public bool showWaypointBorders = true;
    public bool showWaypointNames = true;
    
    [SerializeField] private float minBranchCheckDistance = 5f;
    [SerializeField] private float maxBranchCheckDistance = 40f;
    [SerializeField] private float minBranchDistanceCoefficient = 1.5f;

    
    public float MinBranchCheckDistance => minBranchCheckDistance;

    public float MaxBranchCheckDistance => maxBranchCheckDistance;

    public float MinBranchDistanceCoefficient => minBranchDistanceCoefficient;
}
