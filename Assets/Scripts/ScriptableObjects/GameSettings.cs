using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Settings", menuName = "Scriptable Objects/Game Settings", order = 3)]
public class GameSettings : ScriptableObject
{
    #region EditorVariables
    
    [Header("Movement"), Header("CHARACTERS")]
    [SerializeField, Range(0f, 20f)] private float defaultSpeed = 10f;
    [SerializeField, Range(1f, 1.1f)] private float speedMultPerCheck = 1.01f;
    [SerializeField, Range(0f, 2f)] private float speedMultCheckDelay = 0.5f;
    [SerializeField, Range(0f, 30f)] private float maxSpeed = 20f;
    [SerializeField, Range(0f, 360f)] private float turnSpeed = 180f;
    [SerializeField, Range(0f, 5f)] private float jumpHeight = 2f;
    [SerializeField, Range(0f, 25f)] private float gravity = 20f;
    [SerializeField, Range(0f, 25f)] private float waterGravity = 10f;

    [Header("Cling")] 
    [SerializeField, Range(0f, 2f)] private float clingDistance = 1f;
    [SerializeField, Range(0f, 5f)] private float verticalOffset = 0.2f;
    [SerializeField, Min(0)] private float colliderChangeDuration = 0.1f;
    [SerializeField] private LayerMask clingMask = default;

    [Header("Planks")] 
    [SerializeField] private Material planksOpaqueMaterial = default;
    [SerializeField] private Material planksTransparentMaterial = default;
    [SerializeField] private bool respawnPlanks = true;
    [SerializeField, Range(1f, 10f)] private float plankRespawnTime = 5f;
    [SerializeField, Range(0f, 1f)] private float plankStackOffset = 0.2f;
    [SerializeField, Range(0f, 1f)] private float plankStackScale = 0.5f;
    [SerializeField] private bool isFixedSpawnOffset = true;
    [SerializeField] private float fixedSpawnOffest = -0.0499f;
    [SerializeField, Range(0, 30)] private int oversizeCount = 20;
    [SerializeField, Min(0f)] private float planksPerMeter = 2f;
    [SerializeField, Range(0f, 0.001f)] protected float planksStackBendMult = 0.00015f;
    [SerializeField, Min(0)] protected int planksStackBendOffset = 30;
    
    [Header("Raycast")]
    [SerializeField, Range(0f, 1f)] private float raycastDistance = 1f;
    [SerializeField] private Vector3 raycastBoxSize = default;
    [SerializeField] private Vector3 footRaycastBoxSize = default;
    [SerializeField] private LayerMask raycastMask = default;

    [Header("Bot Navigation")] 
    [SerializeField, Range(0f, 1f)] private float planksCollectionMod = 0.5f;
    [SerializeField, Min(0)] private int stackNavigationColliderActivationThreshold = 5;
    
    [Header("Physics")]
    [SerializeField, Range(0f, 250f)] protected float pendulumCollisionForce = 75f;
    [SerializeField, Range(0f, 1f)] protected float pendulumVerticalModifier = 0.25f;
    [SerializeField, Range(0f, 1f)] protected float minMoveDistByExtForce = 0.3f;
    [SerializeField, Range(0f, 10f)] protected float forceDecreaseMult = 3f;

    [Header("ECONOMICS")]
    [SerializeField, Min(2)] private int extraPointsMultiplier = 3;



    #endregion

    
    #region Properties

    public float DefaultSpeed => defaultSpeed;

    public float SpeedMultPerCheck => speedMultPerCheck;

    public float SpeedMultCheckDelay => speedMultCheckDelay;

    public float MaxSpeed => maxSpeed;
    
    public float TurnSpeed => turnSpeed;
    
    public float JumpHeight => jumpHeight;
    
    public float Gravity => gravity;

    public float WaterGravity => waterGravity;

    public float PlankStackOffset => plankStackOffset;

    public float PlankStackScale => plankStackScale;

    public bool IsFixedSpawnOffset => isFixedSpawnOffset;

    public float FixedSpawnOffest => fixedSpawnOffest;

    public int OversizeCount => oversizeCount;
    
    public float PlanksPerMeter => planksPerMeter;

    public float RaycastDistance => raycastDistance;

    public Vector3 RaycastBoxSize => raycastBoxSize;

    public Vector3 FootRaycastBoxSize => footRaycastBoxSize;
    
    public LayerMask RaycastMask => raycastMask;

    public float ClingDistance => clingDistance;

    public LayerMask ClingMask => clingMask;

    public Material PlanksOpaqueMaterial => planksOpaqueMaterial;

    public Material PlanksTransparentMaterial => planksTransparentMaterial;
    
    public bool RespawnPlanks => respawnPlanks;

    public float PlankRespawnTime => plankRespawnTime;

    public float VerticalOffset => verticalOffset;
    
    public float ColliderChangeDuration => colliderChangeDuration;

    
    public float PlanksStackBendMult => planksStackBendMult;

    public int PlanksStackBendOffset => planksStackBendOffset;
    
    public float PlanksCollectionMod => planksCollectionMod;

    public int StackNavigationColliderActivationThreshold => stackNavigationColliderActivationThreshold;
    
    public float PendulumCollisionForce => pendulumCollisionForce;

    public float PendulumVerticalModifier => pendulumVerticalModifier;

    public float MinMoveDistByExtForce => minMoveDistByExtForce;

    public float ForceDecreaseMult => forceDecreaseMult;

    public int ExtraPointsMultiplier => extraPointsMultiplier;

    #endregion
    

}
