using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CharacterSkin : MonoBehaviour
{
    [SerializeField]
    private List<SkinnedMeshRenderer> meshLods;
    [SerializeField]
    private Avatar avatar;
    [SerializeField] 
    private Transform leaderCrown;

    public List<SkinnedMeshRenderer> MeshLods => meshLods;
    
    public Avatar Avatar => avatar;
    
    public Transform LeaderCrown => leaderCrown;
}
