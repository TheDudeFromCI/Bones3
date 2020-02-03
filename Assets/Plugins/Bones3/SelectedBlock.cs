namespace WraithavenGames.Bones3
{
	public struct SelectedBlock
	{
		public int xInside;
		public int yInside;
		public int zInside;
		public int xOn;
		public int yOn;
		public int zOn;
		public bool hasSelectedBlock;

		public override bool Equals(object obj)
		{
			return obj is SelectedBlock && this == (SelectedBlock)obj;
		}

		public override int GetHashCode()
		{
			int i = 0;
			i ^= xInside.GetHashCode();
			i ^= yInside.GetHashCode();
			i ^= zInside.GetHashCode();
			i ^= xOn.GetHashCode();
			i ^= yOn.GetHashCode();
			i ^= zOn.GetHashCode();

			if (hasSelectedBlock)
				i = ~i;

			return i;
		}

		public static bool operator ==(SelectedBlock a, SelectedBlock b)
		{
			return a.hasSelectedBlock == b.hasSelectedBlock
				&& a.xInside == b.xInside
				&& a.yInside == b.yInside
				&& a.zInside == b.zInside
				&& a.xOn == b.xOn
				&& a.yOn == b.yOn
				&& a.zOn == b.zOn;
		}

		public static bool operator !=(SelectedBlock a, SelectedBlock b)
		{
			return !(a == b);
		}
	}
}
