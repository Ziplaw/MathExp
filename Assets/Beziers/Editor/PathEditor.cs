using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    Path manager;

    void OnEnable()
    {
        manager = target as Path;
        SceneView.duringSceneGui += DuringSceneView;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneView;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void DuringSceneView(SceneView sv)
    {
        for (var i = 0; i < manager.points.Length; i++)
        {
            manager.points[i].up = Handles.Disc(Quaternion.identity, manager.points[i].position,
                manager.points[i].localSpaceExitControl, manager.width, false, 15) * manager.points[i].up;

            if (i < manager.points.Length - 1)
            {
                Handles.DrawBezier(manager.points[i].position + manager.points[i].displacementVector * manager.width,
                    manager.points[i + 1].position +
                    manager.points[i + 1].displacementVector * manager.width, /*manager.points[i].position +*/
                    manager.points[i].worldSpaceExitControl + manager.points[i].displacementVector * manager.width,
                    manager.points[i + 1].worldSpaceEntryControl +
                    manager.points[i + 1].displacementVector * manager.width, Color.cyan,
                    Texture2D.whiteTexture, 3);
                Handles.DrawBezier(
                    manager.points[i].position - manager.points[i].displacementVector * manager.width,
                    manager.points[i + 1].position - manager.points[i + 1].displacementVector * manager.width, /*manager.points[i].position +*/
                    manager.points[i].worldSpaceExitControl - manager.points[i].displacementVector * manager.width,
                    manager.points[i + 1].worldSpaceEntryControl -
                    manager.points[i + 1].displacementVector * manager.width, Color.cyan,
                    Texture2D.whiteTexture, 3);
            }

            // Handles.DrawLine(manager.points[i].position, manager.points[i].position + manager.points[i].up);
            // Handles.DrawLine(manager.points[i].position, manager.points[i].position + manager.points[i].displacementVector);
        }


        // Handles.DrawLine(Vector3.zero,GetDisplacementVector(manager.points[0],manager.points[1]));
        // GetDisplacementVector(manager.points[0], manager.points[1]);
    }

    // Vector3 GetDisplacementVector(BezierPoint p0, BezierPoint p1)
    // {
    //     var direction = (p0.worldSpaceExitControl - p0.position).normalized;
    //     //Dot Product, solve for dot = 0
    //     var z2 = (-direction.x - direction.y) / direction.z;
    //     var normal = new Vector3(1, 1, z2).normalized;
    //
    //     Handles.DrawLine(Vector3.zero, direction);
    //     Handles.DrawLine(Vector3.zero, normal);
    //     return Vector3.zero;
    // }
}