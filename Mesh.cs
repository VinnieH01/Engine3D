using Engine3D.Math;
using Engine3D.Rendering;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engine3D
{
	class Mesh
	{
		public ReadOnlyCollection<Triangle> Triangles { get; private set; }
		public Vector3 Position { get; set; }
		public Vector3 Rotation { get; set; }
		public Texture Texture { get; private set; }

		public Mesh(List<Triangle> triangles, Texture texture)
		{
			Triangles = triangles.AsReadOnly();
			Texture = texture;
		}
	}
}
