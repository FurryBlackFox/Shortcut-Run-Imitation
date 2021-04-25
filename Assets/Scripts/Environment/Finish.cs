using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;


public class Finish : MonoBehaviour
{
    public static event Action<Character> OnCharacterFinish;
    public static event Action OnPlayerFinish;
    public static event Action OnEveryoneFinished;
    
    
    [SerializeField] private CinemachineVirtualCamera finishCamera = default;
    [SerializeField] private List<Transform> finishPositions = default;

    public static Vector3 Position { get; private set; }



    private List<Character> finishersList = default;
    

    private int charactersCount;
    private void Start()
    {
        #if UNITY_EDITOR
        CustomTools.IsNull(finishCamera, nameof(finishCamera), name);
        #endif
        
        
        if (finishPositions.Count == 0)
            Debug.LogError($"Finish positions in {name} are not assigned !");

        finishersList = new List<Character>();
        Position = transform.position;
        charactersCount = GameDataKeeper.S.characters.Count;
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void RegisterCharacter(Character character)
    {
        if (finishersList.Contains(character))
            return;
        finishersList.Add(character);
        
        if (character.TryGetComponent<Player>(out var player))
            OnPlayerFinish?.Invoke();
        
        OnCharacterFinish?.Invoke(character);
        
        if(finishersList.Count == charactersCount)
            OnEveryoneFinished?.Invoke();
    }

    public Vector3 GetCameraPosition()
    {
        return finishCamera.transform.position;
    }

    public int GetFinishingResult(Character character)
    {
        return finishersList.IndexOf(character) + 1;
    }

    public Vector3 GetFinishPosition(int position)
    {
        return finishPositions[position - 1].position;
    }
    
    
    
    
}
