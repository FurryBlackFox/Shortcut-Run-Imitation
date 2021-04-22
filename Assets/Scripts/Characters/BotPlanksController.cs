using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotPlanksController : CharacterPlanksController
{
    internal enum PlanksMaterial
    {
        Opaque,
        Transparent
    }

    
    
    internal void ApplyNewMaterial(PlanksMaterial planksMaterial)
    {
        currentPlanksMaterial =
            planksMaterial == PlanksMaterial.Opaque ? planksMaterialOpaque : planksMaterialTransparent;

        foreach (var plank in planks)
        {
            plank.Material = currentPlanksMaterial;
        }
    }

}
