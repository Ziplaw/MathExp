using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

public class ConvertToCell : MonoBehaviour
{
    public Texture2D texture;
    public int pixelSize = 1;
    // public Texture2D outTexture;

    [ContextMenu("cellify")]
    void Cellify()
    {
        
        Texture2D newTex = new Texture2D(texture.width/pixelSize,texture.height/pixelSize, TextureFormat.RGB24, false);
        for (int y = 0; y < texture.height/pixelSize; y++)
        {
            for (int x = 0; x < texture.width/pixelSize; x++)
            {
                var r = texture.GetPixel(x * pixelSize, y* pixelSize).r;
                var g = texture.GetPixel(x* pixelSize, y* pixelSize).g;
                var b = texture.GetPixel(x* pixelSize, y* pixelSize).b;


                Color color = r + g + b > 1.5f ? Color.white : Color.black;
                newTex.SetPixel(x,y,color);
            }
        }
        
        newTex.Apply();
        var bytes = newTex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Tests/" + texture.name +"_cellified.png",bytes);
        // outTexture = newTex;
        // AssetDatabase.CreateAsset(newTex,@"Assets/Tests/output.png");
    }
    [ContextMenu("invert")]

    void Invert()
    {
        
        Texture2D newTex = new Texture2D(texture.width/pixelSize,texture.height/pixelSize, TextureFormat.RGB24, false);
        for (int y = 0; y < texture.height/pixelSize; y++)
        {
            for (int x = 0; x < texture.width/pixelSize; x++)
            {
                var r = texture.GetPixel(x * pixelSize, y* pixelSize).r;
                var g = texture.GetPixel(x* pixelSize, y* pixelSize).g;
                var b = texture.GetPixel(x* pixelSize, y* pixelSize).b;


                Color color = r + g + b < 1.5f ? Color.white : Color.black;
                newTex.SetPixel(x,y,color);
            }
        }
        
        newTex.Apply();
        newTex.filterMode = FilterMode.Point;
        var bytes = newTex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Tests/" + texture.name +"_inverted.png",bytes);
        // outTexture = newTex;
        // AssetDatabase.CreateAsset(newTex,@"Assets/Tests/output.png");
    }
}
