using Bones3Rebuilt;

namespace WraithavenGames.Bones3
{
    public interface IBlockLoader
    {
        /// <summary>
        /// Loads the default set of blocks into the given block list.
        /// </summary>
        /// <param name="blockList">The block list to write to.</param>
        void LoadBlocks(IBlockTypeList blockList);
    }
}
