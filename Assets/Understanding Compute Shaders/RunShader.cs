using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunShader : MonoBehaviour
{
    public ComputeShader shader;
    public bool useGPU;

    private void Update() {
        if(useGPU) RunGPU(); else RunCPU();
    }
    struct VecMatPair
    {
        public Vector3 point;
        public Vector3 addition;
    }

    [ContextMenu("runGPU")]
    void RunGPU()
    {
        DateTime before = DateTime.Now;

        VecMatPair[] data = new VecMatPair[transform.childCount];
        VecMatPair[] output = new VecMatPair[transform.childCount];

        for (int i = 0; i < data.Length; i++)
        {
            data[i].point = transform.position;
            data[i].addition = new Vector3(0, Mathf.Sin (i + Time.time), i);
        }

        // print(data[0].point);

        ComputeBuffer buffer = new ComputeBuffer(data.Length, 24); // 3 floats + 16 floats * 4 bytes per float
        buffer.SetData(data);

        int kernel = shader.FindKernel("Multiply");
        shader.SetBuffer(kernel, "dataBuffer", buffer);
        shader.Dispatch(kernel, data.Length, 1, 1);
        buffer.GetData(output);
        // print(output[0].point);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).position = output[i].point;
        }
        buffer.Dispose();



        DateTime now = DateTime.Now;
        TimeSpan duration = now.Subtract(before);
        print(duration.Milliseconds);
    }

    [ContextMenu("runCPU")]
    void RunCPU()
    {
        DateTime before = DateTime.Now;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).position = new Vector3(Mathf.Cos(i + Time.time), Mathf.Sin(i + Time.time), i);
        }

        DateTime now = DateTime.Now;
        TimeSpan duration = now.Subtract(before);
        print(duration.Milliseconds);
    }

    [ContextMenu("reset")]
    void Reset()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).position = new Vector3(0, 0, 0);
        }
    }
}
