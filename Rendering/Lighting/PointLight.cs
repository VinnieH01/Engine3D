using Engine3D.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine3D.Rendering.Lighting
{
	class PointLight : Light
	{
		private Vector3 position;
		private readonly float maxDistance;

		public PointLight(Vector3 color, Vector3 position, float maxDistance)
			: base(color)
		{
			this.position = position;
			this.maxDistance = maxDistance;
		}

		public override Vector3 CalculateLightAmount(Vector3 position, Vector3 normal)
		{
			Vector3 direction = position - this.position;
			float distance = direction.Magnitude();
			if (distance < maxDistance)
				return color * MathF.Max(-direction.Normalized().Dot(normal), 0f) * (1 - (distance / maxDistance));
			else return new();
		}
	}
}
