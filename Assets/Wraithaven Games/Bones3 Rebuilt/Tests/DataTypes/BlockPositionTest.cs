using NUnit.Framework;
using Bones3Rebuilt;

namespace Test
{
    public class BlockPositionTest
    {
        [Test]
        public void AddPositions()
        {
            var a = new BlockPosition(5, 10, 15);
            var b = new BlockPosition(2, 3, 4);

            Assert.AreEqual(new BlockPosition(7, 13, 19), a + b);
        }

        [Test]
        public void ScalePosition()
        {
            var a = new BlockPosition(10, 20, 40);
            var b = 2;

            Assert.AreEqual(new BlockPosition(20, 40, 80), a * b);
        }

        [Test]
        public void MaskPosition()
        {
            var a = new BlockPosition(5, 17, -5);
            var b = 15;

            Assert.AreEqual(new BlockPosition(5, 1, 11), a & b);
        }

        [Test]
        public void ToChunkPos()
        {
            var pos = new BlockPosition(66, 7, -10);
            var gridSize = new GridSize(4);

            Assert.AreEqual(new ChunkPosition(4, 0, -1), pos.ToChunkPosition(gridSize));
        }
    }
}