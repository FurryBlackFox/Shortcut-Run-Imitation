using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Character), typeof(Animator), typeof(CharacterVFXController))]
public class CharacterSkinController : MonoBehaviour
{
    public event Action OnSkinChange;
    
    [SerializeField] 
    private List<SkinnedMeshRenderer> currentMeshLods;

 

    public CharacterSkin skin1;
    public CharacterSkin skin2;

    private Transform rootBone;
    private Animator animator;
    
    private Transform rootBoneParent;
    private int rootBoneIndex;

    private Vector3
        rootBoneLocalPos,
        rootBoneLocalScale;

    private Quaternion rootBoneLocalRot;

    private CharacterVFXController characterVfxController;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        characterVfxController = GetComponent<CharacterVFXController>();
        
        rootBone = currentMeshLods[0].rootBone;
        
        rootBoneParent = rootBone.parent;
        rootBoneIndex = rootBone.GetSiblingIndex();

        rootBoneLocalPos = rootBone.localPosition;
        rootBoneLocalRot = rootBone.localRotation;
        rootBoneLocalScale = rootBone.localScale;
    }

    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
            SetSkin(skin1);
        if(Input.GetKeyDown(KeyCode.G))
            SetSkin(skin2);
    }


    public void SetSkin(CharacterSkin newSkin)
    {
        OnSkinChange?.Invoke();
        var instantiatedSkin = Instantiate(newSkin);
        
        ChangeRootBone(instantiatedSkin.MeshLods[0].rootBone);
        ChangeMesh(instantiatedSkin.MeshLods);
        ChangeAvatar(instantiatedSkin.Avatar);
        
        characterVfxController.LeaderCrown = instantiatedSkin.LeaderCrown.gameObject;
        
        Destroy(instantiatedSkin.gameObject);
    }

    private void ChangeRootBone(Transform newRootBone)
    {
        Destroy(rootBone.gameObject);
        
        newRootBone.parent = rootBoneParent;
        newRootBone.SetSiblingIndex(rootBoneIndex);

        newRootBone.localPosition = rootBoneLocalPos;
        newRootBone.localRotation = rootBoneLocalRot;
        newRootBone.localScale = rootBoneLocalScale;
        
        rootBone = newRootBone;
    }

    private void ChangeMesh(List<SkinnedMeshRenderer> newRenderers)
    {
        if (currentMeshLods.Count != newRenderers.Count)
        {
            Debug.LogWarning("Skinned meshes count does not match!");
            return;
        }

        for (var i = 0; i < currentMeshLods.Count; i++)
        {
            var skinnedMeshRendererLod = currentMeshLods[i];
            skinnedMeshRendererLod.rootBone = rootBone;
            
            skinnedMeshRendererLod.bones = newRenderers[i].bones;    
             
            //skinnedMeshRendererLod.localBounds = newRenderers[i].localBounds;
            skinnedMeshRendererLod.sharedMesh = newRenderers[i].sharedMesh;
            skinnedMeshRendererLod.materials = newRenderers[i].sharedMaterials;
        }
        
    }
    private void ChangeAvatar(Avatar newAvatar)
    {
       
        animator.avatar = newAvatar;
        animator.Rebind();
     
    }
}
