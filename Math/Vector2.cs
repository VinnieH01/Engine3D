using System;
using System.Collections.Generic;
using System.Text;

namespace Engine3D.Math
{
    struct Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 operator *(Vector2 vector, float factor)
            => new Vector2(vector.X * factor, vector.Y * factor);

        public static Vector2 operator /(Vector2 vector, float denom)
            => new Vector2(vector.X / denom, vector.Y / denom);
    }
}
