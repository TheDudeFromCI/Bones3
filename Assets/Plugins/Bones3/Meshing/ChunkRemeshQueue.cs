using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace WraithavenGames.Bones3.Meshing
{
	public class ChunkRemeshQueue
	{
		private LinkedList<IChunkRemeshTask> queue;
		private LinkedList<IChunkRemeshTask> finishedTasks;

		private bool running = true;
		private Thread[] thread;

		public ChunkRemeshQueue(int workers)
		{
			queue = new LinkedList<IChunkRemeshTask>();
			finishedTasks = new LinkedList<IChunkRemeshTask>();

			thread = new Thread[workers];
			for (int i = 0; i < workers; i++)
			{
				thread[i] = new Thread(Run);
				thread[i].Start();
			}
		}

		public void AddTask(IChunkRemeshTask task)
		{
			bool allow = true;

			lock(queue)
			{
				foreach (IChunkRemeshTask t in queue)
					if (t.MatchesTask(task))
					{
						allow = false;
						break;
					}

				if (allow)
					queue.AddLast(task);
			}

			if (!allow)
				task.CleanupRemesh();
		}

		public void KillThread()
		{
			running = false;

			for (int i = 0; i < thread.Length; i++)
				thread[i].Join();
		}

		public void FinishTasks()
		{
			lock(finishedTasks)
			{
				foreach (IChunkRemeshTask task in finishedTasks)
				{
					task.FinishTask();
					task.CleanupRemesh();
				}

				finishedTasks.Clear();
			}
		}

		private void Run()
		{
			try
			{
				IChunkRemeshTask task;
				while (running)
				{
					lock (queue)
					{
						if (queue.Count == 0)
							task = null;
						else
						{
							task = queue.First.Value;
							queue.Remove(task);
						}
					}

					if (task == null)
					{
						Thread.Sleep(1);
						continue;
					}

					task.RemeshChunk();

					lock (finishedTasks)
					{
						finishedTasks.AddLast(task);
					}
				}
			}
			catch(Exception exception)
			{
				Debug.LogError(exception);
			}
		}
	}
}
