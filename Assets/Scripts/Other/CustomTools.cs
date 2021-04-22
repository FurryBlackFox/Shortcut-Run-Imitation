using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public static class CustomTools
{
    #region Debug
    
    public static bool IsNull<T>(T obj, string parameterName, string invokerName = "")
    {
#if UNITY_EDITOR
        if (obj != null)
            return true;
        Debug.LogError($"{parameterName} is nog assigned in {invokerName}");
#endif
        return false;
    }


    public static void DrawThickLine(Vector3 start, Vector3 end, Color color, float thickness)
    {
#if UNITY_EDITOR
        Handles.DrawBezier(start, end, start, end, color, 
            null, thickness);
#endif
    }

    public static void DrawString(string text, Vector3 pos, int? fontSize = null,  Color? textColor = null, Color? backgroundColor = null)
    {
#if UNITY_EDITOR
        var view = SceneView.currentDrawingSceneView;
        if (view == null || view.camera == null) 
            return;
        
        var screenPos = view.camera.WorldToScreenPoint(pos);
        if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width ||
            screenPos.z < 0)
            return;
            
        Handles.BeginGUI();
            
        var style = new GUIStyle(EditorStyles.numberField);
        //style.normal.background = Texture2D.whiteTexture;
        style.fontSize = fontSize ?? 12;
        style.alignment = TextAnchor.MiddleCenter;

        var prevTextColor = GUI.color;
        var prevBackColor = GUI.backgroundColor;
            
        GUI.contentColor = textColor ?? Color.white;
        GUI.backgroundColor = backgroundColor ?? Color.black;
        GUI.depth = Int32.MaxValue;

        var size = style.CalcSize(new GUIContent(text));
        var rect = new Rect(screenPos.x - (size.x * 0.5f), -screenPos.y + view.position.height + 4,
            size.x + 2, size.y);
        GUI.Box(rect, text, style);
        GUI.color = prevTextColor;
        GUI.backgroundColor = prevBackColor;
            
        Handles.EndGUI();
#endif
    }



    #endregion


    #region GUI
    
    public static void EnableCanvasGroup(this CanvasGroup canvasGroup, bool value)
    {
        canvasGroup.alpha = value ? 1f : 0f;
        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
    }


    #endregion


    #region Random

        
    
    public static float SampleGaussian01()
    {
        return SampleGaussian(0.5f, 0.16f);
    }
    
    public static float SampleGaussian(float mean, float stdDev) //TODO: mb too expensive to runtime execution  
    {
        // The method requires sampling from a uniform random of (0,1]
        // but Random.NextDouble() returns a sample of [0,1).
        
        var x1 = Random.Range(0.000000000000001f, 1); //almost exclusive 0
        var x2 = Random.Range(0.000000000000001f, 1); //almost exclusive 0

        var y1 = Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Cos(2.0f * Mathf.PI * x2);
        return y1 * stdDev + mean;
    }

    #endregion
}
