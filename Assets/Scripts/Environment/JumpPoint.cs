using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPoint : MonoBehaviour
{
    [SerializeField, Range(0f, 25f)] private float jumpHeight = 10f;

    public float JumpHeight => jumpHeight;

}

