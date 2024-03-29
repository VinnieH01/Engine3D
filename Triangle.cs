﻿using Engine3D.Math;
using Engine3D.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Engine3D
{
	/// <summary>
	/// Class <c>Triangle</c> models a ngle in 3D space.
	/// </summary>
	public class Triangle
	{
		public Vertex[] Vertices { get; private set; }

		/// <summary>
		/// Defines a triangle in 3D space using 3 verticies passed in clockwise order.
		/// </summary>
		/// <param name="v0">The first vertex</param>
		/// <param name="v1">The second vertex</param>
		/// <param name="v2">The third vertex</param>
		public Triangle(Vertex v0, Vertex v1, Vertex v2)
		{
			Vertices = new Vertex[] { v0, v1, v2 };
		}
	}
}
