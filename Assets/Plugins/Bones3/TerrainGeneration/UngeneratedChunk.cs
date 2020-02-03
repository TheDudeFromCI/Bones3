using UnityEngine;
using WraithavenGames.Bones3;
using WraithavenGames.Bones3.BlockProperties;

namespace WraithavenGames.Bones3.Terrain
{
	public class UngeneratedChunk
	{
		public int chunkX;
		public int chunkY;
		public int chunkZ;

		private Material[] materials;
		private ushort[] blocks;
		private byte[] blockStates;
		private int blockCount;

		public UngeneratedChunk()
		{
			materials = new Material[16 * 16 * 16];
			blocks = new ushort[16 * 16 * 16];
			blockStates = new byte[16 * 16 * 16];
		}

		public void Clear()
		{
			Fill(null);
		}

		public void Fill(Material m)
		{
			for (int i = 0; i < materials.Length; i++)
				materials[i] = m;
		}

		public Material GetMaterial(int x, int y, int z)
		{
			return materials[x * 16 * 16 + y * 16 + z];
		}

		public void SetMaterial(int x, int y, int z, Material m)
		{
			materials[x * 16 * 16 + y * 16 + z] = m;
		}

		public void Compile(BlockWorld world)
		{
			blockCount = 0;

			MaterialBlock matProps;
			for (int i = 0; i < materials.Length; i++)
			{
				if (materials[i] == null)
				{
					blocks[i] = 0;
					blockStates[i] = 0;
					continue;
				}

				matProps = world.BlockTypes.GetMaterialProperties(materials[i]);

				blockCount++;
				blocks[i] = matProps.Id;
				blockStates[i] = matProps.BlockState;
			}
		}

		public int GetBlockCount()
		{
			return blockCount;
		}

		public ushort[] GetBlocks()
		{
			return blocks;
		}

		public byte[] GetBlockStates()
		{
			return blockStates;
		}
	}
}
