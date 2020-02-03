using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using System;

namespace WraithavenGames.Bones3.Terrain
{
	public class TerrainWorker
	{
		private readonly object LOCK_1 = true;
		private readonly object LOCK_2 = true;
		private LinkedList<ITerrainWorkerTask> tasks;
		private LinkedList<ITerrainWorkerTask> finishedTasks;
		private bool idle = true;

		private bool running = true;
		private Thread thread;

		public TerrainWorker()
		{
			tasks = new LinkedList<ITerrainWorkerTask>();
			finishedTasks = new LinkedList<ITerrainWorkerTask>();

			thread = new Thread(Run);
			thread.Start();
		}

		public void AddTaskToQueue(ITerrainWorkerTask task)
		{
			task.PrepareTask();

			lock (LOCK_1)
			{
				idle = false;
				tasks.AddLast(task);
			}
		}

		public bool IsIdle()
		{
			lock(LOCK_1)
			{
				return idle;
			}
		}

		public void FinalizeFinishTasks()
		{
			lock(LOCK_2)
			{
				foreach(ITerrainWorkerTask task in finishedTasks)
					task.FinishTask();
				finishedTasks.Clear();
			}
		}

		public void KillThread()
		{
			running = false;
			thread.Join();
		}

		private void Run()
		{
			try
			{
				ITerrainWorkerTask task;

				while(running)
				{
					// Get next task, or wait until available
					{
						lock(LOCK_1)
						{
							if (tasks.Count == 0)
								task = null;
							else
							{
								task = tasks.First.Value;
								tasks.RemoveFirst();
							}
						}

						if (task == null)
						{
							Thread.Sleep(1);
							continue;
						}
					}

					// Run task
					task.RunTask();

					lock (LOCK_1)
					{
						if (tasks.Count == 0)
							idle = true;
					}

					// Move task to finished list
					lock (LOCK_2)
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
