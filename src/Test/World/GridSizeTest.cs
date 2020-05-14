using Bones3Rebuilt;

using NUnit.Framework;

namespace Test
{
    public class GridSizeTest
    {
        [Test]
        public void GridSizeProperties()
        {
            var grid = new GridSize(4);

            Assert.AreEqual(4, grid.IntBits);
            Assert.AreEqual(16, grid.Value);
            Assert.AreEqual(15, grid.Mask);
        }

        [Test]
        public void GridSize_NegativeBits()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new GridSize(-1));
        }
    }
}
