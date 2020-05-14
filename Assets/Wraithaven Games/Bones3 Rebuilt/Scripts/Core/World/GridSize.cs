namespace Bones3Rebuilt
{
    /// <summary>
    /// A measurement for efficient grid sizes and utilities.
    /// </summary>
    public struct GridSize
    {
        private readonly int bits;

        /// <summary>
        /// Gets the number of bits for this grid size.
        /// </summary>
        /// <value>
        /// The bit value such as that 2^bits can be used to
        /// represent the size of the grid along a single axis.
        /// </value>
        public int IntBits => bits;

        /// <summary>
        /// Gets the value of the grid size in units.
        /// </summary>
        /// <value>The grid size value.</value>
        public int Value => 1 << IntBits;

        /// <summary>
        /// Gets the mask value for for converting global coords into local coords.
        /// </summary>
        /// <value>The grid size mask.</value>
        public int Mask => Value - 1;

        /// <summary>
        /// Gets the number of elements within the volume of a single grid cube.
        /// </summary>
        public int Volume => Value * Value * Value;

        /// <summary>
        /// Creates a new grid size with the given bit value.
        /// </summary>
        /// <param name="bits">The bit value for this grid size.</param>
        public GridSize(int bits)
        {
            if (bits < 0)
                throw new System.ArgumentOutOfRangeException($"Grid size cannot be negative!");

            this.bits = bits;
        }

        public override string ToString()
        {
            return $"GridSize:[{Value}]";
        }
    }
}
