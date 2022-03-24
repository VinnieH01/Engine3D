using Engine3D.Math;
using Engine3D.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine3D
{
	public struct Vertex
	{
		public Vector3 Position { get; private set; }
		public Vector2 UV { get; private set; }

		public Vertex(Vector3 position, Vector2 uv)
		{
			Position = position;
			UV = uv;
		}

		public Vertex(Vector3 position, float u, float v)
			: this(position, new(u, v)) { }

		public Vertex(float x, float y, float z, float u, float v)
			: this(new(x, y, z), new(u, v)) { }

		public Vertex Rotated(Vector3 rotation)
		{
			return new(Position.Rotated(rotation), UV);
		}

		public Vertex RotatedX(float rotation)
		{
			return new(Position.RotatedX(rotation), UV);
		}

		public Vertex Translated(Vector3 translation)
		{
			return new(Position + translation, UV);
		}
	}
}
