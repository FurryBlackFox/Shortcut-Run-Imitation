using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator), typeof(Character))]
public class CharacterAnimator : MonoBehaviour
{
    [Header("Hands IK")]
    [SerializeField] private float planksOffset = 0f;
    [SerializeField] private float weight = 1f;

    private Animator animator;
    private Character character;

    private readonly int
        idSqrSpeed = Animator.StringToHash("SqrSpeed"),
        idRotation = Animator.StringToHash("Rotation"),
        idSpeedMult = Animator.StringToHash("SpeedMult"),
        idHasPlanks = Animator.StringToHash("HasPlanks"),
        idDrowning = Animator.StringToHash("Drowning"),
        idFinished = Animator.StringToHash("Finished"),
        idFunishingResults = Animator.StringToHash("FinishingResults"),
        idClimbing = Animator.StringToHash("Climbing"),
        idJump = Animator.StringToHash("Jump"),
        idInAir = Animator.StringToHash("InAir");

    private Transform plankHoldPoint;

    private Vector3 target;
    

    private bool hasPlanks;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        character = GetComponent<Character>();
    }

    private void OnEnable()
    {
        character.CharacterMovementController.OnMove += SetMovement;
        character.CharacterMovementController.OnRotation += SetRotation;
        character.CharacterPlanksController.HasPlanks += CheckPlanksInHands;
        character.OnDrowning += TriggerDrowning;
        character.OnFinish += SetFinish;
        character.CharacterMovementController.OnLedgeCling += ClimbingToLedge;
        character.CharacterMovementController.OnJump += SetJump;
        character.CharacterPlanksController.OnSpeedChange += ChangeSpeedMult;
    }

    private void OnDisable()
    {
        character.CharacterMovementController.OnMove -= SetMovement;
        character.CharacterMovementController.OnRotation -= SetRotation;
        character.CharacterPlanksController.HasPlanks -= CheckPlanksInHands;
        character.OnDrowning -= TriggerDrowning;
        character.OnFinish -= SetFinish;
        character.CharacterMovementController.OnLedgeCling -= ClimbingToLedge;
        character.CharacterMovementController.OnJump -= SetJump;
        character.CharacterPlanksController.OnSpeedChange -= ChangeSpeedMult;
    }

    private void Start()
    {
        plankHoldPoint = character.CharacterPlanksController.PlankHoldPoint;
    }

    private void Update()
    {
        animator.SetBool(idInAir, !character.IsGrounded());
    }

    private void SetMovement(float speed)
    {
        animator.SetFloat(idSqrSpeed, speed);
        
    }

    private void SetRotation(float rotation)
    {
        animator.SetFloat(idRotation, rotation);
    }

    private void TriggerDrowning()
    {
        animator.SetTrigger(idDrowning);
    }

    private void SetFinish(int result)
    {
        animator.SetTrigger(idFinished);
        animator.SetInteger(idFunishingResults, result);
    }

    private void SetJump()
    {
        animator.SetTrigger(idJump);
    }

    private void CheckPlanksInHands(bool value)
    {
        hasPlanks = value;
        animator.SetBool(idHasPlanks, value);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!hasPlanks)
            return;
        
        target = plankHoldPoint.position;
        var targetOffset = plankHoldPoint.right * planksOffset;
        
        animator.SetIKPosition(AvatarIKGoal.LeftHand, target - targetOffset);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
        
        animator.SetIKPosition(AvatarIKGoal.RightHand, target + targetOffset);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
    }

    private void ClimbingToLedge(bool value)
    {
        if(value)
            animator.SetTrigger(idClimbing);
        
        animator.applyRootMotion = value;
    }

    private void ChangeSpeedMult(float mult)
    {
        animator.SetFloat(idSpeedMult, mult);
    }
}
