using System;
using System.Collections.Generic;
using System.Text;

namespace Engine3D.Math
{
	public struct Vector2
	{
		public float X { get; set; }
		public float Y { get; set; }
		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}

		public static Vector2 operator *(Vector2 vector, float factor)
			=> new(vector.X * factor, vector.Y * factor);

		public static Vector2 operator /(Vector2 vector, float denom)
			=> new(vector.X / denom, vector.Y / denom);

		public static Vector2 operator *(Vector2 vector, int factor)
		   => new(vector.X * factor, vector.Y * factor);

		public static Vector2 operator *(int factor, Vector2 vector)
		   => vector * factor;

		public static Vector2 operator /(Vector2 a, Vector2 b)
		   => new(a.X / b.X, a.Y / b.Y);

		public static Vector2 operator -(Vector2 a, Vector2 b)
			=> new(a.X - b.X, a.Y - b.Y);

		public static Vector2 operator +(Vector2 a, Vector2 b)
			=> new(a.X + b.X, a.Y + b.Y);
	}
}
