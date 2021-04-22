using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
[RequireComponent(typeof(CinemachineCameraOffset))]
public class CamController : MonoBehaviour
{
    [SerializeField] private bool isWorking = true;
    [SerializeField] private Vector3 offsetPerPlank = default;
    [SerializeField, Range(0f, 1f)] private float transactionDuration = 0.5f;
    [SerializeField, Range(0, 10f)] private float lookingDelay = 3f;
    [SerializeField] private int maxPlanksCount = 150;

    private CinemachineCameraOffset cmCamOffset;
    private CinemachineVirtualCamera cmCam;
    private Vector3
        oldCamPos = Vector3.zero,
        newCamPos = Vector3.zero;

    private float 
        transactionTimer,
        oneDivTransDuration;
    private Player player;


    private void Awake()
    {
        cmCamOffset = GetComponent<CinemachineCameraOffset>();
        cmCamOffset.m_Offset = Vector3.zero;
        cmCam = GetComponent<CinemachineVirtualCamera>();
        player = GameDataKeeper.S.Player;
        var playerTransf = player.transform;
        cmCam.LookAt = playerTransf;
        cmCam.Follow = playerTransf;
    }

    private void Start()
    {
        CorrectCamPos(0);
    }

    private void OnEnable()
    {
        player.OnPlanksOverload += CorrectCamPos;
        player.OnDrowning += StopCamPos;
        player.OnPlayerFinished += ChangeCamPriority;
    }

    private void OnDisable()
    {
        player.OnPlanksOverload -= CorrectCamPos;
        player.OnDrowning -= StopCamPos;
        player.OnPlayerFinished -= ChangeCamPriority;
    }

    private void Update()
    {
        if(oldCamPos == newCamPos)
            return;
        
        transactionTimer += Time.deltaTime * oneDivTransDuration;
        cmCamOffset.m_Offset = Vector3.Lerp(oldCamPos, newCamPos, transactionTimer);

        if (!(transactionTimer > 1)) 
            return;
        
        transactionTimer = 0;
        oldCamPos = newCamPos;
    }

    private void CorrectCamPos(int planksCount)
    {
        if(!isWorking)
            return;

        if (planksCount > maxPlanksCount)
            return;
        
        oldCamPos = cmCamOffset.m_Offset;
        newCamPos = planksCount * offsetPerPlank;
        transactionTimer = 0f;
        oneDivTransDuration = 1 / transactionDuration;
    }

    private void StopCamPos()
    {
        cmCam.Follow = null;
        StartCoroutine(StopLooking());
        
    }

    private IEnumerator StopLooking()
    {
        yield return new WaitForSeconds(lookingDelay);
        cmCam.LookAt = null;
    }

    private void ChangeCamPriority()
    {
        cmCam.Priority = 0;
    }
    
}
