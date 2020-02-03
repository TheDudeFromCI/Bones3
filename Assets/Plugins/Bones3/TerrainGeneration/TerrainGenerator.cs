using UnityEngine;
using WraithavenGames.Bones3;

namespace WraithavenGames.Bones3.Terrain
{
	[RequireComponent(typeof(BlockWorld))]
	[RequireComponent(typeof(ChunkLoadOrder))]
	public class TerrainGenerator : MonoBehaviour
	{
		public WorldPopulator worldPopulator;
		public Transform player;
		[Range(3,30)] public int chunkLoadRadius = 15;
		[Range(3,30)] public int chunkLoadHeight = 15;
		[Range(5,40)] public int chunkUnloadRadius = 20;
		[Range(1,7)] public int workerThreads = 1;
		public int pregenerateChunks = 50;
		public bool forceRegenerateChunks = false;

		private BlockWorld world;
		private ChunkLoadOrder loadOrder;
		private TerrainWorker[] workers;
		private GenerateChunkTask[] genTasks;

		private int lastChunkX;
		private int lastChunkY;
		private int lastChunkZ;

		private void Awake()
		{
			world = GetComponent<BlockWorld>();
			loadOrder = GetComponent<ChunkLoadOrder>();

			workers = new TerrainWorker[workerThreads];
			for (int i = 0; i < workerThreads; i++)
				workers[i] = new TerrainWorker();
			loadOrder.UpdateBufferRadius(chunkLoadRadius, chunkLoadHeight);

			genTasks = new GenerateChunkTask[workerThreads];
			for (int i = 0; i < workerThreads; i++)
				genTasks[i] = new GenerateChunkTask(worldPopulator, world);

			for (int i = 0; i < pregenerateChunks; i++)
				GenerateNextChunk(true, 0);
		}

		private void OnValidate()
		{
			chunkUnloadRadius = Mathf.Max(chunkUnloadRadius, Mathf.Max(chunkLoadRadius, chunkLoadHeight) + 1);
		}

		private void Start()
		{
			GetPlayerChunk();
		}

		private void OnDisable()
		{
			for (int i = 0; i < workers.Length; i++)
				workers[i].KillThread();
		}

		private void GetPlayerChunk()
		{
			Vector3 pos = player.position;
			pos = world.transform.InverseTransformPoint(pos);

			int chunkX = Mathf.RoundToInt(pos.x) >> 4;
			int chunkY = Mathf.RoundToInt(pos.y) >> 4;
			int chunkZ = Mathf.RoundToInt(pos.z) >> 4;

			if (chunkX == lastChunkX && chunkY == lastChunkY && chunkZ == lastChunkZ)
				return;

			lastChunkX = chunkX;
			lastChunkY = chunkY;
			lastChunkZ = chunkZ;
			loadOrder.Reset();
			world.UnloadDistantChunks(chunkX, chunkY, chunkZ, chunkUnloadRadius);
		}

		private void Update()
		{
			if (chunkLoadRadius != loadOrder.GetBufferRadius() || chunkLoadHeight != loadOrder.GetBufferHeight())
				loadOrder.UpdateBufferRadius(chunkLoadRadius, chunkLoadHeight);

			for (int i = 0; i < workers.Length; i++)
			{
				workers[i].FinalizeFinishTasks();

				if (workers[i].IsIdle())
					GenerateNextChunk(false, i);
			}
		}

		public void ResetChunkLoader()
		{
			loadOrder.Reset();
		}

		private void GenerateNextChunk(bool fast, int worker)
		{
			if (!loadOrder.HasNext())
				return;

			GetPlayerChunk();

			int chunkX, chunkY, chunkZ;
			for (int i = 0; i < 10; i++)
			{
				ChunkOffset offset = loadOrder.NextPosition();

				chunkX = offset.x + lastChunkX;
				chunkY = offset.y + lastChunkY;
				chunkZ = offset.z + lastChunkZ;

				if (forceRegenerateChunks || world.GetChunkByCoords(chunkX, chunkY, chunkZ, false) == null)
				{
					genTasks[worker].SetupChunk(chunkX, chunkY, chunkZ);
					if (fast)
					{
						genTasks[worker].PrepareTask();
						genTasks[worker].RunTask();
						genTasks[worker].FinishTask();
					}
					else
						workers[worker].AddTaskToQueue(genTasks[worker]);

					break;
				}
			}
		}
	}
}
