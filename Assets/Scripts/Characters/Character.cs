using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(CharacterController), typeof(CharacterMovementController), 
    typeof(CharacterPlanksController))]
public class Character : MonoBehaviour
{
    #region Variables
    
    public event Action<int> OnFinish; //TODO: check
    public event Action OnDrowning;
    public event Action OnEmerge;
    
    
    [Header("Character Canvas")] 
    [SerializeField] protected TextMeshProUGUI uiCharacterNameText = default;

    protected string characterName;
    public string CharacterName => characterName;


    protected CharacterPlanksController characterPlanksController;
    public CharacterPlanksController CharacterPlanksController => characterPlanksController;
    protected CharacterMovementController characterMovementController;
    public CharacterMovementController CharacterMovementController => characterMovementController;
    protected GameSettings gameSettings;

    #endregion

    protected virtual void Awake()
    {
        gameSettings = GameDataKeeper.S.GameSettings;
        characterMovementController = GetComponent<CharacterMovementController>();
        characterPlanksController = GetComponent<CharacterPlanksController>();
    }
    

    protected virtual void OnEnable()
    {
        characterPlanksController.OnSpeedChange += OnSpeedChangeHandler;
    }

    protected virtual void OnDisable()
    {
        characterPlanksController.OnSpeedChange -= OnSpeedChangeHandler;
    }
    
    
    protected virtual void Start()
    {
        OnEmerge?.Invoke();
    }



    protected virtual void FixedUpdate()
    {
        characterMovementController.OnFixedUpdateHandler();
        characterPlanksController.OnFixedUpdateHandler();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Plank":
                OnCollisionWithPlankHandler(other);
                break;
            case "Finish":
                OnCollisionWithFinishHandler(other);
                break;
            case "Water":
                OnDrowning?.Invoke();
                break;
            case "Pendulum":
                OnCollisionWithPendulumHandler(other);
                break;
        }
    }
    
    protected void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("JumpPoint"))
        {
            var height = other.GetComponent<JumpPoint>().JumpHeight;
            characterMovementController.Jump(height);
        }
    }

    protected void OnCollisionEnter(Collision other)
    {
        Debug.Log("collision");
    }


    protected virtual void OnDrawGizmos()
    {
        
    }


    internal void Jump(float height)
    {
        characterMovementController.Jump(height);
    }

    private void OnSpeedChangeHandler(float value)
    {
        characterMovementController.currentSpeedMult = value;
    }
    
    private void OnCollisionWithPlankHandler(Collider plankCollider)
    {
        var triggeredPlank = plankCollider.GetComponentInParent<TriggerPlank>();
        triggeredPlank.Trigger();
        characterPlanksController.AddPlank();
    }

    private void OnCollisionWithFinishHandler(Collider finishCollider)
    {
        var finish = finishCollider.GetComponent<Finish>();
        finish.RegisterCharacter(this);
        var position = finish.GetFinishingResult(this);
        characterMovementController.SetPosition(finish.GetFinishPosition(position)); //TODO: smooth movement
        characterMovementController.OnFinishHandler(finish.GetCameraPosition());
        characterPlanksController.HidePlanks();
        OnFinish?.Invoke(position);
    }

    private void OnCollisionWithPendulumHandler(Collider pendulumCollider)
    {
        var position = transform.position;
        var pendulumPos = pendulumCollider.ClosestPoint(position);
        var direction = position - pendulumPos;
        direction.y = 0;
        direction.Normalize();
        direction = Vector3.Lerp(direction, Vector3.up, gameSettings.PendulumVerticalModifier);
        characterMovementController.AddForce(direction * gameSettings.PendulumCollisionForce);
    }

    

    #region Public

    public bool IsGrounded()
    {
        return characterMovementController.IsGrounded();
    }
    
    
    public void SetPosition(Vector3 position)
    {
        characterMovementController.SetPosition(position);
    }


    #endregion
    
}
