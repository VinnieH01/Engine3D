using System;
using System.Collections.Generic;
using Engine3D.Rendering;
using Engine3D.Math;

namespace Engine3D
{
    public class Engine
    {
        private readonly Camera camera;
        private readonly List<Triangle> triangles;
        private readonly Renderer renderer;
        private float angle;

        public Engine()
        {
            camera = new Camera(60f, 1f, 10)
            {
                Position = new Vector3(0, 0, -5)
            };

            renderer = new Renderer();
            triangles = new List<Triangle>();

            Texture diamondTexture = TextureLoader.LoadTexture("Assets/diamond.png");

            triangles.Add(new Triangle(new Vertex(-1, -1, -1, 0, 0), new Vertex(-1, 1, -1, 0, 1), new Vertex(1, -1, -1, 1, 0), diamondTexture));
            triangles.Add(new Triangle(new Vertex(-1, 1, -1, 0, 1), new Vertex(1, 1, -1, 1, 1), new Vertex(1, -1, -1, 1, 0), diamondTexture));
            // back
            triangles.Add(new Triangle(new Vertex(1, -1, 1, 1, 0), new Vertex(-1, 1, 1, 0, 1), new Vertex(-1, -1, 1, 0, 0), diamondTexture));
            triangles.Add(new Triangle(new Vertex(1, -1, 1, 1, 0), new Vertex(1, 1, 1, 1, 1), new Vertex(-1, 1, 1, 0, 1), diamondTexture));
            // left
            triangles.Add(new Triangle(new Vertex(-1, -1, 1, 0, 1), new Vertex(-1, -1, -1, 0, 0), new Vertex(1, -1, 1, 1, 1), diamondTexture));
            triangles.Add(new Triangle(new Vertex(-1, -1, -1, 0, 0), new Vertex(1, -1, -1, 1, 0), new Vertex(1, -1, 1, 1, 1), diamondTexture));
            // right
            triangles.Add(new Triangle(new Vertex(-1, 1, -1, 0, 0), new Vertex(1, 1, 1, 1, 1), new Vertex(1, 1, -1, 1, 0), diamondTexture));
            triangles.Add(new Triangle(new Vertex(-1, 1, -1, 0, 0), new Vertex(-1, 1, 1, 0, 1), new Vertex(1, 1, 1, 1, 1), diamondTexture));
            // top
            triangles.Add(new Triangle(new Vertex(-1, -1, 1, 0, 1), new Vertex(-1, 1, -1, 1, 0), new Vertex(-1, -1, -1, 0, 0), diamondTexture));
            triangles.Add(new Triangle(new Vertex(-1, -1, 1, 0, 1), new Vertex(-1, 1, 1, 1, 1), new Vertex(-1, 1, -1, 1, 0), diamondTexture));
            // bottom
            triangles.Add(new Triangle(new Vertex(1, -1, -1, 0, 0), new Vertex(1, 1, -1, 1, 0), new Vertex(1, -1, 1, 0, 1), diamondTexture));
            triangles.Add(new Triangle(new Vertex(1, -1, 1, 0, 1), new Vertex(1, 1, -1, 1, 0), new Vertex(1, 1, 1, 1, 1), diamondTexture));
        }

        public void Draw(ScreenBuffer drawBuffer, DepthBuffer depthBuffer, float deltaTime)
        {
            Console.WriteLine(1.0f / deltaTime);
            angle += 80 * deltaTime;
            camera.Update(deltaTime);
            foreach (Triangle t in triangles)
            {
                t.Position = new Vector3(0, 0, 0);
                t.Rotation = new Vector3(angle * 0.5f, angle, angle * 0.2f);
            }
            renderer.Draw(triangles, drawBuffer, depthBuffer, camera);
        }
    }
}
