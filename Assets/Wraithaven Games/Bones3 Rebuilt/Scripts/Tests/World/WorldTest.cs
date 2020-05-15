using Bones3Rebuilt;

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

            Assert.AreEqual(chunkSize, world.ContainerSize);
        }

        [Test]
        public void GetChunk_Create()
        {
            var chunkSize = new GridSize(4);
            var world = new World(chunkSize);

            var chunkPosition = new ChunkPosition(15, 129, 63);
            var chunk = world.GetContainer(chunkPosition, true);

            Assert.IsNotNull(chunk);
            Assert.AreEqual(chunkPosition, chunk.Position);
            Assert.AreEqual(chunkSize, chunk.Size);
        }

        [Test]
        public void ChunkCreated_TriggersEvent()
        {
            var chunkSize = new GridSize(4);
            var world = new World(chunkSize);

            int calls = 0;
            world.OnBlockContainerCreated += e =>
            {
                calls++;

                Assert.IsNotNull(e.BlockContainer);
                Assert.AreEqual(world, e.ContainerProvider);
            };

            var chunkPosition = new ChunkPosition(15, 129, 63);
            world.GetContainer(chunkPosition, true);

            Assert.AreEqual(1, calls);
        }

        [Test]
        public void GetExistingChunk_DontTriggerEvent()
        {
            var chunkSize = new GridSize(4);
            var world = new World(chunkSize);

            int calls = 0;
            world.OnBlockContainerCreated += e => calls++;

            var chunkPosition = new ChunkPosition(0, -1, 0);
            world.GetContainer(chunkPosition, true);
            world.GetContainer(chunkPosition, true);
            world.GetContainer(chunkPosition, true);
            world.GetContainer(chunkPosition, true);

            Assert.AreEqual(1, calls);
        }

        [Test]
        public void DestroyChunk()
        {
            var chunkSize = new GridSize(4);
            var world = new World(chunkSize);

            var chunkPosition = new ChunkPosition(15, 129, 63);
            world.GetContainer(chunkPosition, true);
            world.DestroyContainer(chunkPosition);

            Assert.IsNull(world.GetContainer(chunkPosition, false));
        }

        [Test]
        public void DestroyChunk_EventTriggered()
        {
            var chunkSize = new GridSize(4);
            var world = new World(chunkSize);

            var chunkPosition = new ChunkPosition(5, 29, 6);
            var chunk = world.GetContainer(chunkPosition, true);

            int calls = 0;
            world.OnBlockContainerDestroyed += e =>
            {
                calls++;

                Assert.AreEqual(chunk, e.BlockContainer);
                Assert.AreEqual(world, e.ContainerProvider);
            };
            world.DestroyContainer(chunkPosition);

            Assert.AreEqual(1, calls);
        }

        [Test]
        public void DestroyChunk_DoesntExist_DontTriggerEvent()
        {
            var chunkSize = new GridSize(4);
            var world = new World(chunkSize);

            int calls = 0;
            world.OnBlockContainerDestroyed += e => calls++;

            var chunkPosition = new ChunkPosition(5, 29, 6);
            world.DestroyContainer(chunkPosition);

            Assert.AreEqual(0, calls);
        }
    }
}
