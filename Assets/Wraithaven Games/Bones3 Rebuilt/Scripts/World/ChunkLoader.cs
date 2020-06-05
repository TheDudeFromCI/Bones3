using System.Collections.Generic;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A handler for loading chunks sync and async.
    /// </summary>
    internal class ChunkLoader
    {
        private readonly List<IChunkLoadHandler> m_ChunkLoadHandlers = new List<IChunkLoadHandler>();

        /// <summary>
        /// Triggers a chunk to load in a background task. The chunk should *not* be accessed in
        /// any way until this task is finished. The task can be force finished by calling
        /// `Finish(chunk);`
        /// 
        /// This method does nothing if the chunk is already being handled.
        /// </summary>
        /// <param name="chunk">The chunk to load.</param>
        /// <returns>True if the chunk needs to be remeshed. False otherwise.</returns>
        internal bool Load(Chunk chunk)
        {
            bool remesh = false;
            foreach (var handler in m_ChunkLoadHandlers)
                remesh |= handler.OnChunkLoad(chunk, remesh);

            return remesh;
        }

        /// <summary>
        /// Adds a new chunk load handler to this world. Handlers are executed on
        /// chunks in the order they are added to this loaded.
        /// </summary>
        /// <param name="loadHandler">The handler.</param>
        internal void AddChunkLoadHandler(IChunkLoadHandler loadHandler) => m_ChunkLoadHandlers.Add(loadHandler);
    }
}
