using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDisposer : MonoBehaviour
{
    [SerializeField] private bool autoDispose = false;
    [SerializeField] private bool randomDispose = false;


    private List<Character> characters;
    private StartPlatform startPlatform;
    private void Awake()
    {
        characters = GameDataKeeper.S.characters;
        startPlatform = FindObjectOfType<StartPlatform>();
        if(!startPlatform)
            Debug.LogWarning("There is no Start Platforms in scene");
    }

    private void Start()
    {
        if(!autoDispose)
            return;
        
        if(!startPlatform)
            return;
        
        if (!randomDispose)
        {
            foreach (var character in characters)
            {
                var point = startPlatform.GetStartPoint();
                character.SetPosition(point.position);
                character.transform.rotation = point.rotation;
            }
        }
        else
        {
            foreach (var character in characters)
            {
                var point = startPlatform.GetRandomStartPoint();
                character.SetPosition(point.position);
                character.transform.rotation = point.rotation;
            }
        }
    }
}
