namespace WraithavenGames.Bones3
{
	public struct BlockLocation
	{
		public int x;
		public int y;
		public int z;

		public BlockLocation(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public override bool Equals(object obj)
		{
			return obj is BlockLocation && this == (BlockLocation)obj;
		}

		public override int GetHashCode()
		{
			int i = 0;

			i ^= x;
			i ^= y;
			i ^= z;

			return i;
		}

		public static bool operator ==(BlockLocation a, BlockLocation b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		public static bool operator !=(BlockLocation a, BlockLocation b)
		{
			return !(a == b);
		}
	}
}
