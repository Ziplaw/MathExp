using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ziplaw
{
    public struct ZQuaternion
    {
        private Vector3 vectorPart;
        private float scalarPart;

        public ZQuaternion inverse => new ZQuaternion(-vectorPart, (double)scalarPart);

        public ZQuaternion(Vector3 axis, float theta)
        {
            scalarPart = Mathf.Cos(theta*.5f);
            vectorPart = axis * Mathf.Sin(theta * .5f);
        }

        internal ZQuaternion(Vector3 vectorPart, double scalarPart)
        {
            this.vectorPart = vectorPart;
            this.scalarPart = (float)scalarPart;
        }

        public static implicit operator Quaternion(ZQuaternion q)
        {
            return new Quaternion(q.vectorPart.x,q.vectorPart.y,q.vectorPart.z,q.scalarPart);
        }

        public static ZQuaternion operator *(ZQuaternion p, ZQuaternion q)
        {
            // Grassman Product // Only for normalized vector parts
            var vectorPart = p.scalarPart * q.vectorPart + q.scalarPart * p.vectorPart + Vector3.Cross(p.vectorPart, q.vectorPart);
            double scalarPart = p.scalarPart * q.scalarPart - Vector3.Dot(p.vectorPart, q.vectorPart);
            
            // return new ZQuaternion(axis, theta);
            return new ZQuaternion(vectorPart, scalarPart);
            
        }

        public static ZQuaternion operator *(ZQuaternion q, Vector3 v)
        {
            // v' = p * v * p^-1 Only for rotating vectors
            var vq = new ZQuaternion(v,0d);

            Vector3 vectorPart = Vector3.Cross(q.vectorPart, vq.vectorPart);
            vectorPart = Vector3.Cross(vectorPart, q.inverse.vectorPart);
            
            float scalarPart = q.scalarPart * vq.scalarPart * q.inverse.scalarPart;
            
            return new ZQuaternion(vectorPart, scalarPart);
        }
    }

    public static class Extensions
    {
        public static ZQuaternion AsZQuaternion(this Quaternion q)
        {
            return new ZQuaternion(new Vector3(q.x,q.y,q.z),(double)q.w);
        }
    }
}
