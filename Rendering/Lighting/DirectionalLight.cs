using Engine3D.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine3D.Rendering.Lighting
{
	class DirectionalLight : Light
	{
		private Vector3 direction;

		public DirectionalLight(Vector3 color, Vector3 direction)
			: base(color)
		{
			this.direction = direction.Normalized();
		}

		public override Vector3 CalculateLightAmount(Vector3 position, Vector3 normal)
		{
			return color * MathF.Max(-direction.Dot(normal), 0f);
		}
	}
}
