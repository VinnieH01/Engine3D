using Engine3D.Math;
using System;

namespace Engine3D.Rendering
{
	public class Camera
	{
		//public Vector3 Rotation { get; set; }
		public Vector3 Position { get; set; }
		public float Near { get; private set; }
		public float Far { get; private set; }

		private Vector3 Forward;

		private readonly float f;

		private float xRot, yRot;

		public Camera(float fov, float near, float far)
		{
			f = 1.0f / MathF.Tan((fov * MathF.PI / 180f) / 2.0f);
			Near = near;
			Far = far;
			Forward = new Vector3(0, 0, 1);
		}

		public Vertex[] CalculateViewVertices(Vertex[] vertices)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = vertices[i].Translated(-Position).Rotated(-new Vector3(0, yRot, 0)).RotatedX(-xRot);
			}

			return vertices;
		}

		public ScreenPoint Project(Vertex vertex, ScreenBuffer drawBuffer)
		{
			float aspectRatio = (float)drawBuffer.Width / drawBuffer.Height;

			return new(
				(int)(((aspectRatio * vertex.Position.X * f / vertex.Position.Z) + 1.0f) * 0.5f * drawBuffer.Width),
				(int)(((vertex.Position.Y * f / vertex.Position.Z) + 1.0f) * 0.5f * drawBuffer.Height),
				vertex.Position.Z, vertex.UV);
		}

		public void Update(float deltaTime)
		{
			yRot += Input.MouseDelta.X;
			xRot -= Input.MouseDelta.Y;

			if (Input.GetKey("w"))
			{
				Position += Forward.RotatedX(xRot).Rotated(new Vector3(0, yRot, 0)) * 2 * deltaTime;
			}
			if (Input.GetKey("s"))
			{
				Position -= Forward.RotatedX(xRot).Rotated(new Vector3(0, yRot, 0)) * 2 * deltaTime;
			}
			if (Input.GetKey("a"))
			{
				Position += Forward.Rotated(new Vector3(0, -90, 0)).RotatedX(xRot).Rotated(new Vector3(0, yRot, 0)) * 2 * deltaTime;
			}
			if (Input.GetKey("d"))
			{
				Position -= Forward.Rotated(new Vector3(0, -90, 0)).RotatedX(xRot).Rotated(new Vector3(0, yRot, 0)) * 2 * deltaTime;
			}
			if (Input.GetKey("q"))
			{
				Position += new Vector3(0, 1, 0).Rotated(new Vector3(0, -90, 0)).RotatedX(xRot).Rotated(new Vector3(0, yRot, 0)) * 2 * deltaTime;
			}
			if (Input.GetKey("e"))
			{
				Position -= new Vector3(0, 1, 0).Rotated(new Vector3(0, -90, 0)).RotatedX(xRot).Rotated(new Vector3(0, yRot, 0)) * 2 * deltaTime;
			}
		}
	}
}
