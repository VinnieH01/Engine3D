using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine3D
{
    class Engine
    {
        public void Draw(ScreenBuffer buffer)
        {
            buffer.SetPixel(20, 20, new Color(255, 255, 0));
        }
    }
}
