namespace WraithavenGames.Bones3.Terrain
{
	public class GenerateChunkTask : ITerrainWorkerTask
	{
		private WorldPopulator populator;
		private BlockWorld world;
		private UngeneratedChunk chunk;

		public GenerateChunkTask(WorldPopulator populator, BlockWorld world)
		{
			this.populator = populator;
			this.world = world;
			chunk = new UngeneratedChunk();

			world.EnsureFullyGenerated();
		}

		public void SetupChunk(int chunkX, int chunkY, int chunkZ)
		{
			chunk.chunkX = chunkX;
			chunk.chunkY = chunkY;
			chunk.chunkZ = chunkZ;
		}

		public void PrepareTask()
		{
			// Nothing to do
		}

		public void RunTask()
		{
			chunk.Clear();
			populator.GenerateChunk(chunk);
			chunk.Compile(world);
		}

		public void FinishTask()
		{
			world.SetChunk(chunk);
		}
	}
}
