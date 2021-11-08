using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PNG_to_Spritesheet.Editor
{
    public class PNGtoSSEditorWindow : EditorWindow
    {
        [MenuItem("Tools/PNG to Spritesheet")]
        private static void ShowWindow()
        {
            var window = GetWindow<PNGtoSSEditorWindow>();
            window.titleContent = new GUIContent("PNG to Spritesheet");
            window.Show();
        }

        private SerializedObject serializedObject;

        private void OnEnable()
        {
            serializedObject = new SerializedObject(GetWindow<PNGtoSSEditorWindow>());
        }

        public List<Texture2D> list = new List<Texture2D>();

        private void OnGUI()
        {
            // list = EditorGUILayout.ObjectField(list, typeof(List<Texture2D>),false);
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("list"));
            serializedObject.ApplyModifiedProperties();
            int t = list.Count;
            

            if (t > 0)
            {
                if (list.Any(t => t.width != list[0].width || t.height != list[0].height))
                {
                    GUILayout.Label(
                        $"Hey! These textures are different sizes, they should all be the same resolution ({list[0].width}x{list[0].height})");
                    return;
                }

                int y = Mathf.FloorToInt(Mathf.Sqrt(t));
                int x = Mathf.CeilToInt(t / (float) y);
                GUILayout.Label("SpriteSheet Layout: " + x + "x" + y + $", {list[0].width}px by {list[0].height}px");
                
                


                if (GUILayout.Button("Generate"))
                {
                    Texture2D texture2D = new Texture2D(x * list[0].width, y * list[0].height,TextureFormat.RGBA32, false);
                    // RenderTexture renderTexture = RenderTexture.GetTemporary(texture2D.width, texture2D.height,0,RenderTextureFormat.Default,RenderTextureReadWrite.Linear);

                    Texture2D[,] array = new Texture2D[x,y]; 
                    for (var j = 0; j < array.GetLength(1); j++)
                    {
                        for (int i = 0; i < array.GetLength(0); i++)
                        {
                            array[i, j] = i + j*array.GetLength(0) >= list.Count ? null :
                                list[i + j*array.GetLength(0)];
                        }
                    }
                    
                    for (int j = 0; j < texture2D.height; j++)
                    {
                        for (int i = 0; i < texture2D.width; i++)
                        {
                            var textureLookUp = array[i / list[0].width, y-1-j / list[0].height];
                            
                            //Just in case:
                            //16 => 1; x = 4; y = 4;
                            //15 => 3; x = 5; y = 3;
                            //14 => 3; x = 5; y = 3;
                            //12 => 2; x = 4; y = 3
                            //11 => 2; x = 4; y = 3
                            //10 => 2; x = 4, y = 3
                            //9 => 1; x = 3, y = 3
                            //8 => 3; x = 4, y = 2
                            //7 => 3; x = 4, y = 2
                            //6 => 2; x = 3, y = 2 //ceil x / y ?
                            //5 => 2; x = 3, y = 2
                            //4 => 1; x = 2, y = 2
                            //3 => 3; x = 3, y = 1
                            //2 => 2; x = 2, y = 1
                            //1 => 1; x = 1, y = 1
                            
                            texture2D.SetPixel(i, j,
                            textureLookUp != null
                                ? textureLookUp.GetPixel(i % list[0].width, j % list[0].height)
                                : new Color(0, 0, 0, 0));
                        }
                    }
                    
                    texture2D.Apply();

                    var path = EditorUtility.SaveFilePanel("Save Sprite Sheet in:", "Assets", "out", "png");
                    if (path.StartsWith(Application.dataPath))
                    {
                        path = "Assets" + path.Substring(Application.dataPath.Length);
                    }
                    else if (string.IsNullOrEmpty(path))
                    {
                        return;
                    }

                    Debug.Log("Asset being saved at: " + path);
                    // AssetDatabase.CreateAsset(readableText, path);
                    var bytes = texture2D.EncodeToPNG();
                    File.WriteAllBytes(path,bytes);
                    AssetDatabase.Refresh();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}