namespace WraithavenGames.Bones3
{
    /// <summary>
    /// Generates the voxel collision mesh for a chunk.
    /// </summary>
    internal class CollisionRemeshTask : VoxelChunkMesher
    {
        /// <inheritdoc cref="VoxelChunkMesher"/>
        internal CollisionRemeshTask(ChunkGroup chunkProperties, GreedyMesher mesher) :
            base(chunkProperties, mesher)
        { }

        /// <inheritdoc cref="VoxelChunkMesher"/>
        protected override bool CanPlaceQuad(ChunkGroup chunkProperties, BlockPosition pos, int side)
        {
            var block = chunkProperties.GetBlock(pos);
            if (!block.Solid)
                return false;

            var nextPos = pos;
            nextPos = nextPos.ShiftAlongDirection(side);
            if (!nextPos.IsWithinGrid(chunkProperties.ChunkSize))
                return true;

            var next = chunkProperties.GetBlock(nextPos);
            return !next.Solid;
        }
    }
}
