using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Engine3D
{
    public static class TextureLoader
    {
        private static GraphicsDevice graphicsDevice;

        public static void Init(GraphicsDevice graphicsDevice)
        {
            TextureLoader.graphicsDevice = graphicsDevice;
        }

        public static Rendering.Texture LoadTexture(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            Texture2D texture2d = Texture2D.FromStream(graphicsDevice, fileStream);
            fileStream.Dispose();

            uint[] data = new uint[texture2d.Width * texture2d.Height];
            texture2d.GetData(data);

            Rendering.Texture texture = new Rendering.Texture(data, texture2d.Width, texture2d.Height);

            texture2d.Dispose();

            return texture;
        }
    }
}
