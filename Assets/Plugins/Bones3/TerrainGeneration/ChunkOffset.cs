using System;
using UnityEngine;

namespace WraithavenGames.Bones3.Terrain
{
	public class ChunkOffset : IComparable
	{
		public int x;
		public int y;
		public int z;

		private float val;

		public ChunkOffset(LoadOrder loadOrder, int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;

			FindValue(loadOrder);
		}

		private void FindValue(LoadOrder loadOrder)
		{
			switch(loadOrder)
			{
				case LoadOrder.FLAT_SPHERE:
				{
					float y2 = y * 2f;
					val = x * x + y2 * y2 + z * z;
					break;
				}

				case LoadOrder.SPHERE:
				{
					val = x * x + y * y + z * z;
					break;
				}

				case LoadOrder.CYLINDER:
				{
					val = x * x + z * z + y * 0.0001f;
					break;
				}

				case LoadOrder.CUBE_FILL:
				{
					val = y * 1000 * 1000 + z * 1000 + x;
					break;
				}
			}
		}

		public int CompareTo(System.Object x)
		{
			ChunkOffset other = (ChunkOffset) x;

			if (other.val == val)
				return 0;
			return val > other.val ? 1 : -1;
		}
	}
}
