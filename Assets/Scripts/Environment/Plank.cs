using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plank : MonoBehaviour
{
    public event Action OnPickUp;
    public event Action OnSpawn;
    
    [SerializeField] private MeshRenderer meshRenderer = default;
    
    private Collider cashedCollider;
    private bool isPickedUp = false;
    private Material material = default;
    public bool IsPickedUp
    {
        get => isPickedUp;
        set
        {
            isPickedUp = value;
            cashedCollider.enabled = !value;
            if (!value)
            {
                OnSpawn?.Invoke();
            }
            else
            {
               OnPickUp?.Invoke();
            }
        }
    }

    public Material Material
    {
        get => material;
        set
        {
            material = value;
            meshRenderer.material = value;
        }
    }


    private void Awake()
    {
#if UNITY_EDITOR
        
            CustomTools.IsNull(meshRenderer, nameof(meshRenderer), name);

#endif

        cashedCollider = GetComponent<Collider>();
        cashedCollider.enabled = false;
    }


}
