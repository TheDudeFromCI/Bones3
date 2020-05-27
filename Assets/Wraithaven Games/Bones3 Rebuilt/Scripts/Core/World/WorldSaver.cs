using System.Collections.Generic;

using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// The WorldSaver class can be used to save a world from file.
    /// </summary>
    internal class WorldSaver
    {
        private readonly World m_World;
        private readonly string m_ChunkFolder;

        /// <summary>
        /// Creates a new world saver object.
        /// </summary>
        /// <param name="world">The world to manage.</param>
        internal WorldSaver(World world)
        {
            m_World = world;

#if UNITY_EDITOR
            m_ChunkFolder = $"{Application.dataPath}/../Bones3/Worlds/{m_World.ID}/Chunks";
#else
            m_ChunkFolder = $"{Application.persistentDataPath}/Worlds/{m_World.ID}/Chunks";
#endif
        }

        /// <summary>
        /// Saves all chunks in the world.
        /// </summary>
        internal void SaveWorld()
        {
            List<ChunkSaveOperation> operations = new List<ChunkSaveOperation>();

            foreach (var chunk in m_World.ChunkIterator())
            {
                if (chunk.IsModified)
                    operations.Add(new ChunkSaveOperation(m_ChunkFolder, chunk));
            }

            List<System.Exception> exceptions = new List<System.Exception>();
            foreach (var op in operations)
            {
                try
                {
                    op.FinishTask();
                }
                catch (System.Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Count > 0) // We still want to throw all exceptions, but ensure tasks finish first.
                throw new System.AggregateException(exceptions);
        }
    }
}
