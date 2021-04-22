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
    [SerializeField] protected TextMeshProUGUI UICharacterNameText = default;

    protected string characterName;
    public string CharacterName => characterName;


    protected CharacterController characterController;
    public CharacterController CharacterController => characterController;
    protected CharacterPlanksController characterPlanksController;
    public CharacterPlanksController CharacterPlanksController => characterPlanksController;
    protected CharacterMovementController characterMovementController;
    public CharacterMovementController CharacterMovementController => characterMovementController;
    protected GameSettings gameSettings;

    #endregion

    protected virtual void Awake()
    {
        gameSettings = GameDataKeeper.S.GameSettings;
        characterController = GetComponent<CharacterController>();
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
        if (other.CompareTag("Plank"))
        {
            var triggeredPlank = other.GetComponentInParent<TriggerPlank>();
            triggeredPlank.Trigger();
            characterPlanksController.AddPlank();
        }
        else if (other.CompareTag("Finish"))
        {
            
            var finish = other.GetComponent<Finish>();
            finish.RegisterCharacter(this);
            var position = finish.GetFinishingResult(this);
            characterMovementController.SetPosition(finish.GetFinishPosition(position)); //TODO: smooth movement
            characterMovementController.OnFinishHandler(finish.GetCameraPosition());
            characterPlanksController.HidePlanks();
            OnFinish?.Invoke(position);
;        }
        else if (other.CompareTag("Water"))
        {
            OnDrowning?.Invoke();
        }
        else if (other.CompareTag("Pendulum"))
        {
            var position = transform.position;
            var pendulumPos = other.ClosestPoint(position);
            var direction = position - pendulumPos;
            direction.y = 0;
            direction.Normalize();
            direction = Vector3.Lerp(direction, Vector3.up, gameSettings.PendulumVerticalModifier);
            characterMovementController.AddForce(direction * gameSettings.PendulumCollisionForce);
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

    protected virtual void OnDrawGizmos()
    {
        
    }


    internal void Jump(float height)
    {
        characterMovementController.Jump(height);
    }

    public void SetPosition(Vector3 position)
    {
        characterMovementController.SetPosition(position);
    }

    private void OnSpeedChangeHandler(float value)
    {
        characterMovementController.currentSpeedMult = value;
    }
    
}
