using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public Transform root;
    public GameObject prefab;
    public Vector3 rotation;

    public int recursivity;
    // public int branchesGenerated = 0;

    List<GameObject> branches = new List<GameObject>();

    public void Generate()
    {
        // branchesGenerated = 0;
        DeleteBranches();
        GenerateBranch(root);
    }

    [ContextMenu("Generate")]
    public void GenerateBranch(Transform branch)
    {
        var node = branch.GetComponent<Node>();
        
        
        if (node.depth <= recursivity)
        {
            var go1 = Instantiate(prefab, branch.position + branch.up, Quaternion.identity);
            go1.transform.localRotation = branch.rotation * Quaternion.Euler(rotation);
            go1.transform.SetParent(branch);
            
            var go2 = Instantiate(prefab, branch.position + branch.up, Quaternion.identity);
            go2.transform.localRotation = branch.rotation * Quaternion.Euler(-rotation);
            go2.transform.SetParent(branch);
            
            branches.Add(go1);
            branches.Add(go2);
            go1.GetComponent<Node>().depth = node.depth + 1;
            go2.GetComponent<Node>().depth = node.depth + 1;
            
            GenerateBranch(go1.transform);
            GenerateBranch(go2.transform);
        }
    }

    public void DeleteBranches()
    {
        branches.ForEach(x => DestroyImmediate(x));
        branches.Clear();
    }
}