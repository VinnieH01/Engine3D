﻿using Engine3D.Math;
using System;

namespace Engine3D.Rendering
{
    public class Camera
    {
        private readonly float width;
        private readonly float fov;
        public Vector3 Direction {get; private set;}

        public Camera(float width, float fov)
        {
            this.width = width;
            this.fov = fov;
            Direction = new Vector3(0, 0, 1);
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