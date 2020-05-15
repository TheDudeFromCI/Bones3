using Bones3Rebuilt;

using NUnit.Framework;

namespace Test
{
    public class ChunkTest
    {
        [Test]
        public void CreateChunk()
        {
            var chunkSize = new GridSize(4);
            var chunkPosition = new ChunkPosition(15, 30, 77);
            var chunk = new Chunk(chunkSize, chunkPosition);

            Assert.AreEqual(chunkSize, chunk.Size);
            Assert.AreEqual(chunkPosition, chunk.Position);
        }

        [Test]
        public void SetBlock()
        {
            var chunkSize = new GridSize(4);
            var chunkPosition = new ChunkPosition(15, 30, 77);
            var chunk = new Chunk(chunkSize, chunkPosition);

            var blockPosition = new BlockPosition(2, 3, 1);
            chunk.SetBlockID(blockPosition, 4);
            Assert.AreEqual(4, chunk.GetBlockID(blockPosition));
        }

        [Test]
        public void SetBlock_TriggerModificationEvent()
        {
            var chunkSize = new GridSize(4);
            var chunkPosition = new ChunkPosition(1, 2, 3);
            var chunk = new Chunk(chunkSize, chunkPosition);

            var blockPos = new BlockPosition(3, 0, 2);

            int calls = 0;
            chunk.OnBlockContainerModified += e =>
            {
                calls++;

                Assert.AreEqual(chunk, e.BlockContainer);
                Assert.AreEqual(blockPos, e.BlockPos);
                Assert.AreEqual(0, e.OldBlock);
                Assert.AreEqual(7, e.NewBlock);
            };

            chunk.SetBlockID(blockPos, 7);
            Assert.AreEqual(1, calls);
        }
    }
}
