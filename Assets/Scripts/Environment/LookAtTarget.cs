using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


public class LookAtTarget : MonoBehaviour
{
    enum Target
    {
        Player,
        Camera
    }

    [SerializeField] private bool constY = false;
    [SerializeField] private Target target = Target.Player;
    private Transform mainCameraTransform;
    private Transform playerTransform;

    private void Start()
    {
        var mainCamera = GameDataKeeper.S.CinemachineBrain.ActiveVirtualCamera;
        if(mainCamera != null)
            mainCameraTransform = mainCamera.VirtualCameraGameObject.transform;
        else
            Debug.LogError($"Camera is not assigned in {name}");
        playerTransform = GameDataKeeper.S.Player.transform;
    }

    private void LateUpdate()
    {
        var targetTransf = target == Target.Player ? playerTransform : mainCameraTransform;
        var targetPos = targetTransf.position;
        if (constY)
            targetPos.y = transform.position.y;
        transform.LookAt(targetPos);
        transform.Rotate(0, 180f, 0);
    }
}
