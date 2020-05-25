using Bones3Rebuilt;
using Bones3Rebuilt.World;
using NUnit.Framework;

namespace Test
{
    public class WorldTest
    {
        [Test]
        public void CreateWorld()
        {
            var chunkSize = new GridSize(4);
            var world = new World(chunkSize);

            Assert.AreEqual(chunkSize, world.ChunkSize);
        }

        [Test]
        public void GetChunk_DoesntExist()
        {
            var chunkSize = new GridSize(4);
            var world = new World(chunkSize);

            var chunkPosition = new ChunkPosition(15, 129, 63);
            var chunk = world.GetChunk(chunkPosition);
            Assert.IsNull(chunk);
            Assert.IsFalse(world.DoesChunkExist(chunkPosition));
        }

        [Test]
        public void GetChunk_Create()
        {
            var chunkSize = new GridSize(4);
            var world = new World(chunkSize);

            var chunkPosition = new ChunkPosition(15, 129, 63);
            var chunk = world.CreateChunk(chunkPosition);

            Assert.IsNotNull(chunk);
            Assert.AreEqual(chunkPosition, chunk.Position);
            Assert.AreEqual(chunkSize, chunk.Size);
            Assert.IsTrue(world.DoesChunkExist(chunkPosition));
        }

        [Test]
        public void GetChunk_AlreadyExists()
        {
            var chunkSize = new GridSize(4);
            var world = new World(chunkSize);

            var chunkPosition = new ChunkPosition(17, -1000, 8);
            var chunk1 = world.CreateChunk(chunkPosition);
            var chunk2 = world.CreateChunk(chunkPosition);

            Assert.AreSame(chunk1, chunk2);
            Assert.IsTrue(world.DoesChunkExist(chunkPosition));
        }

        [Test]
        public void DestroyChunk()
        {
            var chunkSize = new GridSize(4);
            var world = new World(chunkSize);

            var chunkPosition = new ChunkPosition(5, 19, -3);
            world.CreateChunk(chunkPosition);
            world.DestroyChunk(chunkPosition);

            Assert.IsNull(world.GetChunk(chunkPosition));
            Assert.IsFalse(world.DoesChunkExist(chunkPosition));
        }
    }
}
