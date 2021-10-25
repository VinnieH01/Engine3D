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
            List<(Vertex[], Texture)> viewTriangles = new List<(Vertex[], Texture)>();

            foreach(Triangle triangle in triangles)
            {
                (Vertex[] globalVertices, Texture texture) = triangle.CalculateGlobalVertices();

                Vertex[] viewVertices = CalculateViewVertices(globalVertices, camera);

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

            //Sort the triangles so that ones further away are drawn first
            viewTriangles.Sort(((Vertex[] vertices, Texture) a, (Vertex[] vertices, Texture) b) => {
                float aAverageZ = (a.vertices[0].Position.Z + a.vertices[1].Position.Z + a.vertices[2].Position.Z) / 3.0f;
                float bAverageZ = (b.vertices[0].Position.Z + b.vertices[1].Position.Z + b.vertices[2].Position.Z) / 3.0f;
                return aAverageZ > bAverageZ ? -1 : aAverageZ > bAverageZ ? 1 : 0; 
            });

            foreach ((Vertex[] vertices, Texture texture) in viewTriangles)
            {
                ScreenPoint[] screenPoints = ProjectTriangleTo2D(vertices, camera, buffer);
                DrawTriangle(screenPoints, texture, buffer);
            }
        }

        public Vertex[] CalculateViewVertices(Vertex[] vertices, Camera camera)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = vertices[i].Rotated(-camera.Rotation).Translated(-camera.Position);
            }

            return vertices;
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
