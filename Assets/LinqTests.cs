using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LinqTests : MonoBehaviour
{
    public Vector2[] array;

    
    [ContextMenu("do the thing")]
    void Randomize()
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = Random.insideUnitCircle;
            array[i].Normalize();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Array.Sort(array, new ClockwiseVector2Comparer());
        var arrayCopy = array.ToList();
        arrayCopy.Add(transform.position);
        array = arrayCopy.ToArray();
    }

    // Update is called once per frame
    void Update()
    {

        var meshFilter = GetComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        
        List<int> indices = new List<int>();


        for (int i = 0; i < array.Length-1; i++)
        {
            indices.Add(i);
            indices.Add(array.Length - 1);
            indices.Add(i + 1);
        }

        mesh.vertices =  (from v in array select (Vector3)v).ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < array.Length; i++)
        {
            Gizmos.DrawWireSphere(array[i],.1f);
        }
    }
}

public class ClockwiseVector2Comparer : IComparer<Vector2>
{
    public int Compare(Vector2 v1, Vector2 v2)
    {
        if (v1.x >= 0)
        {
            if (v2.x < 0)
            {
                return -1;
            }
            return -Comparer<float>.Default.Compare(v1.y, v2.y);
        }
        else
        {
            if (v2.x >= 0)
            {
                return 1;
            }
            return Comparer<float>.Default.Compare(v1.y, v2.y);
        }
    }
}
