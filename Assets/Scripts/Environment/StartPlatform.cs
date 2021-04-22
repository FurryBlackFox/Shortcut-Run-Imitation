using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StartPlatform : MonoBehaviour
{
    [SerializeField] private List<Transform> startPoints = default;

    private List<Transform> pointsInUse;
    
    private void Awake()
    {
        if (startPoints.Count == 0)
            Debug.LogError($"{nameof(startPoints)} is not assigned in {name}");
        
        pointsInUse = new List<Transform>();
        pointsInUse.AddRange(startPoints);
    }

    private Transform GetStartPointAt(int index)
    {
        var point = startPoints[index];
        startPoints.Remove(point);
        pointsInUse.Add(point);
        return point;
    }

    public Transform GetStartPoint()
    {
        if (startPoints.Count == 0)
        {
            Debug.LogError($"Cant get StartPoint 'cause {nameof(startPoints)} is empty");
            return null;
        }

        return GetStartPointAt(startPoints.Count - 1);
    }

    public Transform GetRandomStartPoint()
    {
        if (startPoints.Count == 0)
        {
            Debug.LogError($"Cant get StartPoint 'cause {nameof(startPoints)} is empty");
            return null;
        }
            
        var index = Random.Range(0, startPoints.Count);
        return GetStartPointAt(index);
    }
}
