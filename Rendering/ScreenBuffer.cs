using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine3D.Rendering
{
    public class ScreenBuffer
    {
        private readonly Color[] bufferData;
        private readonly Texture2D buffer;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public ScreenBuffer(int width, int height, GraphicsDevice device)
        {
            Width = width;
            Height = height;
            buffer = new Texture2D(device, width, height);
            bufferData = new Color[width * height];
        }

        public void Clear()
        {
            Array.Clear(bufferData, 0, bufferData.Length);
        }

        public void SetPixel(int x, int y, uint color)
        {
            bufferData[Width * y + x] = new Color(color);
        }

        public void Draw(SpriteBatch batch, int width, int height)
        {
            buffer.SetData(bufferData);
            batch.Draw(buffer, new Rectangle(0, 0, width, height), Color.White);
        }


    }
}
