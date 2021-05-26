using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct Branch
{
    public Waypoint waypoint;
    public int cost;
    public float coefficient;
}

[ExecuteInEditMode]
public class Waypoint : MonoBehaviour
{
    public static bool showWaypointsNames = false;
    public static bool showWaypointBorders = false;
    
    public Waypoint 
        previousWaypoint = default,
        nextWaypoint = default;

    [SerializeField, Range(0f, 9f)] private float width = 8f;
    public float Width => width;

    public float distanceToFinish = -1;

    public List<Branch> branches = new List<Branch>();

    [SerializeField, Range(0f, 1f)] private float branchRatio = 1f;
    public float BranchRatio => branchRatio;
    
    public Vector3 LeftWidth { get; private set; }
    public Vector3 RightWidth { get; private set; }



    private void Start()
    {
        CalculateWidthPoints();
    }

    private void OnValidate()
    {
        CalculateWidthPoints();
    }
    
    

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if(transform.hasChanged)
            CalculateWidthPoints();

        var pos = transform.position;
        
        Gizmos.color = Color.yellow * 0.5f;
        Gizmos.DrawSphere(pos, 0.5f);
        
        
        if(showWaypointsNames)
            CustomTools.DrawString($"{name} \n {distanceToFinish:F1}m", transform.position, 12, Color.white, Color
                .black * 0.75f);
        
        
        if(!showWaypointBorders)
            return;
        
        var thickness = 3f;
        
        CustomTools.DrawThickLine(LeftWidth, RightWidth, Color.white, thickness);

        if (previousWaypoint)
        {
            if(previousWaypoint.transform.hasChanged)
                CalculateWidthPoints();

            CustomTools.DrawThickLine(RightWidth, previousWaypoint.RightWidth, Color.red, thickness);
        }

        if (nextWaypoint)
        {
            if(nextWaypoint.transform.hasChanged)
                CalculateWidthPoints();
            
            CustomTools.DrawThickLine(LeftWidth, nextWaypoint.LeftWidth, Color.green, thickness);
        }
        
        if (branches == null) 
            return;
        
        foreach (var branch in branches)
        {
            if(!branch.waypoint)
                continue;
            var branchPos = branch.waypoint.GetPosition();
            CustomTools.DrawThickLine(GetPosition(), branchPos, Color.blue, thickness);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(LeftWidth, branch.waypoint.LeftWidth);
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(RightWidth, branch.waypoint.RightWidth);
        }
    }
    

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.5f);
        

        if (branches.Count > 0)
        {
            var pos = transform.position;
            pos.y += 1.5f;
            var chance = 100 - (branchRatio * 100);
            CustomTools.DrawString($"{chance}%", pos, 20, Color.white, Color.black);
            chance = branchRatio * 100 / branches.Count;
            foreach (var branch in branches)
            {
                pos = branch.waypoint.transform.position;
                pos.y += 1f;
                CustomTools.DrawString($"{chance}% - {branch.cost}p", pos, 20,
                    Color.white, Color.black);
            }
        }
    }
    
#endif
    
    
    private void CalculateWidthPoints()
    {
        var transf = transform;
        var position = transf.position;
        var offset = transf.right * (Width * 0.5f);
        RightWidth = position + offset;
        LeftWidth = position + -offset;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Vector3 GetRandomPosition() 
    {
        return Vector3.Lerp(LeftWidth, RightWidth, CustomTools.SampleGaussian01());
    }

    public bool IsLast()
    {
        return !nextWaypoint && branches.Count == 0;
    }

}
