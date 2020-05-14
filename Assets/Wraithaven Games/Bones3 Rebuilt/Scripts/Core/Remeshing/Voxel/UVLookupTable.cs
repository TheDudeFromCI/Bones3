using System.Collections.Generic;
using Bones3Rebuilt.Util;

namespace Bones3Rebuilt
{
    /// <summary>
    /// A lookup table for uv coordinates for quads.
    /// </summary>
    public class UVLookupTable
    {
        /// <summary>
        /// The quad uv information.
        /// </summary>
        public struct UVSpecs
        {
            /// <summary>
            /// The side of the block.
            /// </summary>
            public int Side { get; set; }

            /// <summary>
            /// The Texture rotation.
            /// </summary>
            public int Rotation { get; set; }

            /// <summary>
            /// The quad width.
            /// </summary>
            public int W { get; set; }

            /// <summary>
            /// The quad height.
            /// </summary>
            public int H { get; set; }

            /// <summary>
            /// The Texture index to apply to this quad.
            /// </summary>
            public int Texture { get; set; }
        }

        /// <summary>
        /// Retrieves the given uv coords and stores them in the uv list.
        /// </summary>
        /// <param name="uvs">The list of UVs to write to.</param>
        /// <param name="specs">The quad uv information.</param>
        public void Find(List<Vec3> uvs, UVSpecs specs)
        {
            if (specs.Side < 0 || specs.Side > 5)
                throw new System.ArgumentException($"Unknown side {specs.Side}!");

            if (specs.Rotation < 0 || specs.Rotation > 7)
                throw new System.ArgumentException($"Unknown rotation {specs.Rotation}!");

            if (specs.Side % 2 == 0)
                FindFront(uvs, specs);
            else
                FindBack(uvs, specs);
        }

        void FindFront(List<Vec3> uvs, UVSpecs specs)
        {
            if (specs.Rotation < 4)
                FindFrontNormal(uvs, specs);
            else
                FindFrontMirrored(uvs, specs);
        }

        void FindBack(List<Vec3> uvs, UVSpecs specs)
        {
            if (specs.Rotation < 4)
                FindBackNormal(uvs, specs);
            else
                FindBackMirrored(uvs, specs);
        }

        void FindFrontNormal(List<Vec3> uvs, UVSpecs specs)
        {
            switch (specs.Rotation)
            {
                case 0:
                    uvs.Add(new Vec3(specs.H, specs.W, specs.Texture));
                    uvs.Add(new Vec3(specs.H, 0, specs.Texture));
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    uvs.Add(new Vec3(0, specs.W, specs.Texture));
                    break;

                case 1:
                    uvs.Add(new Vec3(0, specs.H, specs.Texture));
                    uvs.Add(new Vec3(specs.W, specs.H, specs.Texture));
                    uvs.Add(new Vec3(specs.W, 0, specs.Texture));
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    break;

                case 2:
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    uvs.Add(new Vec3(0, specs.W, specs.Texture));
                    uvs.Add(new Vec3(specs.H, specs.W, specs.Texture));
                    uvs.Add(new Vec3(specs.H, 0, specs.Texture));
                    break;

                case 3:
                    uvs.Add(new Vec3(specs.W, 0, specs.Texture));
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    uvs.Add(new Vec3(0, specs.H, specs.Texture));
                    uvs.Add(new Vec3(specs.W, specs.H, specs.Texture));
                    break;
            }
        }

        void FindFrontMirrored(List<Vec3> uvs, UVSpecs specs)
        {
            switch (specs.Rotation)
            {
                case 4:
                    uvs.Add(new Vec3(0, specs.W, specs.Texture));
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    uvs.Add(new Vec3(specs.H, 0, specs.Texture));
                    uvs.Add(new Vec3(specs.H, specs.W, specs.Texture));
                    break;

                case 5:
                    uvs.Add(new Vec3(specs.W, specs.H, specs.Texture));
                    uvs.Add(new Vec3(0, specs.H, specs.Texture));
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    uvs.Add(new Vec3(specs.W, 0, specs.Texture));
                    break;

                case 6:
                    uvs.Add(new Vec3(specs.H, 0, specs.Texture));
                    uvs.Add(new Vec3(specs.H, specs.W, specs.Texture));
                    uvs.Add(new Vec3(0, specs.W, specs.Texture));
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    break;

                case 7:
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    uvs.Add(new Vec3(specs.W, 0, specs.Texture));
                    uvs.Add(new Vec3(specs.W, specs.H, specs.Texture));
                    uvs.Add(new Vec3(0, specs.H, specs.Texture));
                    break;
            }
        }

        void FindBackNormal(List<Vec3> uvs, UVSpecs specs)
        {
            switch (specs.Rotation)
            {
                case 0:
                    uvs.Add(new Vec3(specs.H, 0, specs.Texture));
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    uvs.Add(new Vec3(0, specs.W, specs.Texture));
                    uvs.Add(new Vec3(specs.H, specs.W, specs.Texture));
                    break;

                case 1:
                    uvs.Add(new Vec3(specs.W, specs.H, specs.Texture));
                    uvs.Add(new Vec3(specs.W, 0, specs.Texture));
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    uvs.Add(new Vec3(0, specs.H, specs.Texture));
                    break;

                case 2:
                    uvs.Add(new Vec3(0, specs.W, specs.Texture));
                    uvs.Add(new Vec3(specs.H, specs.W, specs.Texture));
                    uvs.Add(new Vec3(specs.H, 0, specs.Texture));
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    break;

                case 3:
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    uvs.Add(new Vec3(0, specs.H, specs.Texture));
                    uvs.Add(new Vec3(specs.W, specs.H, specs.Texture));
                    uvs.Add(new Vec3(specs.W, 0, specs.Texture));
                    break;
            }
        }

        void FindBackMirrored(List<Vec3> uvs, UVSpecs specs)
        {
            switch (specs.Rotation)
            {
                case 4:
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    uvs.Add(new Vec3(specs.H, 0, specs.Texture));
                    uvs.Add(new Vec3(specs.H, specs.W, specs.Texture));
                    uvs.Add(new Vec3(0, specs.W, specs.Texture));
                    break;

                case 5:
                    uvs.Add(new Vec3(0, specs.H, specs.Texture));
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    uvs.Add(new Vec3(specs.W, 0, specs.Texture));
                    uvs.Add(new Vec3(specs.W, specs.H, specs.Texture));
                    break;

                case 6:
                    uvs.Add(new Vec3(specs.H, specs.W, specs.Texture));
                    uvs.Add(new Vec3(0, specs.W, specs.Texture));
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    uvs.Add(new Vec3(specs.H, 0, specs.Texture));
                    break;

                case 7:
                    uvs.Add(new Vec3(specs.W, 0, specs.Texture));
                    uvs.Add(new Vec3(specs.W, specs.H, specs.Texture));
                    uvs.Add(new Vec3(0, specs.H, specs.Texture));
                    uvs.Add(new Vec3(0, 0, specs.Texture));
                    break;
            }
        }
    }
}
