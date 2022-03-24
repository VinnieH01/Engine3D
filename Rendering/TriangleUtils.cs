using Engine3D.Math;
using System;
using System.Collections.Generic;

namespace Engine3D.Rendering
{
	class TriangleUtils
	{
		public static Vector3 CalculateNormal(Vertex[] triangle)
		{
			Vector3 u = triangle[1].Position - triangle[0].Position;
			Vector3 v = triangle[2].Position - triangle[0].Position;

			return u.Cross(v).Normalized();
		}

		public static Vertex[][] GetClippedZTriangles(Vertex[] triangle, float near, float far)
		{
			Queue<Vertex[]> triangleQueue = new();
			triangleQueue.Enqueue(triangle);

			int numTriangles;
			List<Vertex> insidePoints = new();
			List<Vertex> outsidePoints = new();

			//Clip triangles with the near plane
			numTriangles = triangleQueue.Count;
			for (int i = 0; i < numTriangles; i++)
			{
				ExtractTrianglePoints(triangleQueue, insidePoints, outsidePoints, point => point.Position.Z < near);
				ClipZ(triangleQueue, insidePoints, outsidePoints, near);
			}

			//Clip triangles with the far plane
			numTriangles = triangleQueue.Count;
			for (int i = 0; i < numTriangles; i++)
			{
				ExtractTrianglePoints(triangleQueue, insidePoints, outsidePoints, point => point.Position.Z > far);
				ClipZ(triangleQueue, insidePoints, outsidePoints, far);
			}

			return triangleQueue.ToArray();

			//Uses the clipping function to clip triangles in the Z axis
			void ClipZ(Queue<Vertex[]> toReturn, List<Vertex> insidePoints, List<Vertex> outsidePoints, float z)
			{
				Clip(toReturn, insidePoints, outsidePoints, (o, i) => MathF.Abs(z - o[0].Position.Z) / MathF.Abs(i[0].Position.Z - o[0].Position.Z),
					(o, i) => MathF.Abs(z - outsidePoints[0].Position.Z) / MathF.Abs(insidePoints[1].Position.Z - outsidePoints[0].Position.Z),
					(o, i) => MathF.Abs(z - outsidePoints[1].Position.Z) / MathF.Abs(insidePoints[0].Position.Z - outsidePoints[1].Position.Z),
					MathUtils.Lerp);
			}
		}

		/// <summary>
		/// Dequeues the triangle queue and populates the inside and outside lists with the trinagles points.
		/// </summary>
		/// <typeparam name="T">The type of point (ScreenPoint or Vertex)</typeparam>
		/// <param name="triangleQueue">The queue of triangles</param>
		/// <param name="insidePoints">The list of inside points</param>
		/// <param name="outsidePoints">The list of outside points</param>
		/// <param name="outside">A function to determine if a point is outside the frustum</param>
		private static void ExtractTrianglePoints<T>(Queue<T[]> triangleQueue, List<T> insidePoints, List<T> outsidePoints, Func<T, bool> outside)
		{
			T[] currentTriangle = triangleQueue.Dequeue();

			insidePoints.Clear();
			outsidePoints.Clear();
			for (int j = 0; j < 3; j++)
			{
				if (outside(currentTriangle[j]))
					outsidePoints.Add(currentTriangle[j]);
				else
					insidePoints.Add(currentTriangle[j]);
			}
		}

		/// <summary>
		/// Clips triangles and constructs new ones based on the points in insidePoints and outsidePoints.
		/// </summary>
		/// <typeparam name="T">The type of point (ScreenPoint or Vertex) </typeparam>
		/// <param name="triangleQueue">The queue of triangles that will be returned once all clipping is finished</param>
		/// <param name="insidePoints">The points on inside the frustum</param>
		/// <param name="outsidePoints">The points on outside the frustum</param>
		/// <param name="firstToFirstT">The proportion of distance outside the frustum to the total distance between the first outside to the first inside point</param>
		/// <param name="firstToSecondT">The proportion of distance outside the frustum to the total distance between the first outside to the second inside point</param>
		/// <param name="secondToFirstT">The proportion of distance outside the frustum to the total distance between the second outside to the first inside point</param>
		/// <param name="interpolation">An interpolation function to interpolate between points</param>
		private static void Clip<T>(Queue<T[]> triangleQueue, List<T> insidePoints, List<T> outsidePoints,
				Func<List<T>, List<T>, float> firstToFirstT,
				Func<List<T>, List<T>, float> firstToSecondT,
				Func<List<T>, List<T>, float> secondToFirstT,
				Func<T, T, float, T> interpolation)
		{
			if (outsidePoints.Count == 0)
			{
				//If no points are outside we reconstruct the triangle
				triangleQueue.Enqueue(new T[] { insidePoints[0], insidePoints[1], insidePoints[2] });
			}
			else if (outsidePoints.Count == 1)
			{
				//Get the progress from the outside point to the first inside point
				float t = firstToFirstT(outsidePoints, insidePoints);

				//Interpolate between them so we get the new point that lies at the border
				T extraPoint1 = interpolation(outsidePoints[0], insidePoints[0], t);

				//Do the same for the outside point to the second inside point
				t = firstToSecondT(outsidePoints, insidePoints);
				T extraPoint2 = interpolation(outsidePoints[0], insidePoints[1], t);

				//Construct the two resulting triangles
				triangleQueue.Enqueue(new T[] { extraPoint1, insidePoints[0], insidePoints[1] });
				triangleQueue.Enqueue(new T[] { extraPoint2, extraPoint1, insidePoints[1] });
			}
			else if (outsidePoints.Count == 2)
			{
				//Interpolate from the first outside point to the inside point
				float t = firstToFirstT(outsidePoints, insidePoints);
				T extraPoint1 = interpolation(outsidePoints[0], insidePoints[0], t);

				//Interpolate from the second outside point to the inside point
				t = secondToFirstT(outsidePoints, insidePoints);
				T extraPoint2 = interpolation(outsidePoints[1], insidePoints[0], t);

				//Construct the triangle
				triangleQueue.Enqueue(new T[] { extraPoint1, extraPoint2, insidePoints[0] });
			}
		}

