using Engine3D.Math;
using System;

namespace Engine3D.Rendering
{
    public class Camera
    {
        private readonly float width;
        private readonly float fov;
        public Vector3 Rotation { get; set; }
        public Vector3 Position { get; set; }

        public Camera(float width, float fov)
        {
            this.width = width;
            this.fov = fov;
        }

        public ScreenPoint Project(Vertex vertex)
        {
            float eyeDistance = (width / 2.0f) / MathF.Tan(fov / 2.0f);
            return new ScreenPoint(
                (int)(vertex.Position.X * eyeDistance / vertex.Position.Z),
                (int)(vertex.Position.Y * eyeDistance / vertex.Position.Z),
                vertex.Position.Z, vertex.UV);
        }
    }
}
