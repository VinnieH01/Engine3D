using System;
using System.Collections.Generic;
using System.Text;

namespace Engine3D.Rendering
{
    public class DepthBuffer
    {
        private readonly float[] bufferData;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public DepthBuffer(int width, int height)
        {
            Width = width;
            Height = height;
            bufferData = new float[width * height];
        }

        public void Clear()
        {
            for (int i = 0; i < bufferData.Length; ++i)
            {
                bufferData[i] = float.PositiveInfinity;
            }
        }

        public void SetDepth(int x, int y, float depth)
        {
            bufferData[Width * y + x] = depth;
        }

        public float GetDepth(int x, int y)
        {
            return bufferData[Width * y + x];
        }
    }
}

