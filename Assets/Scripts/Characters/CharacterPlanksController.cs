using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Character), typeof(CharacterController))]
public class CharacterPlanksController : MonoBehaviour
{
    public event Action<bool> HasPlanks;
    public event Action<float> OnSpeedChange;
    
    [Header("Planks")] 
    [SerializeField] protected Transform plankHoldPoint = default;
    public Transform PlankHoldPoint => plankHoldPoint;
    [SerializeField] protected Transform plankSpawnPoint = default;
    
    [Header("Raycast")] 
    [SerializeField] protected Transform frontRaycastPoint = default;
    [SerializeField] protected Transform behindRaycastPoint = default;
    [SerializeField] protected Color gizmosColor = default;
    
    protected static Transform plankParent;
    protected List<Plank> planks;
    internal int PlanksCount => planks.Count;
    protected bool plankSpawned = false;
    
    protected Character character;
    protected GameSettings gameSettings;
    protected CharacterController characterController;
    protected float controllerStartHeight = 0f;

    
    internal Material planksMaterialOpaque;
    internal Material planksMaterialTransparent;
    internal Material currentPlanksMaterial;
    
    protected float currentSpeedMult = 1f;
    protected WaitForSeconds speedMultCheckDelay;
    
    
    protected virtual void Awake()
    {
        character = GetComponent<Character>();
        characterController = GetComponent<CharacterController>();
        
        planks = new List<Plank>();
        if (!plankParent)
        {
            plankParent = new GameObject("Plank Parent").transform;
        }
        
        gameSettings = GameDataKeeper.S.GameSettings;
        plankHoldPoint.localScale = Vector3.one * gameSettings.PlankStackScale;
        speedMultCheckDelay = new WaitForSeconds(gameSettings.SpeedMultCheckDelay);
    }
    


    protected virtual void Start()
    {
        StartCoroutine(CheckForSpawnedPlanks());
    }

    private void OnDrawGizmos()
    {
        if (!gameSettings)
            gameSettings = GameDataKeeper.S.GameSettings;
        Gizmos.color = gizmosColor;
        Gizmos.DrawCube(frontRaycastPoint.position, gameSettings.RaycastBoxSize);
        Gizmos.DrawCube(behindRaycastPoint.position, gameSettings.FootRaycastBoxSize);
    }


    internal void OnFixedUpdateHandler()
    {
        CheckForGround();
        BendPlankStack();
    }
    
    internal virtual Vector3 AddPlank()
    {
        var newPlank = GameDataKeeper.S.PlankObjectPool.GetPlank();
        newPlank.Material = currentPlanksMaterial;
        newPlank.IsPickedUp = true;
        planks.Add(newPlank);
        var plankTransform = newPlank.transform;
        plankTransform.SetParent(plankHoldPoint, false);
        var offset = planks.Count * gameSettings.PlankStackOffset;
        plankTransform.localPosition = new Vector3(0, offset, 0);
        if(planks.Count == 1)
            HasPlanks?.Invoke(true);
        return plankTransform.position;
    }

    internal virtual void CheckForGround()
    {
        var yPos = transform.position.y;
        if (!characterController.isGrounded && yPos > controllerStartHeight)
            return;
        if (characterController.isGrounded)
            controllerStartHeight = yPos + 0.5f;
        if (!Physics.BoxCast(frontRaycastPoint.position, gameSettings.RaycastBoxSize, Vector3.down,
            transform.rotation, gameSettings.RaycastDistance, gameSettings.RaycastMask))
        {
            if (planks.Count > 0)
            {
                SpawnPlank(plankSpawnPoint);
            }
            else
            {
                character.Jump(gameSettings.JumpHeight);
            }
        }

        if (!Physics.BoxCast(behindRaycastPoint.position, gameSettings.FootRaycastBoxSize, Vector3.down,
            transform.rotation, gameSettings.RaycastDistance, gameSettings.RaycastMask))
        {
            if (planks.Count > 0)
            {
                SpawnPlank(plankSpawnPoint);
            }
        }
    }

    internal virtual void SpawnPlank(Transform point)
    {
        if (!gameSettings.IsFixedSpawnOffset)
            SpawnPlankAt(point.position);
        else
        {
            var pos = point.position;
            pos.y = gameSettings.FixedSpawnOffest;
            SpawnPlankAt(pos);
        }
    }

    internal virtual void SpawnPlankAt(Vector3 pos)
    {
        var index = planks.Count - 1;
        var plank = planks[index];
        planks.RemoveAt(index);
        plank.IsPickedUp = false;
        plank.Material = planksMaterialOpaque;
        var plankTransform = plank.transform;
        plankTransform.parent = plankParent;
        plankTransform.rotation = characterController.transform.rotation;
        plankTransform.position = pos;
        plankTransform.localScale = Vector3.one;
        plankSpawned = true;
        if (planks.Count == 0)
            HasPlanks?.Invoke(false);
    }

    internal void HidePlanks()
    {
        foreach (var plank in planks)
        {
            plank.gameObject.SetActive(false);
        }
        HasPlanks?.Invoke(false);
    }

    internal IEnumerator CheckForSpawnedPlanks()
    {
        while (this)
        {
            currentSpeedMult = plankSpawned ? currentSpeedMult * gameSettings.SpeedMultPerCheck : 1f;
            plankSpawned = false;
            
            OnSpeedChange?.Invoke(currentSpeedMult); //todo:uncomment
            yield return speedMultCheckDelay;
        }
    }

    internal void BendPlankStack()
    {
        var cos = Mathf.Cos(Time.time);
        Vector3 newPos;
        var planksStackBendOffset = gameSettings.PlanksStackBendOffset;
        for (var i = planksStackBendOffset; i < planks.Count; i++)
        {
            newPos = planks[i].transform.localPosition;
            var j = i - planksStackBendOffset;
            newPos.x = gameSettings.PlanksStackBendMult * cos * j * j;
            planks[i].transform.localPosition = newPos;
        }
    }
}
