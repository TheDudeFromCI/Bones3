using UnityEngine;

namespace WraithavenGames.Bones3.Terrain
{
	public abstract class WorldPopulator : MonoBehaviour
	{
		public abstract void GenerateChunk(UngeneratedChunk chunk);
	}
}
