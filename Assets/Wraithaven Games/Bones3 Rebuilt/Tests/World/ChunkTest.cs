using Bones3Rebuilt;
using Bones3Rebuilt.World;
using NUnit.Framework;

namespace Test
{
    public class ChunkTest
    {
        public GridSize Size;
        public ChunkPosition Position;
        public Chunk Chunk;

        [SetUp]
        public void Init()
        {
            Size = new GridSize(4);
            Position = new ChunkPosition(15, 30, -77);

            var world = new World(Size);

            Chunk = world.CreateChunk(Position);
        }

        [Test]
        public void CreateChunk()
        {
            Assert.AreEqual(Size, Chunk.Size);
            Assert.AreEqual(Position, Chunk.Position);
        }

        [Test]
        public void SetBlock()
        {
            var blockPosition = new BlockPosition(2, 3, 1);

            Chunk.SetBlockID(blockPosition, 4);

            Assert.AreEqual(4, Chunk.GetBlockID(blockPosition));
        }

        [Test]
        public void SetBlock_OutOfBounds()
        {
            var blockPosition = new BlockPosition(-2, 0, 0);
            Assert.Throws<System.ArgumentOutOfRangeException>(() => Chunk.SetBlockID(blockPosition, 3));
        }
    }
}
