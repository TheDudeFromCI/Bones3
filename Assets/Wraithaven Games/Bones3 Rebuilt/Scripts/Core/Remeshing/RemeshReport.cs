namespace Bones3Rebuilt
{
    public class RemeshReport
    {
        /// <summary>
        /// Gets the Guid of the chunk that was remeshed.
        /// </summary>
        /// <value>The chunk Guid.</value>
        public ChunkPosition ChunkPosition { get; }

        /// <summary>
        /// Gets the collision mesh that was generated.
        /// </summary>
        /// <value>The collision mesh.</value>
        public LayeredProcMesh CollisionMesh { get; }

        /// <summary>
        /// Gets the visual mesh that was generated.
        /// </summary>
        /// <value>The visual mesh.</value>
        public LayeredProcMesh VisualMesh { get; }

        /// <summary>
        /// Creates a new remesh report.
        /// </summary>
        /// <param name="chunkPosition">The position of the chunk that was remeshed.</param>
        /// <param name="collisionMesh">The new collision mesh.</param>
        /// <param name="visualMesh">The new visual mesh.</param>
        public RemeshReport(ChunkPosition chunkPosition, LayeredProcMesh collisionMesh, LayeredProcMesh visualMesh)
        {
            ChunkPosition = chunkPosition;
            CollisionMesh = collisionMesh;
            VisualMesh = visualMesh;
        }
    }
}