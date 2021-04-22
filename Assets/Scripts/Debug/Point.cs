using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Point : MonoBehaviour
{
    [SerializeField] private bool isDrawing = true;
    [SerializeField] private float size = 0.1f;
    [SerializeField] private Color color = Color.blue;
    private void OnDrawGizmos()
    {
        if(!isDrawing) return;
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, size);
    }
}
