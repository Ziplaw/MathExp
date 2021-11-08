using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ziplaw;

public class ZQuaternionRotator : MonoBehaviour
{
    [Serializable]
    public struct QuaternionData
    {
        public Vector3 axis;
        public float degrees;

        public QuaternionData(Vector3 axis, float degrees)
        {
            this.axis = axis;
            this.degrees = degrees;
        }
    }

    public List<QuaternionData> rots;

    private void OnValidate()
    {
        transform.rotation = UnityEngine.Quaternion.identity;
        transform.rotation = new ZQuaternion(rots[0].axis, rots[0].degrees * Mathf.Deg2Rad);
        
        for (var i = 1; i < rots.Count; i++)
        {
            if (i != rots.Count)
            {
                transform.rotation = transform.rotation.AsZQuaternion() * new ZQuaternion(rots[i].axis, rots[i].degrees * Mathf.Deg2Rad);
            }
        }
    }
}