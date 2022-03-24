using Engine3D.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine3D.Rendering.Lighting
{
	class AmbientLight : Light
	{
		public AmbientLight(Vector3 color)
			: base(color) { }

		public override Vector3 CalculateLightAmount(Vector3 position, Vector3 normal)
		{
			return color;
		}
	}
}
