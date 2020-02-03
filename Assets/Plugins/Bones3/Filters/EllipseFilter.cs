using UnityEngine;
using WraithavenGames.Bones3;

namespace WraithavenGames.Bones3.Filter
{
	public class EllipseFilter : IFilter
	{
		private static EllipseFilter _instance;
		public static EllipseFilter Instance
		{
			get
			{
				if (_instance == null)
					_instance = new EllipseFilter();
				return _instance;
			}
		}

		private Vector3 center;
		private Vector3 size;

		public void InitalizeFilter(BlockLocation block1, BlockLocation block2, Material newBlock)
		{
			center = new Vector3();
			center.x = block1.x + block2.x;
			center.y = block1.y + block2.y;
			center.z = block1.z + block2.z;
			center /= 2f;

			size = new Vector3();
			size.x = Mathf.Max(block1.x, block2.x) - Mathf.Min(block1.x, block2.x) + 1;
			size.y = Mathf.Max(block1.y, block2.y) - Mathf.Min(block1.y, block2.y) + 1;
			size.z = Mathf.Max(block1.z, block2.z) - Mathf.Min(block1.z, block2.z) + 1;
			size /= 2f;
		}

		private Vector3 Div(Vector3 a, Vector3 b)
		{
			Vector3 c;

			c.x = a.x / b.x;
			c.y = a.y / b.y;
			c.z = a.z / b.z;

			return c;
		}

		public bool CanPlaceBlock(Material oldBlock, BlockLocation loc)
		{
			Vector3 pos = new Vector3(loc.x, loc.y, loc.z);
			Vector3 delta = pos - center;
			Vector3 nd = Div(delta, size);

			return nd.magnitude <= 1f;
		}
	}
}
