using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningObstacle : MonoBehaviour
{
    [SerializeField] private float rotPerSec = 15f;

    public float RotPerSec => rotPerSec;

    private void Update()
    {
        var rotation = rotPerSec * Time.deltaTime;
        transform.Rotate(Vector3.up, rotation);
    }
}
