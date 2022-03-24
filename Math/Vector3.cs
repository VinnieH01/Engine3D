using System;
using System.Collections.Generic;
using System.Text;

namespace Engine3D.Math
{
	public struct Vector3
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public float Magnitude()
			=> MathF.Sqrt(X * X + Y * Y + Z * Z);
		
		public static Vector3 operator *(Vector3 vector, float factor)
			=> new(vector.X * factor, vector.Y * factor, vector.Z * factor);

		public static Vector3 operator /(Vector3 vector, float factor)
			=> new(vector.X / factor, vector.Y / factor, vector.Z / factor);

		public static Vector3 operator +(Vector3 a, Vector3 b)
			=> new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

		public static Vector3 operator -(Vector3 a, Vector3 b)
			=> new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

		public static Vector3 operator -(Vector3 a)
			=> new(-a.X, -a.Y, -a.Z);

		public Vector3 Cross(Vector3 other)
			=> new(Y * other.Z - Z * other.Y, Z * other.X - X * other.Z, X * other.Y - Y * other.X);

		public float Dot(Vector3 other)
			=> X * other.X + Y * other.Y + Z * other.Z;

		public Vector3 Normalized() => this / Magnitude();

		public Vector3 Rotated(Vector3 rotation)
		{
			Vector3 radians = rotation * (MathF.PI / 180.0f);
			return new(X * (MathF.Cos(radians.Z) * MathF.Cos(radians.Y)) +
							Y * (MathF.Cos(radians.Z) * MathF.Sin(radians.Y) * MathF.Sin(radians.X) - MathF.Sin(radians.Z) * MathF.Cos(radians.X)) +
							Z * (MathF.Cos(radians.Z) * MathF.Sin(radians.Y) * MathF.Cos(radians.X) + MathF.Sin(radians.Z) * MathF.Sin(radians.X)),
					X * (MathF.Sin(radians.Z) * MathF.Cos(radians.Y)) +
							Y * (MathF.Sin(radians.Z) * MathF.Sin(radians.Y) * MathF.Sin(radians.X) + MathF.Cos(radians.Z) * MathF.Cos(radians.X)) +
							Z * (MathF.Sin(radians.Z) * MathF.Sin(radians.Y) * MathF.Cos(radians.X) - MathF.Cos(radians.Z) * MathF.Sin(radians.X)),
					X * (-MathF.Sin(radians.Y)) +
							Y * (MathF.Cos(radians.Y) * MathF.Sin(radians.X)) +
							Z * (MathF.Cos(radians.Y) * MathF.Cos(radians.X)));
		}

		public Vector3 RotatedX(float rotation)
		{
			float radians = rotation * (MathF.PI / 180.0f);
			return new(X, Y * MathF.Cos(radians) - Z * MathF.Sin(radians), Y * MathF.Sin(radians) + Z * MathF.Cos(radians));
		}

		public Vector3 Translated(Vector3 translation)
		{
			return this + translation;
		}
	}
}
