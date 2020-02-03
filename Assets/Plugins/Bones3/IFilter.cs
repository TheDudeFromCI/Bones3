using UnityEngine;
using WraithavenGames.Bones3;

namespace WraithavenGames.Bones3.Filter
{
	public interface IFilter
	{
		void InitalizeFilter(BlockLocation block1, BlockLocation block2, Material newBlock);
		bool CanPlaceBlock(Material oldBlock, BlockLocation loc);
	}
}
