namespace WraithavenGames.Bones3
{
    /// <summary>
    /// The block being targeting while world editing.
    /// </summary>
    public struct TargetBlock
    {
        private int side;

        /// <summary>
        /// The block coordinates the cursor is in.
        /// </summary>
        public BlockPosition Inside { get; set; }

        /// <summary>
        /// The block coordinates the cursor is over.
        /// </summary>
        public BlockPosition Over { get; set; }

        /// <summary>
        /// The side of the block being targeted.
        /// </summary>
        public int Side
        {
            get => side;
            set
            {
                if (value < 0 || value > 5)
                    throw new System.ArgumentException($"Unknown direction {value}!");

                side = value;
            }
        }

        /// <summary>
        /// Whether or not a block is being targeted, or if the target is infinity.
        /// </summary>
        public bool HasBlock { get; set; }

        // TODO Remove, legacy code
        public bool HasShift { get; set; }
    }
}
