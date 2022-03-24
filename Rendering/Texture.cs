using System;
using System.Collections.Generic;
using System.Text;
using Engine3D.Math;

namespace Engine3D.Rendering
{
	/// <summary>
	/// Represents an array of colors in a 2D grid that can be used to texture meshes
	/// </summary>
	public class Texture
	{
		/// <summary>
		/// The width of the texture in pixels
		/// </summary>
		public int Width {get; private set;}

		/// <summary>
		/// The height of the texture in pixels
		/// </summary>
		public int Height { get; private set; }

		private readonly uint[] data;

		public Texture(uint[] colors, int width, int height)
		{
			data = colors;
			Width = width;
			Height = height;
		}

		/// <summary>
		/// Gets a pixel from the texture at the uv coord. The coordinate is clamped.
		/// </summary>
		/// <param name="uv">The coordiante where the pixel lies in uv space (0-1)</param>
		/// <returns>The color of the pixel at the supplied uv coord</returns>
		public uint GetPixel(Vector2 uv)
		{
			int x = System.Math.Clamp((int)(uv.X * Width), 0, Width - 1);
			int y = System.Math.Clamp((int)(uv.Y * Height), 0, Height - 1);

			return data[Width * y + x];
		}
	}
}
