using System;

namespace Bones3Rebuilt.Util
{
    /// <summary>
    /// A small and compact data structure for storing 3D floating point vector.
    /// </summary>
    public struct Vec3
    {
        /// <summary>
        /// Gets the X component of this vector.
        /// </summary>
        /// <value>The X component.</value>
        public float X { get; }

        /// <summary>
        /// Gets the Y component of this vector.
        /// </summary>
        /// <value>The Y component.</value>
        public float Y { get; }

        /// <summary>
        /// Gets the Z component of this vector.
        /// </summary>
        /// <value>The Z component.</value>
        public float Z { get; }

        /// <summary>
        /// Creates a new vector with the given coordinates.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        /// <param name="z">The Z component.</param>
        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static bool operator ==(Vec3 a, Vec3 b)
        {
            return Math.Abs(a.X - b.X) < 0.001f
                && Math.Abs(a.Y - b.Y) < 0.001f
                && Math.Abs(a.Z - b.Z) < 0.001f;
        }

        public static bool operator !=(Vec3 a, Vec3 b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is Vec3 vec && (this == vec);
        }

        public override int GetHashCode()
        {
            int hashCode = -307843816;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            return hashCode;
        }
    }
}