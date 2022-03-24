using Engine3D.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine3D.Math
{
	public static class MathUtils
	{
		public static float Lerp(float from, float to, float t)
		{
			return from + t * (to - from);
		}

		public static Vector3 Lerp(Vector3 from, Vector3 to, float t)
		{
			return new Vector3(Lerp(from.X, to.X, t), Lerp(from.Y, to.Y, t), Lerp(from.Z, to.Z, t));
		}

		public static Vector2 Lerp(Vector2 from, Vector2 to, float t)
		{
			return new Vector2(Lerp(from.X, to.X, t), Lerp(from.Y, to.Y, t));
		}

		/// <summary>
		/// Lerps all values of the <c>ScreenPoint</c>. This includes position, uv, uv/z, and 1/z
		/// </summary>
		/// <param name="from">The start point</param>
		/// <param name="to">The end point</param>
		/// <param name="t">The interpolation value</param>
		/// <returns></returns>
		public static ScreenPoint Lerp(ScreenPoint from, ScreenPoint to, float t)
		{
			return new ScreenPoint((int)Lerp(from.X, to.X, t), (int)Lerp(from.Y, to.Y, t), Lerp(from.Z, to.Z, t),
				Lerp(from.UV, to.UV, t), Lerp(from.UVoverZ, to.UVoverZ, t), Lerp(from.ZInv, to.ZInv, t));
		}

		public static Vertex Lerp(Vertex from, Vertex to, float t)
		{
			return new Vertex(Lerp(from.Position, to.Position, t), Lerp(from.UV, to.UV, t));
		}

		public static float Map(float value, float inStart, float inEnd, float outStart, float outEnd)
		{
			return outStart + (outEnd - outStart) / (inEnd - inStart) * (value - inStart);
		}
	}
}
