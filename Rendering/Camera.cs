using Engine3D.Math;
using System;

namespace Engine3D.Rendering
{
    public class Camera
    {
        public Vector3 Rotation { get; set; }
        public Vector3 Position { get; set; }
        public float Near { get; private set; }
        public float Far { get; private set; }
        private readonly float f;

        public Camera(float fov, float near, float far)
        {
            f = 1.0f / MathF.Tan((fov * MathF.PI / 180f) / 2.0f);
            Near = near;
            Far = far;
        }

        public Vertex[] CalculateViewVertices(Vertex[] vertices)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = vertices[i].Translated(-Position).Rotated(-Rotation);
            }

            return vertices;
        }

        public ScreenPoint Project(Vertex vertex, ScreenBuffer drawBuffer)
        {
            float aspectRatio = (float)drawBuffer.Width / drawBuffer.Height;

            return new ScreenPoint(
                (int)(((aspectRatio * vertex.Position.X * f / vertex.Position.Z) + 1.0f) * 0.5f * drawBuffer.Width),
                (int)(((vertex.Position.Y * f / vertex.Position.Z) + 1.0f) * 0.5f * drawBuffer.Height),
                vertex.Position.Z, vertex.UV);
        }

        public void Update(float deltaTime)
        {
            Rotation += new Vector3(0, Input.MouseDelta.X, 0);

            if (Input.GetKey("w"))
            {
                Position += new Vector3(MathF.Sin(Rotation.Y * MathF.PI / 180f), 0, MathF.Cos(Rotation.Y * MathF.PI / 180f)) * 2 * deltaTime;
            }
            if (Input.GetKey("s"))
            {
                Position -= new Vector3(MathF.Sin(Rotation.Y * MathF.PI / 180f), 0, MathF.Cos(Rotation.Y * MathF.PI / 180f)) * 2 * deltaTime;
            }
        }
    }
}
