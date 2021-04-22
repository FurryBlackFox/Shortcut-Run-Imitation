using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerMovementController : CharacterMovementController
{

    private Vector2 input;
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }
    
    internal override void OnFixedUpdateHandler()
    {
        base.OnFixedUpdateHandler();
        CalculateMovement();
        ApplyGravity();
        ApplyExternalForces();
        characterController.Move(movement * Time.fixedDeltaTime);
    }
    
    internal void ApplyInput(Vector2 playerInput)
    {
        input = playerInput;
    }
    
    internal void Jump() //TODO: delete
    {
        base.Jump(gameSettings.JumpHeight);
    }

    internal virtual void CalculateMovement()
    {
        Vector3 forward;
        var direction = input;
        if (isMoving)
        {
            var transf = transform;
        
            if (!player.inputEnabled)
                direction = Vector3.zero;
        
        
            var deltaRotation = gameSettings.TurnSpeed * Time.deltaTime * direction.x;
            transform.Rotate(Vector3.up, deltaRotation);
            forward = transf.forward;  //TODO: smooth acceleration
            if (!player.EnableAutoRun)
                forward *= direction.y;
            
            var speed = gameSettings.DefaultSpeed * currentSpeedMult;
            forward *= speed > gameSettings.MaxSpeed ? gameSettings.MaxSpeed : speed;

            CallMovementEvents(forward.sqrMagnitude, direction.x);
        }
        else
        {
            forward = Vector3.zero;
        }

        movement.x = forward.x;
        movement.z = forward.z;

    }
}
