using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class CharacterMovementController : MonoBehaviour
{
    public event Action<float> OnMove;
    public event Action<float> OnRotation;
    public event Action OnJump;
    public event Action<bool> OnLedgeCling;
    
    [Header("Cling Settings")] 
    [SerializeField] protected Transform lowerClingRayPoint = default;
    [SerializeField] protected Transform upperClingRayPoint = default;
    [SerializeField] protected Color gizmosColor = default;
    

    
    protected Character character;
    protected CharacterController characterController;
    protected GameSettings gameSettings;
    
    protected Vector3 movement;
    protected Vector3 externalForces;

    
    protected bool inWater = false;
    protected bool isMoving = false; //TODO: replace
    protected bool isClinging = false;
    
    protected float rotAngle;
    protected float cashedVerticalForce;
    internal float currentSpeedMult;

    protected virtual void Awake()
    {
        character = GetComponent<Character>();
        characterController = GetComponent<CharacterController>();
        gameSettings = GameDataKeeper.S.GameSettings;

 
    }

    protected virtual void OnEnable()
    {
        GameManager.OnGameStart += OnGameStartHandler;
        character.OnDrowning += OnDrowningHandler;
    }

    protected virtual void OnDisable()
    {
        GameManager.OnGameStart -= OnGameStartHandler;
        character.OnDrowning -= OnDrowningHandler;
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void OnDrawGizmos()
    {
        if (!gameSettings)
            gameSettings = GameDataKeeper.S.GameSettings;
        Gizmos.color = gizmosColor;
        Gizmos.DrawRay(lowerClingRayPoint.position, lowerClingRayPoint.forward*gameSettings.ClingDistance);
        Gizmos.DrawRay(upperClingRayPoint.position, upperClingRayPoint.forward*gameSettings.ClingDistance);
    }


    #region Movement

    protected void OnGameStartHandler()
    {
        isMoving = true;
    }

    protected void OnDrowningHandler()
    {
        inWater = true;
        isMoving = false;

        movement.x = 0f;
        movement.z = 0f;
    }
    
    
    internal void OnFinishHandler(Vector3 cameraPos)
    {
        isMoving = false;
        StartCoroutine(RotateToTargetWithDelay(cameraPos, 1f)); //TODO: remove delay
    }


    internal virtual void OnFixedUpdateHandler()
    {
        CheckForLedge();
    }

    protected IEnumerator RotateToTargetWithDelay(Vector3 target, float startDelay)
    {
        yield return new WaitForSeconds(startDelay);
        var delay = new WaitForEndOfFrame();
        rotAngle = CalculateRotAngle(target);
        while (Mathf.Abs(rotAngle) > 0)
        {
            Rotate();
            yield return delay;
        }
    }
    
    protected float CalculateRotAngle(Vector3 target)
    {
        var transf = transform;
        var direction = target - transf.position;
        direction.y = 0;
        var angle = Vector3.SignedAngle(transf.forward, direction, Vector3.up);
        return angle;
    }

    protected virtual void Rotate()
    {
        var sign = Mathf.Sign(rotAngle);
        var deltaRotation = gameSettings.TurnSpeed * Time.deltaTime * sign;
        if (Mathf.Abs(rotAngle) <= Mathf.Abs(deltaRotation))
        {
            deltaRotation = rotAngle;
        }
        rotAngle -= deltaRotation;
        transform.Rotate(Vector3.up, deltaRotation);
    }

    protected void ApplyGravity()
    {
        if(isClinging)
            return;
        
        var gravity = inWater ? gameSettings.WaterGravity : gameSettings.Gravity;
        
        if (characterController.isGrounded)
        {
             if(movement.y <= 0f)
                 movement.y = -gravity * 0.01f;
        }
        else
        {
            if (movement.y > -gravity)
            {
                movement.y -= gravity * Time.fixedDeltaTime;
            }
            else
            {
                movement.y = -gravity;
            }  
        }
    }

    protected void ApplyExternalForces()
    {
        if (isClinging || inWater)
            externalForces = Vector3.zero;
        var minMoveDistByExtForce = gameSettings.MinMoveDistByExtForce;
        if (externalForces.sqrMagnitude < minMoveDistByExtForce * minMoveDistByExtForce)
            return;

        var horizontalForce = externalForces;
        horizontalForce.y = 0;
        var force = horizontalForce;
        
        force.y -= cashedVerticalForce;
        var verticalForce = externalForces.y;
        cashedVerticalForce = verticalForce;
        force.y += verticalForce;

        movement += force;
        
        externalForces = Vector3.Lerp(externalForces, Vector3.zero, gameSettings.ForceDecreaseMult * Time.deltaTime);
    }

    internal void AddForce(Vector3 force)
    {
        externalForces += force;
    }

    internal void Jump(float height)
    {
        
        if (!characterController.isGrounded)
            return;
        var force = Mathf.Sqrt(gameSettings.Gravity * 2 * height);
        movement.y = force;
        
        OnJump?.Invoke();
    }

    protected void CallMovementEvents(float move, float rotation)
    {
        OnMove?.Invoke(move);
        OnRotation?.Invoke(rotation);
    }
    
    
    #endregion
    
    
    
    #region Climbing

    internal void CheckForLedge()
    {
        if(characterController.isGrounded)
            return;
        
        if(isClinging)
            return;
        
        if (!Physics.Raycast(lowerClingRayPoint.position, transform.forward, out var lowerHitInfo, 
            gameSettings.ClingDistance, gameSettings.ClingMask))    
            return;

        if (!Physics.Raycast(upperClingRayPoint.position, transform.forward,
            gameSettings.ClingDistance, gameSettings.ClingMask))
        {
            var newPos = upperClingRayPoint.position;
            var offset = upperClingRayPoint.forward * (lowerHitInfo.distance + 0.1f);
            newPos += offset;
            Physics.Raycast(newPos, -transform.up, out var hitInfo, 1f, gameSettings.ClingMask);
            ClimbToLedge(lowerHitInfo.point, hitInfo.point.y);
        }
    }


    private float prevHeight, prevRadius;
    
    protected void ClimbToLedge(Vector3 point, float height) //TODO: do smth better
    {
        isMoving = false;
        isClinging = true;
        movement = Vector3.zero;
        
        
        var transf = transform;
        
        transf.Rotate(transf.up, CalculateRotAngle(point));

        prevHeight = characterController.height;
        prevRadius = characterController.radius;
        var forward = transf.forward * prevRadius;
        var pos = transf.position;
        var vertOffset = transf.up * (height - pos.y - prevHeight * 0.5f + gameSettings.VerticalOffset);
        forward += vertOffset;

        characterController.radius = 0.1f;
        characterController.height = 0.8f;
        characterController.Move(forward);
        
        
        OnLedgeCling?.Invoke(true);

    }

    
    public void EndOfClimbing()
    {
        StartCoroutine(RestoreCollider());
    }
    
    
    private IEnumerator RestoreCollider()
    {
        var t = 0f;
        var currentRadius = characterController.radius;
        var currentHeight = characterController.height;
        var oneDivDuration = 1f / gameSettings.ColliderChangeDuration;
        while (t <= 1f)
        {
            t += Time.fixedDeltaTime * oneDivDuration;
            characterController.radius = Mathf.Lerp(currentRadius, prevRadius, t);
            characterController.height = Mathf.Lerp(currentHeight, prevHeight, t);    
            yield return new WaitForFixedUpdate();
        }
        isMoving = true;
        isClinging = false;
        OnLedgeCling?.Invoke(false);
    }

    #endregion
    
    #region Public
    
    public void SetPosition(Vector3 pos)
    {
        pos.y += characterController.height * 0.5f;
        transform.position = pos;
    }

    public bool IsGrounded()
    {
        return characterController.isGrounded;
    }

    #endregion
}
