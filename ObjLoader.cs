using Engine3D.Math;
using Engine3D.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Engine3D
{
	class ObjLoader
	{
		public static Mesh LoadObj(string filepath, Texture texture)
		{
			string[] objFile = File.ReadAllLines(filepath);

			List<Vector3> vertexPositions = new();
			List<Vector2> vertexUVs = new();
			List<Triangle> result = new();

			foreach(string line in objFile)
			{
				if(line.StartsWith("v "))
				{
					string[] coords = line.Split(" ");
					vertexPositions.Add(new(float.Parse(coords[1]), -float.Parse(coords[2]), float.Parse(coords[3])));
				}
				else if(line.StartsWith("vt "))
				{
					string[] coords = line.Split(" ");
					vertexUVs.Add(new Vector2(float.Parse(coords[1]), 1.0f - float.Parse(coords[2])));
				}
				else if (line.StartsWith("f "))
				{
					List<Vertex> vertices = new();
					string[] vertexData = line.Split(" ");
					for(int i = 1; i <= 3; i++)
					{
						string[] data = vertexData[i].Split("/");
						vertices.Add(new Vertex(vertexPositions[int.Parse(data[0]) - 1], vertexUVs[int.Parse(data[1]) - 1]));
					}
					result.Add(new(vertices[0], vertices[2], vertices[1]));
				}
			}
			return new(result, texture);
		}
	}
}
