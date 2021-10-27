using Engine3D.Math;
using System;
using System.Collections.Generic;

namespace Engine3D.Rendering
{
    class Renderer
    {
        /// <summary>
        /// Draws a list of triangles to the supplied screen buffer.
        /// </summary>
        /// <param name="triangles">The triangles to draw</param>
        /// <param name="drawBuffer">The screen buffer to draw to</param>
        /// <param name="depthBuffer">The depth buffer to write to</param>
        /// <param name="camera">The camera to use for projecting the triangles to the screen</param>
        public void Draw(List<Triangle> triangles, ScreenBuffer drawBuffer, DepthBuffer depthBuffer, Camera camera)
        {
            List<(Vertex[], Texture)> viewTriangles = new List<(Vertex[], Texture)>();

            foreach(Triangle triangle in triangles)
            {
                (Vertex[] globalVertices, Texture texture) = triangle.CalculateGlobalVertices();

                Vertex[] viewVertices = camera.CalculateViewVertices(globalVertices);

                Vector3 u = viewVertices[1].Position - viewVertices[0].Position;
                Vector3 v = viewVertices[2].Position - viewVertices[0].Position;

                //Normal in view space (not the true normal of the object so cannot be used for lighting and such, only for culling)
                Vector3 viewNormal = u.Cross(v);

                //Ray from camera (0, 0, 0) in world space to the normal
                Vector3 cameraDirection = viewVertices[0].Position - new Vector3(0, 0, 0);

                //If the triangle is facing away from the camera we dont care about it
                if (viewNormal.Dot(cameraDirection) >= 0) continue;

                viewTriangles.Add((viewVertices, texture));
            }

            foreach ((Vertex[] vertices, Texture texture) in viewTriangles)
            {
				foreach(Vertex[] triangle3D in GetClippedZTriangles(vertices, camera.Near, camera.Far))
                {
					ScreenPoint[] screenPoints = ProjectTriangleTo2D(triangle3D, camera, drawBuffer);

					foreach (ScreenPoint[] triangle in GetClippedScreenTriangles(screenPoints, drawBuffer.Width, drawBuffer.Height))
					{
						DrawTriangle(triangle, texture, drawBuffer, depthBuffer);
					}
				}
			}    
        }

