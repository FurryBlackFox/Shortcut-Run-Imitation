using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    [SerializeField, Range (0f, 180f)] private float maxAngle = 15f;
    [SerializeField, Range(-5, 5)] private float speed = 2f;

    private Vector3 startRotation;

    private void Awake()
    {
        startRotation = transform.rotation.eulerAngles;
    }

    void FixedUpdate()
    {
        var angle = maxAngle * Mathf.Sin( Time.time * speed);
        var rotation = startRotation + Vector3.right * angle;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
