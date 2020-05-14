namespace Bones3Rebuilt
{
    /// <summary>
    /// Represents an editing tool which takes in a cubic region to modify.
    /// </summary>
    public interface IRegionalEdit
    {
        /// <summary>
        /// Sets the bounding position and fill pattern to use for the next edit.
        /// </summary>
        /// <param name="point1">One corner of the cubic area.</param>
        /// <param name="point2">The opposite corner of the cubic area.</param>
        /// <param name="fillPattern">The fill pattern to use.</param>
        void Set(BlockPosition point1, BlockPosition point2, IFillPattern fillPattern);
    }
}
