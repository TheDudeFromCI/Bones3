using Bones3Rebuilt;

using NUnit.Framework;

namespace Test
{
    public class FloodFillTest
    {
        [Test]
        public void AlwaysReturnsInput()
        {
            var floodFill = new FloodFill(7);

            Assert.AreEqual(7, floodFill.GetBlockID(new BlockPosition(0, 0, 0)));
            Assert.AreEqual(7, floodFill.GetBlockID(new BlockPosition(-100, 31235, 1912)));
            Assert.AreEqual(7, floodFill.GetBlockID(new BlockPosition(129, 0, 100000)));
        }
    }
}
