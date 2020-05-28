namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A measurement for efficient grid sizes and utilities.
    /// </summary>
    public struct GridSize
    {
        /// <summary>
        /// Gets the number of bits for this grid size.
        /// </summary>
        /// <value>
        /// The bit value such as that 2^bits can be used to
        /// represent the size of the grid along a single axis.
        /// </value>
        public int IntBits { get; }

        /// <summary>
        /// Gets the value of the grid size in units.
        /// </summary>
        /// <value>The grid size value.</value>
        public int Value { get; }

        /// <summary>
        /// Gets the mask value for for converting global coords into local coords.
        /// </summary>
        /// <value>The grid size mask.</value>
        public int Mask { get; }

        /// <summary>
        /// Gets the number of elements within the volume of a single grid cube.
        /// </summary>
        public int Volume { get; }

        /// <summary>
        /// Creates a new grid size with the given bit value.
        /// </summary>
        /// <param name="bits">The bit value for this grid size.</param>
        public GridSize(int bits)
        {
            if (bits < 0)
                throw new System.ArgumentOutOfRangeException($"Grid size cannot be negative!");

            IntBits = bits;
            Value = 1 << bits;
            Mask = Value - 1;
            Volume = Value * Value * Value;
        }

        public override string ToString()
        {
            return $"GridSize:[{Value}]";
        }
    }
}
