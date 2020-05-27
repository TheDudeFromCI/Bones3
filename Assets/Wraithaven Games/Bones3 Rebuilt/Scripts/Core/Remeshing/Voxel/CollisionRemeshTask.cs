namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Generates the voxel collision mesh for a chunk.
    /// </summary>
    internal class CollisionRemeshTask : VoxelChunkMesher
    {
        /// <inheritdoc cref="VoxelChunkMesher"/>
        internal CollisionRemeshTask(ChunkProperties chunkProperties, GreedyMesher mesher) :
            base(chunkProperties, mesher)
        { }

        /// <inheritdoc cref="VoxelChunkMesher"/>
        protected override bool CanPlaceQuad(ChunkProperties chunkProperties, BlockPosition pos, int side)
        {
            var block = chunkProperties.GetBlock(pos);
            if (!block.IsSolid)
                return false;

            var nextPos = pos;
            nextPos = nextPos.ShiftAlongDirection(side);
            if (!nextPos.IsWithinGrid(chunkProperties.ChunkSize))
                return true;

            var next = chunkProperties.GetBlock(nextPos);
            return !next.IsSolid;
        }
    }
}
