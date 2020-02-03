namespace WraithavenGames.Bones3.Meshing
{
	public interface IChunkRemeshTask
	{
		void RemeshChunk();
		void CleanupRemesh();
		bool MatchesTask(IChunkRemeshTask other);
		void FinishTask();
	}
}
