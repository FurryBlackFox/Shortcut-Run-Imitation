using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bot Data", menuName = "Scriptable Objects/Bot Data", order = 5)]
public class BotData : ScriptableObject
{
    public List<string> nicknames = default;

    public List<Sprite> countries = default;
    
    public List<Color> plankColors = default;
}
