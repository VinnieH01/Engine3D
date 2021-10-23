using Engine3D.Math;

namespace Engine3D.Rendering
{
    public class ScreenPoint
    {
        /// <summary>
        /// The x coordinate of the <c>ScreenPoint</c> in screen space.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The y coordinate of the <c>ScreenPoint</c> in screen space.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The z coordinate of the <c>ScreenPoint</c> in 3D space.
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// The UV coordinate of the <c>Vertex</c> this <c>ScreenPoint</c> represents.
        /// </summary>
        public Vector2 UV { get; set; }

        /// <summary>
        /// <c>UV/z</c> Used for perspective correct texturing.
        /// </summary>
        public Vector2 UVoverZ { get; set; }

        /// <summary>
        /// <c>1/z</c> Used for perspective correct texturing.
        /// </summary>
        public float ZInv { get; set; }

        public ScreenPoint(int x, int y, float z, Vector2 uv)
        {
            X = x;
            Y = y;
            Z = z;
            UV = uv;
            UVoverZ = uv / z; 
            ZInv = 1.0f / z;
        }

        public ScreenPoint(int x, int y, float z, Vector2 uv, Vector2 UVoverZ, float zInv)
        {
            X = x;
            Y = y;
            Z = z;
            UV = uv;
            this.UVoverZ = UVoverZ;
            ZInv = zInv;
        }

        public ScreenPoint(int x, int y, float z, float u, float v)
            : this(x, y, z, new Vector2(u, v)) { }

        public ScreenPoint(ScreenPoint other)
            : this(other.X, other.Y, other.Z, other.UV) { }
    }
}
