using UnityEngine;
using WraithavenGames.Bones3.Terrain;

namespace WraithavenGames.Bones3.Demo
{
	public class FlatWorld : WorldPopulator
	{
		public Material material;

		public override void GenerateChunk(UngeneratedChunk chunk)
		{
			if (chunk.chunkY < 0)
				chunk.Fill(material);
		}
	}
}
