using System;
using System.Collections.Generic;
using Engine3D.Rendering;
using Engine3D.Math;

namespace Engine3D
{
    public class Engine
    {
        private readonly Camera camera;
        private float angle;
        private readonly List<Triangle> triangles;
        private readonly Rasterizer rasterizer;

        public Engine()
        {
            camera = new Camera(500, 45);
            rasterizer = new Rasterizer();
            triangles = new List<Triangle>();

            Texture diamondTexture = TextureLoader.LoadTexture("Assets/diamond.png");

            triangles.Add(new Triangle(new Vertex(-100, -100, -100, 0, 0), new Vertex(-100, 100, -100, 0, 1), new Vertex(100, -100, -100, 1, 0), diamondTexture));
            triangles.Add(new Triangle(new Vertex(-100, 100, -100, 0, 1), new Vertex(100, 100, -100, 1, 1), new Vertex(100, -100, -100, 1, 0), diamondTexture));
            // back
            triangles.Add(new Triangle(new Vertex(100, -100, 100, 1, 0), new Vertex(-100, 100, 100, 0, 1), new Vertex(-100, -100, 100, 0, 0), diamondTexture));
            triangles.Add(new Triangle(new Vertex(100, -100, 100, 1, 0), new Vertex(100, 100, 100, 1, 1), new Vertex(-100, 100, 100, 0, 1), diamondTexture));
            // left
            triangles.Add(new Triangle(new Vertex(-100, -100, 100, 0, 1), new Vertex(-100, -100, -100, 0, 0), new Vertex(100, -100, 100, 1, 1), diamondTexture));
            triangles.Add(new Triangle(new Vertex(-100, -100, -100, 0, 0), new Vertex(100, -100, -100, 1, 0), new Vertex(100, -100, 100, 1, 1), diamondTexture));
            // right
            triangles.Add(new Triangle(new Vertex(-100, 100, -100, 0, 0), new Vertex(100, 100, 100, 1, 1), new Vertex(100, 100, -100, 1, 0), diamondTexture));
            triangles.Add(new Triangle(new Vertex(-100, 100, -100, 0, 0), new Vertex(-100, 100, 100, 0, 1), new Vertex(100, 100, 100, 1, 1), diamondTexture));
            // top
            triangles.Add(new Triangle(new Vertex(-100, -100, 100, 0, 1), new Vertex(-100, 100, -100, 1, 0), new Vertex(-100, -100, -100, 0, 0), diamondTexture));
            triangles.Add(new Triangle(new Vertex(-100, -100, 100, 0, 1), new Vertex(-100, 100, 100, 1, 1), new Vertex(-100, 100, -100, 1, 0), diamondTexture));
            // bottom
            triangles.Add(new Triangle(new Vertex(100, -100, -100, 0, 0), new Vertex(100, 100, -100, 1, 0), new Vertex(100, -100, 100, 0, 1), diamondTexture));
            triangles.Add(new Triangle(new Vertex(100, -100, 100, 0, 1), new Vertex(100, 100, -100, 1, 0), new Vertex(100, 100, 100, 1, 1), diamondTexture));
        }

        public void Draw(ScreenBuffer buffer, float deltaTime)
        {
            Console.WriteLine(1.0f / deltaTime);
            angle += 80 * deltaTime;
            foreach (Triangle t in triangles)
            {
                t.Position = new Vector3(0, 0, 800);
                t.Rotation = new Vector3(angle * 0.5f, angle, angle * 0.2f);
            }
            rasterizer.Draw(triangles, buffer, camera);
        }
    }
}
