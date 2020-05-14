namespace Bones3Rebuilt
{
    /// <summary>
    /// Generates a visual chunk mesh for a single submesh layer.
    /// </summary>
    public class VisualRemeshTask : VoxelChunkMesher
    {
        private readonly int m_AtlasTarget;

        /// <inheritdoc cref="VoxelChunkMesher"/>
        public VisualRemeshTask(IChunkProperties chunkProperties, int atlasTarget):
            base(chunkProperties, false, true)
            {
                m_AtlasTarget = atlasTarget;
            }

        /// <inheritdoc cref="VoxelChunkMesher"/>
        public override bool CanPlaceQuad(IChunkProperties chunkProperties, BlockPosition pos, int side)
        {
            var block = chunkProperties.GetBlock(pos);
            if (block.Face(side).TextureAtlas != m_AtlasTarget)
                return false;

            var next = chunkProperties.GetNextBlock(pos, side);
            return block.IsVisible && !next.IsVisible;
        }
    }
}
