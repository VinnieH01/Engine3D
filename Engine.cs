using System;
using System.Collections.Generic;
using Engine3D.Rendering;
using Engine3D.Math;
using Engine3D.Rendering.Lighting;

namespace Engine3D
{
	public class Engine
	{
		private readonly Camera camera;
		private readonly Mesh mesh;
		private float angle;
		private readonly List<Light> lights;

		public Engine()
		{
			camera = new Camera(60f, 1f, 40)
			{
				Position = new Vector3(0, -5, -10)
			};

			lights = new List<Light>
			{
				new AmbientLight(new(0.5f, 1, 0.5f)),
				new PointLight(new(5,0,0), new(1, 0, 0), 5),
				new DirectionalLight(new(2,2,2), new(1, 0, 0))
			};

			/*Texture diamondTexture = TextureLoader.LoadTexture("Assets/diamond.png");

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
			triangles.Add(new Triangle(new Vertex(1, -1, 1, 0, 1), new Vertex(1, 1, -1, 1, 0), new Vertex(1, 1, 1, 1, 1), diamondTexture));*/

			mesh = ObjLoader.LoadObj("Assets/Model.obj", TextureLoader.LoadTexture("Assets/Cat_diffuse.jpg"));
		}

		public void Draw(ScreenBuffer drawBuffer, DepthBuffer depthBuffer, float deltaTime)
		{
			Console.WriteLine(1.0f / deltaTime);
			angle += 80 * deltaTime;
			camera.Update(deltaTime);
			//mesh.Position = new Vector3(0, 0, 0);
			//mesh.Rotation = new Vector3(angle * 0.5f, angle, angle * 0.2f);

			Renderer.Draw(mesh, lights, drawBuffer, depthBuffer, camera);

			//Draw depth buffer
			/*for(int x = 0; x < drawBuffer.Width; x++)
			{
				for (int y = 0; y < drawBuffer.Height; y++)
				{
					float depth = System.Math.Clamp(depthBuffer.GetDepth(x, y), 0, camera.Far);
					uint color = (uint)MathUtils.Map(depth, 0, camera.Far, 0, 255);
					drawBuffer.SetPixel(x, y, color);
				}
			}*/
		}
	}
}
