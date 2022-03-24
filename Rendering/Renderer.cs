using Engine3D.Math;
using System;
using System.Collections.Generic;
using Engine3D.Rendering.Lighting;

namespace Engine3D.Rendering
{
	class Renderer
	{
		/// <summary>
		/// Draws a mesh to the supplied screen buffer.
		/// </summary>
		/// <param name="mesh">The mesh to draw</param>
		/// <param name="drawBuffer">The screen buffer to draw to</param>
		/// <param name="depthBuffer">The depth buffer to write to</param>
		/// <param name="camera">The camera to use for projecting the triangles to the screen</param>
		public static void Draw(Mesh mesh, List<Light> lights, ScreenBuffer drawBuffer, DepthBuffer depthBuffer, Camera camera)
		{
			List<(Vertex[], Vector3)> viewTriangles = new();

			foreach(Triangle triangle in mesh.Triangles)
			{
				Vertex[] globalVertices = CalculateGlobalTriangleVertices(triangle, mesh.Position, mesh.Rotation);

				Vector3 normal = TriangleUtils.CalculateNormal(globalVertices);
				Vector3 position = (globalVertices[0].Position + globalVertices[1].Position + globalVertices[2].Position) / 3.0f;

				Vertex[] viewVertices = camera.CalculateViewVertices(globalVertices);

				//Normal in view space (not the true normal of the object so cannot be used for lighting and such, only for culling)
				Vector3 viewNormal = TriangleUtils.CalculateNormal(viewVertices);

				//Ray from camera (0, 0, 0) in world space to the normal
				Vector3 cameraDirection = viewVertices[0].Position - new Vector3(0, 0, 0);

				//If the triangle is facing away from the camera we dont care about it
				if (viewNormal.Dot(cameraDirection) >= 0) continue;
				Vector3 light = CalculateTotalLight(lights, normal, position);

				viewTriangles.Add((viewVertices, light));
			}

			foreach ((Vertex[] vertices, Vector3 light) in viewTriangles)
			{
				foreach(Vertex[] triangle3D in TriangleUtils.GetClippedZTriangles(vertices, camera.Near, camera.Far))
				{
					ScreenPoint[] screenPoints = TriangleUtils.ProjectTriangleTo2D(triangle3D, camera, drawBuffer);

					foreach (ScreenPoint[] triangle in TriangleUtils.GetClippedScreenTriangles(screenPoints, drawBuffer.Width, drawBuffer.Height))
					{
						DrawTriangle(triangle, mesh.Texture, light, drawBuffer, depthBuffer);
					}
				}
			}    
		}

		private static Vector3 CalculateTotalLight(List<Light> lights, Vector3 normal, Vector3 position)
		{
			Vector3 total = new();
			foreach (Light light in lights)
			{
				total += light.CalculateLightAmount(position, normal);
			}

			return total;
		}

		public static Vertex[] CalculateGlobalTriangleVertices(Triangle triangle, Vector3 position, Vector3 rotation)
		{
			Vertex[] vertices = new Vertex[triangle.Vertices.Length];
			for (int i = 0; i < triangle.Vertices.Length; i++)
			{
				vertices[i] = triangle.Vertices[i].Rotated(rotation).Translated(position);
			}

			return vertices;
		}

		private static void DrawTriangle(ScreenPoint[] screenPoints, Texture texture, Vector3 light, ScreenBuffer buffer, DepthBuffer depthBuffer)
		{
			if (screenPoints.Length != 3) throw new ArgumentException("A triangle has exactly 3 points.");

			//Sort the screenPoints so that the one higest on the screen is first in the array
			Array.Sort(screenPoints, (x, y) => { return x.Y > y.Y ? 1 : y.Y > x.Y ? -1 : 0; });

			ScreenPoint high = screenPoints[0];
			ScreenPoint mid = screenPoints[1];
			ScreenPoint low = screenPoints[2];

			//Draws half the triangle from either the high to mid point, or mid to low point.
			void DrawHalfTriangle(ScreenPoint start, ScreenPoint end)
			{
				//Go through all vertical values from start to end point (end.Y-1 because the last line of pixels belongs to the next triangle (a triangle next to this one))
				for (int y = start.Y; y <= end.Y-1; y++)
				{
					//For every vertical value we calculate the total progress of both slopes of the half triangle
					float startToEndProgress = (float)(y - start.Y) / (end.Y - start.Y);
					float highToLowProgress = (float)(y - high.Y) / (low.Y - high.Y);

					//These progresses are then used to interpolate between the screen points
					ScreenPoint startToEnd = MathUtils.Lerp(start, end, startToEndProgress);
					ScreenPoint highToLow = MathUtils.Lerp(high, low, highToLowProgress);

					//The point on the left is the start of the scanline and the one on the right is the end
					ScreenPoint from = startToEnd.X < highToLow.X ? startToEnd : highToLow;
					ScreenPoint to = startToEnd == from ? highToLow : startToEnd;

					DrawScanline(buffer, depthBuffer, from, to, y, texture, light);
				}
			}

			DrawHalfTriangle(high, mid);
			DrawHalfTriangle(mid, low);
		}

		private static void DrawScanline(ScreenBuffer buffer, DepthBuffer depthBuffer, ScreenPoint from, ScreenPoint to, int y, Texture texture, Vector3 light)
		{
			//Go through the entire line except the last pixel because it belongs to the next triangle (a triangle next to this one)
			for (int x = from.X; x <= to.X-1; x++)
			{
				float scanlineProgress = (float)(x - from.X) / (to.X - from.X);

				float depth = MathUtils.Lerp(from.Z, to.Z, scanlineProgress);

				if (depthBuffer.GetDepth(x, y) > depth)
				{
					//A UV divided by the z coord of the projected vertex is used for perspective correct texture mapping.
					//This is why 1/z (zInv) is later used to correct back to normal UV space.
					//see https://en.wikipedia.org/wiki/Texture_mapping#Perspective_correctness for more info.
					Vector2 uv_z = MathUtils.Lerp(from.UVoverZ, to.UVoverZ, scanlineProgress);
					float zInv = MathUtils.Lerp(from.ZInv, to.ZInv, scanlineProgress);

					uint color = LightUtils.ApplyLight(texture.GetPixel(new Vector2(uv_z.X / zInv, uv_z.Y / zInv)), light);
					buffer.SetPixel(x, y, color);
					depthBuffer.SetDepth(x, y, depth);
				}
			}
		}
	}
}
