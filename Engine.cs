using System;
using System.Collections.Generic;
using System.Text;
using Engine3D.Rendering;
using Engine3D.Math;

namespace Engine3D
{
    class Engine
    {
        Vertex v;
        Projector p;
        float angle;

        public Engine()
        {
            v = new Vertex(-50, 0, 0, 0, 0);
            p = new Projector(500, 45);
        }

        public void Draw(ScreenBuffer buffer, float deltaTime)
        {
            angle += 80 * deltaTime;
            ScreenPoint sp = p.Project(v.Rotated(new Vector3(angle*0.5f, angle, angle*0.25f)).Translated(new Vector3(0, 0, 400)));
            sp.X += buffer.Width/2;
            sp.Y += buffer.Height/2;

            buffer.SetPixel(sp.X, sp.Y, 0x0000FF);
        }
    }
}
