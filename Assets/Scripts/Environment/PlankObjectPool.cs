using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankObjectPool : MonoBehaviour
{
    [SerializeField] private Plank plankPrefab;
    [SerializeField] private int stashSize = 150;
    [SerializeField] private string rootName = "PlankObjPool";

    private Stack<Plank> planksInStash;
    private List<Plank> planksInUse;

    private Transform root;

    private void Awake()
    {
#if UNITY_EDITOR
        
        CustomTools.IsNull(plankPrefab, nameof(plankPrefab), name);     
        
#endif
        
        planksInStash = new Stack<Plank>();
        planksInUse = new List<Plank>();
        root = new GameObject {name = rootName}.transform;
    }

    private void Start()
    {
        for (var i = 0; i < stashSize; i++)
        {
            CreatePlank();
        }
    }

    private void CreatePlank()
    {
        var newPlank = Instantiate(plankPrefab, root, false);
        newPlank.gameObject.SetActive(false);
        planksInStash.Push(newPlank);
    }

    public Plank GetPlank()
    {
        if(planksInStash.Count == 0)
            CreatePlank();
        var plank = planksInStash.Pop();
        plank.gameObject.SetActive(true);
        planksInUse.Add(plank);
        return plank;
    }

    public void ReturnPlank(Plank plankToRemove)
    {
        var plankIndex = planksInUse.IndexOf(plankToRemove);
        if (plankIndex == -1)
        {
            Debug.LogError("There is no such plank in inUse array");
            return;
        }
        var plank = planksInUse[plankIndex];
        var lastIndex = planksInUse.Count - 1;
        planksInUse[plankIndex] = planksInUse[lastIndex];
        planksInUse.RemoveAt(lastIndex);
       
        
        plank.transform.parent = root;
        plank.gameObject.SetActive(false);
        planksInStash.Push(plank);
    }
}