		public static ScreenPoint[][] GetClippedScreenTriangles(ScreenPoint[] triangle, int bufferWidth, int bufferHeight)
		{
			Queue<ScreenPoint[]> triangleQueue = new();
			triangleQueue.Enqueue(triangle);

			int numTriangles;
			List<ScreenPoint> insidePoints = new();
			List<ScreenPoint> outsidePoints = new();

			//Clip triangles with the left side of the screen
			numTriangles = triangleQueue.Count;
			for (int i = 0; i < numTriangles; i++)
			{
				ExtractTrianglePoints(triangleQueue, insidePoints, outsidePoints, point => point.X < 0);
				ClipX(triangleQueue, insidePoints, outsidePoints, 0);
			}

			//Clip triangles with the right side of the screen
			numTriangles = triangleQueue.Count;
			for (int i = 0; i < numTriangles; i++)
			{
				ExtractTrianglePoints(triangleQueue, insidePoints, outsidePoints, point => point.X >= bufferWidth);
				ClipX(triangleQueue, insidePoints, outsidePoints, bufferWidth - 1);
			}

			//Clip triangles with the top of the screen
			numTriangles = triangleQueue.Count;
			for (int i = 0; i < numTriangles; i++)
			{
				ExtractTrianglePoints(triangleQueue, insidePoints, outsidePoints, point => point.Y < 0);
				ClipY(triangleQueue, insidePoints, outsidePoints, 0);
			}

			//Clip triangles with the bottom of the screen
			numTriangles = triangleQueue.Count;
			for (int i = 0; i < numTriangles; i++)
			{
				ExtractTrianglePoints(triangleQueue, insidePoints, outsidePoints, point => point.Y >= bufferWidth);
				ClipY(triangleQueue, insidePoints, outsidePoints, bufferHeight - 1);
			}

			return triangleQueue.ToArray();

			//Uses the Clip method to clip triangles with the X axis
			void ClipX(Queue<ScreenPoint[]> toReturn, List<ScreenPoint> insidePoints, List<ScreenPoint> outsidePoints, int x)
			{
				Clip(toReturn, insidePoints, outsidePoints, (o, i) => MathF.Abs(x - o[0].X) / MathF.Abs(i[0].X - o[0].X),
					(o, i) => MathF.Abs(x - outsidePoints[0].X) / MathF.Abs(insidePoints[1].X - outsidePoints[0].X),
					(o, i) => MathF.Abs(x - outsidePoints[1].X) / MathF.Abs(insidePoints[0].X - outsidePoints[1].X),
					MathUtils.Lerp);
			}

			//Uses the Clip method to clip triangles with the Y axis
			void ClipY(Queue<ScreenPoint[]> toReturn, List<ScreenPoint> insidePoints, List<ScreenPoint> outsidePoints, int y)
			{
				Clip(toReturn, insidePoints, outsidePoints, (o, i) => MathF.Abs(y - o[0].Y) / MathF.Abs(i[0].Y - o[0].Y),
					(o, i) => MathF.Abs(y - outsidePoints[0].Y) / MathF.Abs(insidePoints[1].Y - outsidePoints[0].Y),
					(o, i) => MathF.Abs(y - outsidePoints[1].Y) / MathF.Abs(insidePoints[0].Y - outsidePoints[1].Y),
					MathUtils.Lerp);
			}
		}

		public static ScreenPoint[] ProjectTriangleTo2D(Vertex[] vertices, Camera camera, ScreenBuffer buffer)
		{
			if (vertices.Length != 3) throw new ArgumentException("A triangle has exactly 3 verticies.");

			ScreenPoint[] screenPoints = new ScreenPoint[3];

			for (int i = 0; i < 3; i++)
			{
				screenPoints[i] = camera.Project(vertices[i], buffer);
			}

			return screenPoints;
		}
	}
}
