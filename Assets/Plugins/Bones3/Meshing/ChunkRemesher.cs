using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using System.Collections.Generic;
using WraithavenGames.Bones3.BlockProperties;

namespace WraithavenGames.Bones3.Meshing
{
    /// <summary>
    /// The <c>ChunkRemesher</c> is a MonoBehavior which acts as a remesh manager. It simply passes
    /// along chunk remesh requests to the job system, and each frame, once the requests are complete,
    /// the corresponding chunks are updated in Unity.
    /// </summary>
    public class ChunkRemesher
    {
        private ChunkTaskPool pool = new ChunkTaskPool();

        public void Remesh(Chunk chunk)
        {
            ChunkRemeshOperation op = new ChunkRemeshOperation(pool);

            op.Init(chunk);
            op.Finish();

            pool.Dispose();
        }

        public void Dispose()
        {
            pool.Dispose();
        }
    }
}