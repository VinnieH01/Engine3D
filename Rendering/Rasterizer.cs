using Engine3D.Math;
using System;
using System.Collections.Generic;

namespace Engine3D.Rendering
{
    class Rasterizer
    {
        /// <summary>
        /// Draws a list of triangles to the supplied screen buffer.
        /// </summary>
        /// <param name="triangles">The triangles to draw</param>
        /// <param name="buffer">The screen buffer to draw to</param>
        /// <param name="camera">The camera to use for projecting the triangles to the screen</param>
        public void Draw(List<Triangle> triangles, ScreenBuffer buffer, Camera camera)
        {
            List<(Vertex[], Texture)> globalTriangles = new List<(Vertex[], Texture)>();

            foreach(Triangle triangle in triangles)
            {
                (Vertex[] vertices, Texture) globalTriangle = triangle.CalculateGlobalVerticies();

                Vector3 u = globalTriangle.vertices[1].Position - globalTriangle.vertices[0].Position;
                Vector3 v = globalTriangle.vertices[2].Position - globalTriangle.vertices[0].Position;

                Vector3 normal = u.Cross(v);

                //If the triangle is facing away from the camera we dont want to draw it
                if (normal.Dot(camera.Direction) >= 0) continue;

                globalTriangles.Add(globalTriangle);
            }

            //Sort the triangles so that ones further away are drawn first
            globalTriangles.Sort(((Vertex[] vertices, Texture) a, (Vertex[] vertices, Texture) b) => {
                float aAverageZ = (a.vertices[0].Position.Z + a.vertices[1].Position.Z + a.vertices[2].Position.Z) / 3.0f;
                float bAverageZ = (b.vertices[0].Position.Z + b.vertices[1].Position.Z + b.vertices[2].Position.Z) / 3.0f;
                return aAverageZ > bAverageZ ? -1 : aAverageZ > bAverageZ ? 1 : 0; 
            });

            foreach ((Vertex[] vertices, Texture texture) in globalTriangles)
            {
                ScreenPoint[] screenPoints = ProjectTriangleTo2D(vertices, camera, buffer);
                DrawTriangle(screenPoints, texture, buffer);
            }
        }

        private static ScreenPoint[] ProjectTriangleTo2D(Vertex[] vertices, Camera camera, ScreenBuffer buffer)
        {
            if (vertices.Length != 3) throw new ArgumentException("A triangle has exactly 3 verticies.");

            ScreenPoint[] screenPoints = new ScreenPoint[3];

            for (int i = 0; i < 3; i++)
            {
                screenPoints[i] = camera.Project(vertices[i]);

                //As the buffer origin is at the top left of the screen while the
                //world origin is at the center we need to offset the points so
                //the origins line up.
                screenPoints[i].X += buffer.Width / 2;
                screenPoints[i].Y += buffer.Height / 2;
            }

            return screenPoints;
        }

        private void DrawTriangle(ScreenPoint[] screenPoints, Texture texture, ScreenBuffer buffer)
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
                //If there is no vertical difference between the points then we dont draw it (avoiding divide by zero)
                if (start.Y != end.Y)
                {
                    //Go through all vertical values from start to end point
                    for (int y = start.Y; y <= end.Y; y++)
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

                        DrawScanline(buffer, from, to, y, texture);
                    }
                }
            }

            DrawHalfTriangle(high, mid);
            DrawHalfTriangle(mid, low);
        }

        private void DrawScanline(ScreenBuffer buffer, ScreenPoint from, ScreenPoint to, int y, Texture texture)
        {
            for (int x = from.X; x <= to.X; x++)
            {
                float scanlineProgress = (float)(x - from.X) / (to.X - from.X);

                //A UV divided by the z coord of the projected vertex is used for perspective correct texture mapping.
                //This is why 1/z (zInv) is later used to correct back to normal UV space.
                //see https://en.wikipedia.org/wiki/Texture_mapping#Perspective_correctness for more info.
                Vector2 uv = MathUtils.Lerp(from.UVoverZ, to.UVoverZ, scanlineProgress);
                float zInv = MathUtils.Lerp(from.ZInv, to.ZInv, scanlineProgress);

                buffer.SetPixel(x, y, texture.GetPixel(new Vector2(uv.X / zInv, uv.Y / zInv)));
            }
        }
    }
}
