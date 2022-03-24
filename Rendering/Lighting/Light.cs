using Engine3D.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine3D.Rendering.Lighting
{
	public abstract class Light
	{
		protected Vector3 color;

		public Light(Vector3 color)
		{
			this.color = color;
		}

		public abstract Vector3 CalculateLightAmount(Vector3 position, Vector3 normal);
	}
}
