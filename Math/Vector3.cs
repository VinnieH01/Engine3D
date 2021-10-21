using System;
using System.Collections.Generic;
using System.Text;

namespace Engine3D.Math
{
    struct Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 operator *(Vector3 vector, float factor)
            => new Vector3(vector.X * factor, vector.Y * factor, vector.Z * factor);

        public static Vector3 operator +(Vector3 a, Vector3 b)
            => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }
}
