using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
/// <summary>
/// test feature
/// </summary>
public class BakeTextureWindow : EditorWindow {

    private Material ImageMaterial;
    public Material[] ImageMaterials;
    private string FilePath = "Assets/MaterialImage.png";
    private string FilePath2 = "Assets/Textures";
    private Vector2Int Resolution;

    bool hasMaterial;
    bool hasResolution;
    bool hasFilePath;

    private bool hasMaterials;
    private bool hasFilePath2;


    [MenuItem ("Tools/Bake material to texture (TEST FEATURE)")]
    static void OpenWindow() {
        //create window
        BakeTextureWindow window = EditorWindow.GetWindow<BakeTextureWindow>();
        window.Show();
        window.CheckInput();
    }
    
    

    void OnGUI(){
        EditorGUILayout.HelpBox("Set the material you want to bake as well as the size "+
                "and location of the texture you want to bake to, then press the \"Bake\" button.", MessageType.None);

        using(var check = new EditorGUI.ChangeCheckScope())
        {
            ImageMaterial = (Material)EditorGUILayout.ObjectField("Material", ImageMaterial, typeof(Material), false);
            
            SerializedObject so = new SerializedObject(this);
            EditorGUILayout.PropertyField(so.FindProperty("ImageMaterials"), true);
          
            so.ApplyModifiedProperties(); // Remember to apply modified properties
            Resolution = EditorGUILayout.Vector2IntField("Image Resolution", Resolution);
            //FilePath = FileField2(FilePath);
            FilePath2 = FileField(FilePath2);



            if(check.changed){
                CheckInput();
            }
        }

        //GUI.enabled = hasMaterial && hasResolution && hasFilePath;

        GUI.enabled = hasMaterials && hasResolution && hasFilePath2;
        
        if(GUILayout.Button("Bake")){
            //BakeTexture();
            foreach (var material in ImageMaterials)
            {
                var path = GeneratePath(FilePath2, material.name);
                BakeTexture2(material, Resolution, path);
            }
        }
        GUI.enabled = true;

        //tell the user what inputs are missing
        if(!hasMaterial){
            EditorGUILayout.HelpBox("You're still missing a material to bake.", MessageType.Warning);
        }
        if(!hasResolution){
            EditorGUILayout.HelpBox("Please set a size bigger than zero.", MessageType.Warning);
        }
        if(!hasFilePath){
            EditorGUILayout.HelpBox("No file to save the image to given.", MessageType.Warning);
        }
    }

    void CheckInput(){
        //check which values are entered already
        hasMaterial = ImageMaterial != null;
        hasMaterials = ImageMaterials.Length > 0;
        hasResolution = Resolution.x > 0 && Resolution.y > 0;
        // hasFilePath = false;
        // try{
        //     string ext = Path.GetExtension(FilePath);
        //     hasFilePath = ext.Equals(".png");
        // } catch(ArgumentException){}

        hasFilePath2 = true;
    }

    string FileField(string path){
        //allow the user to enter output file both as text or via file browser
        EditorGUILayout.LabelField("Image Path");
        using(new GUILayout.HorizontalScope()){
            path = EditorGUILayout.TextField(path);
            if(GUILayout.Button("choose")){
                //set default values for directory, then try to override them with values of existing path
                string directory = "Assets";
                string fileName = "MaterialImage.png";
                try{
                    directory = Path.GetDirectoryName(path);
                    fileName = Path.GetFileName(path);
                } catch(ArgumentException){}
                string chosenFile = EditorUtility.SaveFilePanelInProject("Choose image file", fileName, 
                        "png", "Please enter a file name to save the image to", directory);
                if(!string.IsNullOrEmpty(chosenFile)){
                    path = chosenFile;
                }
                //repaint editor because the file changed and we can't set it in the textfield retroactively
                Repaint();
            }
        }
        return path;
    }
    
    string FileField2(string path){
        EditorGUILayout.LabelField("Image Path 2");
        using(new GUILayout.HorizontalScope()){
            path = EditorGUILayout.TextField(path);
            // if(GUILayout.Button("choose")){
            //     //set default values for directory, then try to override them with values of existing path
            //     string directory = "Assets";
            //     try{
            //         directory = Path.GetDirectoryName(path);
            //     } catch(ArgumentException){}
            //     // string chosenFile = EditorUtility.SaveFilePanelInProject("Choose image file", fileName, 
            //     //     "png", "Please enter a file name to save the image to", directory);
            //     // if(!string.IsNullOrEmpty(chosenFile)){
            //     //     path = chosenFile;
            //     // }
            //     path = directory;
            //     //repaint editor because the file changed and we can't set it in the textfield retroactively
            //     Repaint();
            // }
        }
        return path;
    }

    void BakeTexture(){
        //render material to rendertexture
        RenderTexture renderTexture = RenderTexture.GetTemporary(Resolution.x, Resolution.y);
        Graphics.Blit(null, renderTexture, ImageMaterial);

        //transfer image from rendertexture to texture
        Texture2D texture = new Texture2D(Resolution.x, Resolution.y);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(Vector2.zero, Resolution), 0, 0);

        //save texture to file
        byte[] png = texture.EncodeToPNG();
        File.WriteAllBytes(FilePath, png);
        AssetDatabase.Refresh();

        //clean up variables
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);
        DestroyImmediate(texture);
    }
    
    void BakeTexture2(Material material, Vector2Int resolution, string directory){
        //render material to rendertexture
        RenderTexture renderTexture = RenderTexture.GetTemporary(resolution.x, resolution.y);
        Graphics.Blit(null, renderTexture, material);

        //transfer image from rendertexture to texture
        Texture2D texture = new Texture2D(resolution.x, resolution.y);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(Vector2.zero, resolution), 0, 0);

        //save texture to file
        byte[] png = texture.EncodeToPNG();
        File.WriteAllBytes(directory, png);
        AssetDatabase.Refresh();

        //clean up variables
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);
        DestroyImmediate(texture);
    }


    private string GeneratePath(string directory, string materialName)
    {
        return Path.Combine(directory, materialName + ".png");
    }
}
