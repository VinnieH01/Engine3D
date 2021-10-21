using Engine3D.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine3D.Rendering
{
    class ScreenPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public float Z { get; set; }
        public Vector2 UV { get; set; }

        public ScreenPoint(int x, int y, float z, Vector2 uv)
        {
            X = x;
            Y = y;
            Z = z;
            UV = uv;
        }

        public ScreenPoint(int x, int y, float z, float u, float v)
        {
            X = x;
            Y = y;
            Z = z;
            UV = new Vector2(u, v);
        }

        public ScreenPoint(ScreenPoint other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
            UV = other.UV;
        }
    }
}
