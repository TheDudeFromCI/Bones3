namespace WraithavenGames.Bones3.Terrain
{
	public interface ITerrainWorkerTask
	{
		void PrepareTask();
		void RunTask();
		void FinishTask();
	}
}
