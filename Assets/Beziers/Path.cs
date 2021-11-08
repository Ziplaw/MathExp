using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public BezierPoint[] points = new BezierPoint[5];
    [SerializeField] private Vector3[] curve;
    public int steps = 5;
    public bool loop;
    public float width;

    private void OnValidate()
    {
        // curve = new Vector3[steps * (points.Length-1)];
        // //curveBuild step * points[i]
        //
        // for (int i = 0; i < points.Length-1; i++)
        // {
        //     for (int j = 0; j < steps; j++)
        //     {
        //         var t = j / (float) steps;
        //         var currentStep = i * steps + j;
        //         curve[currentStep] = PointInCurve(t, points[i].position, points[i].worldSpaceExitControl,
        //             points[i + 1].worldSpaceEntryControl, points[i + 1].position);
        //     }
        // }
    }

    private void OnDrawGizmos()
    {
        // return;

        foreach (var bezierPoint in points)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(bezierPoint.position, 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(bezierPoint.worldSpaceExitControl, 0.05f);
            Gizmos.DrawWireSphere(bezierPoint.worldSpaceEntryControl, 0.05f);

            var prev = points[0].position;

            for (int j = 0; j < points.Length - 1; j++)
            {
                for (int i = 0; i <= steps; i++)
                {
                    var t = i / (float) steps;
                    var position =
                        PointInCurve(t,
                            points[j].position,
                            points[j].worldSpaceExitControl,
                            points[j + 1].worldSpaceEntryControl,
                            points[j + 1].position);
                    // var tangentPos = TangentInCurve(t,
                    //     points[j].position,
                    //     points[j].worldSpaceExitControl,
                    //     points[j + 1].worldSpaceEntryControl,
                    //     points[j + 1].position);

                    Gizmos.color = Color.cyan;
                    // Gizmos.DrawLine(prev, position);
                    // Gizmos.DrawLine(position, position + tangentPos);
                    prev = position;
                }
            }
        }
    }

    public Vector3 PointInCurve(float t, Vector3 p0, Vector3 control0, Vector3 control1, Vector3 p1)
    {
        return (1 - t) * (1 - t) * (1 - t) * p0 + 3 * (1 - t) * (1 - t) * t * control0 +
               3 * (1 - t) * t * t * control1 +
               t * t * t * p1;
    }

    public Vector3 TangentInCurve(float t, Vector3 p0, Vector3 control0, Vector3 control1, Vector3 p1)
    {
        // Vector3 a, b, c, d, e;
        // a = (1 - t) * p0 + t * control0;
        // b = (1 - t) * control0 + t * control1;
        // c = (1 - t) * control1 + t * p1;
        // d = (1 - t) * ((1 - t) * p0 + t * control0) + t * ((1 - t) * control0 + t * control1);
        // e = (1 - t) * ((1 - t) * control0 + t * control1) + t * ((1 - t) * control1 + t * p1);
        // // d = (1-t) * (1 - t) * p0 + t * control0 + t * (1 - t) * control0 + t * control1;
        // // e = (1-t) * (1 - t) * control0 + t * control1 + t * (1 - t) * control1 + t * p1;
        return (((1 - t) * ((1 - t) * control0 + t * control1) + t * ((1 - t) * control1 + t * p1)) -
                ((1 - t) * ((1 - t) * p0 + t * control0) + t * ((1 - t) * control0 + t * control1))).normalized;
    }

    public Vector3 GetPosition(float t) // [0,1]
    {
        if (0 <= t && t <= 1)
        {
            // t -> [0,1]
            // float currentPointIndex = Remap(t, 0, 1, 0, points.Length-1);
            float currentPointIndex = t * (points.Length - 1);
            BezierPoint point = points[Mathf.FloorToInt(currentPointIndex)];
            BezierPoint nextPoint = points[Mathf.FloorToInt(currentPointIndex + (t == 1 ? 0 : 1))];
            return PointInCurve(currentPointIndex % 1f, point.position, point.worldSpaceExitControl,
                nextPoint.worldSpaceEntryControl,
                nextPoint.position);
        }
        else
        {
            Debug.LogError("t is not a value between 0 and 1");
            return Vector3.zero;
        }
    }

    public Quaternion GetRotation(float t)
    {
        if (0 <= t && t <= 1)
        {
            float currentPointIndex = t * (points.Length - 1);
            BezierPoint point = points[Mathf.FloorToInt(currentPointIndex)];
            BezierPoint nextPoint = points[Mathf.FloorToInt(currentPointIndex + (t == 1 ? 0 : 1))];

            return Quaternion.LookRotation(TangentInCurve(currentPointIndex % 1f, point.position,
                    point.worldSpaceExitControl, nextPoint.worldSpaceEntryControl, nextPoint.position),
                Vector3.Lerp(point.up, nextPoint.up, currentPointIndex % 1f));
        }
        else
        {
            Debug.LogError("t is not a value between 0 and 1");
            return Quaternion.identity;
        }
    }

    float Remap(float value, float min, float max, float newMin, float newMax)
    {
        return newMin + (value - min) * (newMax - newMin) / (max - min);
    }
}