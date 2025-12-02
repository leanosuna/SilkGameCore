using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilkGameCore.Collisions
{
    public class CollisionTriangle
    {
        public Vector3[] v;
        public uint id;
        public CollisionTriangle()
        {
            v = new Vector3[3];
        }
        public CollisionTriangle(uint id, Vector3[] v)
        {
            this.id = id;
            this.v = v;
        }
        public Vector3 GetNormal()
        {
            Vector3 edge1 = v[1] - v[0];
            Vector3 edge2 = v[2] - v[0];
            return Vector3.Normalize(Vector3.Cross(edge1, edge2));
        }

        public void GetPlane(out Vector3 normal, out float D)
        {
            normal = GetNormal();
            D = -Vector3.Dot(normal, v[0]);
        }
    }
}
