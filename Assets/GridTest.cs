using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTest : MonoBehaviour
{
    
    public enum ShowGizmo
    {
        All,
        Point,
        None
    }


    private Grid grid;
    public float size = 1;
    public Vector3 origin;
    public Vector3Int dimensions;
    public ShowGizmo showGizmo;
    private Camera camera;

    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    void OnValidate()
    {
        grid = new Grid(size, origin, dimensions);
    }

    private void OnDrawGizmos()
    {
        if (grid && showGizmo != ShowGizmo.None)
            for (int i = 0; i < grid.grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.grid.GetLength(1); j++)
                {
                    for (int k = 0; k < grid.grid.GetLength(2); k++)
                    {
                        Gizmos.color = new Color(0.49f, 1f, 0.87f, .1f);
                        Gizmos.DrawWireSphere(grid.GetPosition(i, j, k), .01f);
                        Gizmos.color = new Color(0.74f, 1f, 0.93f, 0.29f);
                        if (showGizmo == ShowGizmo.All)
                        {
                            Gizmos.DrawLine(grid.GetPosition(i, j, k) - Vector3.one * size * .5f,
                                grid.GetPosition(i + 1, j, k) - Vector3.one * size * .5f);
                            Gizmos.DrawLine(grid.GetPosition(i, j, k) - Vector3.one * size * .5f,
                                grid.GetPosition(i, j + 1, k) - Vector3.one * size * .5f);
                            Gizmos.DrawLine(grid.GetPosition(i, j, k) - Vector3.one * size * .5f,
                                grid.GetPosition(i, j, k + 1) - Vector3.one * size * .5f);
                        }
                    }
                }
            }



        if (camera)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        
            if (Physics.Raycast(ray, out hit)) {
            
            
                Gizmos.DrawWireCube(grid.GetNearestFixedPosition(hit.point),Vector3.one);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}