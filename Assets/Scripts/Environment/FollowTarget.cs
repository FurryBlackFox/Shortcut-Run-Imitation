using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private bool constY = false;
    private Transform target;
    private float yPos;
    private void Start()
    {
        target = GameDataKeeper.S.Player.transform;
        yPos = transform.position.y;
    }

    private void LateUpdate()
    {
        var pos = target.transform.position;
        if (constY)
            pos.y = yPos;
        transform.position = pos;
    }
}
