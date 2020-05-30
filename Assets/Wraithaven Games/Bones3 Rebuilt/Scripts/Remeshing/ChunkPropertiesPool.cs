using System.Collections.Generic;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A pool for holding chunk properties objects.
    /// </summary>
    internal class ChunkPropertiesPool
    {
        private const int MAX_OBJECTs = 16;

        private readonly List<ChunkProperties> m_Pool = new List<ChunkProperties>();
        private readonly GridSize m_ChunkSize;

        internal ChunkPropertiesPool(GridSize chunkSize)
        {
            m_ChunkSize = chunkSize;
        }

        /// <summary>
        /// Pulls an unused chunk properties object from this pool, creating a new
        /// instance if needed.
        /// </summary>
        /// <returns>The chunk properties object.</returns>
        internal ChunkProperties Pull()
        {
            if (m_Pool.Count > 0)
            {
                var index = m_Pool.Count - 1;
                var prop = m_Pool[index];
                m_Pool.RemoveAt(index);
                return prop;
            }

            return new ChunkProperties(m_ChunkSize);
        }

        /// <summary>
        /// Returns a chunk properties object back to the pool after it is finished being used.
        /// </summary>
        /// <param name="props">The properties object to return.</param>
        internal void Return(ChunkProperties props)
        {
            if (m_Pool.Count >= MAX_OBJECTs)
                return;

            m_Pool.Add(props);
        }
    }
}