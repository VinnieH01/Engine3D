using Engine3D.Math;
using System.Runtime.InteropServices;

namespace Engine3D.Rendering.Lighting
{
	public static class LightUtils
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct Color
		{
			[FieldOffset(0)]
			public byte r;
			[FieldOffset(1)]
			public byte g;
			[FieldOffset(2)]
			public byte b;
			[FieldOffset(2)]
			public byte a;

			[FieldOffset(0)]
			public uint color;

			public Color(uint color)
			{
				r = 0;
				g = 0;
				b = 0;
				a = 0;
				this.color = color;
			}
		}

		public static uint ApplyLight(uint color, Vector3 light)
		{
			Color currentColor = new(color);

			currentColor.r = (byte)System.Math.Clamp(currentColor.r * light.X, 0, 255);
			currentColor.g = (byte)System.Math.Clamp(currentColor.g * light.Y, 0, 255);
			currentColor.b = (byte)System.Math.Clamp(currentColor.b * light.Z, 0, 255);

			return currentColor.color;
		}
	}
}
