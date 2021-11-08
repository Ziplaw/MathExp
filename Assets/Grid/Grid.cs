using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private float size;
    private Vector3 origin;
    public int[,,] grid;

    public Grid(float size, Vector3 origin, Vector3Int dimensions)
    {
        this.size = size;
        this.origin = origin;
        
        grid = new int[dimensions.x,dimensions.y,dimensions.z];

        
    }

    public Vector3 GetPosition(int i, int j, int k)
    {
        return origin + new Vector3(i,j,k) * size;
    }

    public Vector3 GetNearestFixedPosition(Vector3 point)
    {
        Vector3 nearestPos = Vector3.positiveInfinity;
        
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                for (int k = 0; k < grid.GetLength(2); k++)
                {
                    if ((GetPosition(i, j, k) - point).sqrMagnitude < (nearestPos - point).sqrMagnitude)
                        nearestPos = GetPosition(i, j, k);
                }
            }
        }

        return nearestPos;
    }
    
    public static implicit operator bool(Grid obj) 
    {
        return obj != null; 
    }
}
