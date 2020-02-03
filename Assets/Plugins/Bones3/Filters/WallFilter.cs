using UnityEngine;
using WraithavenGames.Bones3;

namespace WraithavenGames.Bones3.Filter
{
	public class WallFilter : IFilter
	{
		private static WallFilter _instance;
		public static WallFilter Instance
		{
			get
			{
				if (_instance == null)
					_instance = new WallFilter();
				return _instance;
			}
		}

		private BlockLocation block1;
		private BlockLocation block2;

		public void InitalizeFilter(BlockLocation block1, BlockLocation block2, Material newBlock)
		{
			this.block1 = block1;
			this.block2 = block2;
		}

		public bool CanPlaceBlock(Material oldBlock, BlockLocation loc)
		{
			return loc.x == block1.x || loc.x == block2.x
				|| loc.z == block1.z || loc.z == block2.z;
		}
	}
}
