using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSkinsRepresentator : MonoBehaviour
{
    public static ShopSkinsRepresentator S;
    
    [SerializeField]
    private CharacterSkin characterSkin;

    [SerializeField]
    private ParticleSystem characterEmergeEffect;


    private void Awake()
    {
        if (S == null)
            S = this;
    }
    

    public void RepresentNewSkin(CharacterSkin newCharacterSkin)
    {
        var currentSkinTransform = characterSkin.transform;
        var newSkinTransform = Instantiate(newCharacterSkin, currentSkinTransform.parent).transform;

        newSkinTransform.localPosition = currentSkinTransform.localPosition;
        newSkinTransform.localRotation = currentSkinTransform.localRotation;
        newSkinTransform.localScale = currentSkinTransform.localScale;
        newSkinTransform.SetSiblingIndex(currentSkinTransform.GetSiblingIndex());
        
        Destroy(characterSkin.gameObject);
        characterSkin = newSkinTransform.GetComponent<CharacterSkin>();
        
        characterEmergeEffect.Play();

    }
}
