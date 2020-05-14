namespace Bones3Rebuilt
{
    /// <summary>
    /// Generates the voxel collision mesh for a chunk.
    /// </summary>
    public class CollisionRemeshTask : VoxelChunkMesher
    {
        /// <inheritdoc cref="VoxelChunkMesher"/>
        public CollisionRemeshTask(IChunkProperties chunkProperties) :
            base(chunkProperties, true, false)
        { }

        /// <inheritdoc cref="VoxelChunkMesher"/>
        public override bool CanPlaceQuad(IChunkProperties chunkProperties, BlockPosition pos, int side)
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