		private Vertex[][] GetClippedZTriangles(Vertex[] triangle, float near, float far)
		{
			Queue<Vertex[]> toReturn = new Queue<Vertex[]>();
			toReturn.Enqueue(triangle);

			int numTriangles;
			List<Vertex> insidePoints = new List<Vertex>();
			List<Vertex> outsidePoints = new List<Vertex>();

            //Clip triangles with the near plane
			numTriangles = toReturn.Count;
			for (int i = 0; i < numTriangles; i++)
			{
                ExtractTrianglePoints(toReturn, insidePoints, outsidePoints, point => point.Position.Z < near);
                ClipZ(toReturn, insidePoints, outsidePoints, near);
            }

            //Clip triangles with the far plane
            numTriangles = toReturn.Count;
            for (int i = 0; i < numTriangles; i++)
            {
                ExtractTrianglePoints(toReturn, insidePoints, outsidePoints, point => point.Position.Z > far);
                ClipZ(toReturn, insidePoints, outsidePoints, far);
            }

            return toReturn.ToArray();

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
        /// <param name="triangles">The queue of triangles</param>
        /// <param name="insidePoints">The list of inside points</param>
        /// <param name="outsidePoints">The list of outside points</param>
        /// <param name="outside">A function to determine if a point is outside the frustum</param>
        private void ExtractTrianglePoints<T>(Queue<T[]> triangles, List<T> insidePoints, List<T> outsidePoints, Func<T, bool> outside)
        {
            T[] currentTriangle = triangles.Dequeue();

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
        /// <param name="toReturn">The queue of triangles that will be returned once all clipping is finished</param>
        /// <param name="insidePoints">The points on inside the frustum</param>
        /// <param name="outsidePoints">The points on outside the frustum</param>
        /// <param name="firstToFirstT">The proportion of distance outside the frustum to the total distance between the first outside to the first inside point</param>
        /// <param name="firstToSecondT">The proportion of distance outside the frustum to the total distance between the first outside to the second inside point</param>
        /// <param name="secondToFirstT">The proportion of distance outside the frustum to the total distance between the second outside to the first inside point</param>
        /// <param name="interpolation">An interpolation function to interpolate between points</param>
        private void Clip<T>(Queue<T[]> toReturn, List<T> insidePoints, List<T> outsidePoints,
                Func<List<T>, List<T>, float> firstToFirstT,
                Func<List<T>, List<T>, float> firstToSecondT,
                Func<List<T>, List<T>, float> secondToFirstT,
                Func<T, T, float, T> interpolation)
        {
            if (outsidePoints.Count == 0)
            {
                //If no points are outside we reconstruct the triangle
                toReturn.Enqueue(new T[] { insidePoints[0], insidePoints[1], insidePoints[2] });
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
                toReturn.Enqueue(new T[] { extraPoint1, insidePoints[0], insidePoints[1] });
                toReturn.Enqueue(new T[] { extraPoint2, extraPoint1, insidePoints[1] });
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
                toReturn.Enqueue(new T[] { extraPoint1, extraPoint2, insidePoints[0] });
            }
        }

        private ScreenPoint[][] GetClippedScreenTriangles(ScreenPoint[] triangle, int bufferWidth, int bufferHeight)
		{
			Queue<ScreenPoint[]> toReturn = new Queue<ScreenPoint[]>();
			toReturn.Enqueue(triangle);

			int numTriangles;
			List<ScreenPoint> insidePoints = new List<ScreenPoint>();
			List<ScreenPoint> outsidePoints = new List<ScreenPoint>();

            //Clip triangles with the left side of the screen
            numTriangles = toReturn.Count;
			for (int i = 0; i < numTriangles; i++)
            {
                ExtractTrianglePoints(toReturn, insidePoints, outsidePoints, point => point.X < 0);
                ClipX(toReturn, insidePoints, outsidePoints, 0);
            }

            //Clip triangles with the right side of the screen
            numTriangles = toReturn.Count;
			for (int i = 0; i < numTriangles; i++)
			{
                ExtractTrianglePoints(toReturn, insidePoints, outsidePoints, point => point.X >= bufferWidth);
                ClipX(toReturn, insidePoints, outsidePoints, bufferWidth - 1);
			}

            //Clip triangles with the top of the screen
            numTriangles = toReturn.Count;
			for (int i = 0; i < numTriangles; i++)
            {
                ExtractTrianglePoints(toReturn, insidePoints, outsidePoints, point => point.Y < 0);
                ClipY(toReturn, insidePoints, outsidePoints, 0);
            }

            //Clip triangles with the bottom of the screen
            numTriangles = toReturn.Count;
			for (int i = 0; i < numTriangles; i++)
			{
                ExtractTrianglePoints(toReturn, insidePoints, outsidePoints, point => point.Y >= bufferWidth);
                ClipY(toReturn, insidePoints, outsidePoints, bufferHeight - 1);
			}

			return toReturn.ToArray();

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

        private ScreenPoint[] ProjectTriangleTo2D(Vertex[] vertices, Camera camera, ScreenBuffer buffer)
        {
            if (vertices.Length != 3) throw new ArgumentException("A triangle has exactly 3 verticies.");

            ScreenPoint[] screenPoints = new ScreenPoint[3];

            for (int i = 0; i < 3; i++)
            {
                screenPoints[i] = camera.Project(vertices[i], buffer);
            }

            return screenPoints;
        }

        private void DrawTriangle(ScreenPoint[] screenPoints, Texture texture, ScreenBuffer buffer, DepthBuffer depthBuffer)
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

                    DrawScanline(buffer, depthBuffer, from, to, y, texture);
                }
            }

            DrawHalfTriangle(high, mid);
            DrawHalfTriangle(mid, low);
        }

        private void DrawScanline(ScreenBuffer buffer, DepthBuffer depthBuffer, ScreenPoint from, ScreenPoint to, int y, Texture texture)
        {
            //Go through the entire line except the last pixel because it belongs to the next triangle (a triangle next to this one)
            for (int x = from.X; x <= to.X-1; x++)
            {
                float scanlineProgress = (float)(x - from.X) / (to.X - from.X);

                //A UV divided by the z coord of the projected vertex is used for perspective correct texture mapping.
                //This is why 1/z (zInv) is later used to correct back to normal UV space.
                //see https://en.wikipedia.org/wiki/Texture_mapping#Perspective_correctness for more info.
                Vector2 uv_z = MathUtils.Lerp(from.UVoverZ, to.UVoverZ, scanlineProgress);
                float zInv = MathUtils.Lerp(from.ZInv, to.ZInv, scanlineProgress);
                float depth = MathUtils.Lerp(from.Z, to.Z, scanlineProgress);

                if (depthBuffer.GetDepth(x, y) > depth)
                {
                    buffer.SetPixel(x, y, texture.GetPixel(new Vector2(uv_z.X / zInv, uv_z.Y / zInv)));
                    depthBuffer.SetDepth(x, y, depth);
                }
            }
        }
    }
}
