using Engine3D.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine3D
{
    public struct Vertex
    {
        public Vector3 Position { get; private set; }
        public Vector2 UV { get; private set; }

        public Vertex(Vector3 position, Vector2 uv)
        {
            Position = position;
            UV = uv;
        }

        public Vertex(Vector3 position, float u, float v)
        {
            Position = position;
            UV = new Vector2(u, v);
        }

        public Vertex(float x, float y, float z, float u, float v)
        {
            Position = new Vector3(x, y, z);
            UV = new Vector2(u, v);
        }

        public Vertex Rotated(Vector3 rotation)
        {
            Vector3 radians = rotation * (MathF.PI / 180.0f);
            return new Vertex(
                    new Vector3(Position.X * (MathF.Cos(radians.Z) * MathF.Cos(radians.Y)) +
                            Position.Y * (MathF.Cos(radians.Z) * MathF.Sin(radians.Y) * MathF.Sin(radians.X) - MathF.Sin(radians.Z) * MathF.Cos(radians.X)) +
                            Position.Z * (MathF.Cos(radians.Z) * MathF.Sin(radians.Y) * MathF.Cos(radians.X) + MathF.Sin(radians.Z) * MathF.Sin(radians.X)),
                    Position.X * (MathF.Sin(radians.Z) * MathF.Cos(radians.Y)) +
                            Position.Y * (MathF.Sin(radians.Z) * MathF.Sin(radians.Y) * MathF.Sin(radians.X) + MathF.Cos(radians.Z) * MathF.Cos(radians.X)) +
                            Position.Z * (MathF.Sin(radians.Z) * MathF.Sin(radians.Y) * MathF.Cos(radians.X) - MathF.Cos(radians.Z) * MathF.Sin(radians.X)),
                    Position.X * (-MathF.Sin(radians.Y)) +
                            Position.Y * (MathF.Cos(radians.Y) * MathF.Sin(radians.X)) +
                            Position.Z * (MathF.Cos(radians.Y) * MathF.Cos(radians.X))), UV
            );
        }

        public Vertex Translated(Vector3 translation)
        {
            return new Vertex(Position + translation, UV);
        }
    }
}
