using UnityEngine;
using System.Collections;
using WraithavenGames.Bones3;

namespace WraithavenGames.Bones3.Meshing
{
	public class RemeshManager : MonoBehaviour
	{
		[Range(1,4)] public int workerThreads = 2;

		private ChunkRemeshQueue queue;
		private ArrayList standardRemeshPool;
		private Transform player;

		private void Awake()
		{
			queue = new ChunkRemeshQueue(workerThreads);
			standardRemeshPool = new ArrayList();

			player = Camera.main.transform;
		}

		public void AddTask(IChunkRemeshTask task)
		{
			queue.AddTask(task);
		}

		private void Update()
		{
			queue.FinishTasks();
		}

		private void OnDisable()
		{
			queue.KillThread();
		}

		public void AddStandardRemeshTask(Chunk chunk)
		{
			StandardChunkRemesh task;
			lock (standardRemeshPool)
			{
				task = TakePool_Standard();
				task.MarkActive();
			}

			task.Initalize(player, chunk);
			AddTask(task);
		}

		private StandardChunkRemesh TakePool_Standard()
		{
			for (int i = 0; i < standardRemeshPool.Count; i++)
				if (!((StandardChunkRemesh)standardRemeshPool[i]).IsBeginUsed())
					return (StandardChunkRemesh) standardRemeshPool[i];

			StandardChunkRemesh task = new StandardChunkRemesh();
			standardRemeshPool.Add(task);
			return task;
		}
	}
}
