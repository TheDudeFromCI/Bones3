using UnityEngine;
using System;
using System.Collections;

namespace WraithavenGames.Bones3.Terrain
{
	public class ChunkLoadOrder : MonoBehaviour
	{
		public LoadOrder loadOrder;

		private int bufferRadius = 0;
		private int bufferHeight = 0;
		private ChunkOffset[] chunkOffsets = new ChunkOffset[0];
		private int iteratorPos = 0;
		private LoadOrder lastLoadOrder;

		public void UpdateBufferRadius(int radius)
		{
			UpdateBufferRadius(radius, radius);
		}

		public void UpdateBufferRadius(int radius, int height)
		{
			iteratorPos = 0;
			bufferRadius = radius;
			bufferHeight = height;
			lastLoadOrder = loadOrder;

			int diameter = radius * 2 + 1;
			int diameterUp = height * 2 + 1;
			chunkOffsets = new ChunkOffset[diameter * diameterUp * diameter];

			int x, y, z;
			int i = 0;
			for (x = -radius; x <= radius; x++)
				for (y = -height; y <= height; y++)
					for (z = -radius; z <= radius; z++)
						chunkOffsets[i++] = new ChunkOffset(loadOrder, x, y, z);

			Array.Sort(chunkOffsets);
		}

		private void Update()
		{
			if (loadOrder != lastLoadOrder)
				UpdateBufferRadius(bufferRadius, bufferHeight);
		}

		public int GetBufferRadius()
		{
			return bufferRadius;
		}

		public int GetBufferHeight()
		{
			return bufferHeight;
		}

		public int GetIteratorPos()
		{
			return iteratorPos;
		}

		public void Reset()
		{
			iteratorPos = 0;
		}

		public ChunkOffset NextPosition()
		{
			return chunkOffsets[iteratorPos++];
		}

		public bool HasNext()
		{
			return iteratorPos < chunkOffsets.Length - 1;
		}
	}
}
