using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Level Sequence", menuName = "Scriptable Objects/Level Sequence", order = 9)]
public class LevelSequence : ScriptableObject
{
    [SerializeField] private List<string> levelsList = default;
    public List<string> LevelsList => levelsList;

    [SerializeField] private string shopLevel;
    public string ShopLevel => shopLevel;
}
