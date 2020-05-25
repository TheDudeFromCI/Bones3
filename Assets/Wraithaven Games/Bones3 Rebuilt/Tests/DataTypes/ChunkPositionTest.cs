using NUnit.Framework;
using Bones3Rebuilt;

namespace Test
{
    public class ChunkPositionTest
    {
        [Test]
        public void AddPositions()
        {
            var a = new ChunkPosition(5, 10, 15);
            var b = new ChunkPosition(2, 3, 4);

            Assert.AreEqual(new ChunkPosition(7, 13, 19), a + b);
        }

        [Test]
        public void ScalePosition()
        {
            var a = new ChunkPosition(10, 20, 40);
            var b = 2;

            Assert.AreEqual(new ChunkPosition(20, 40, 80), a * b);
        }
    }
}