using NUnit.Framework;
using Bones3Rebuilt;
using Moq;

namespace Test
{
    public class CollisionRemeshTaskTest
    {
        [Test]
        public void TwoCubes()
        {
            var chunkSize = new GridSize(3);
            var chunkPosition = new ChunkPosition(0, 0, 0);
            var chunk = new Chunk(chunkSize, chunkPosition);

            chunk.SetBlockID(new BlockPosition(0, 0, 0), 2);
            chunk.SetBlockID(new BlockPosition(0, 1, 0), 2);

            var world = new Mock<IBlockContainerProvider>();
            world.Setup(w => w.GetContainer(chunkPosition, It.IsAny<bool>())).Returns(chunk);

            BlockTypeList blockList = new BlockTypeList();
            blockList.AddBlockType(new BlockBuilder(2)
                .Name("Grass")
                .Solid(true)
                .Visible(true)
                .Build());

            var props = new ChunkProperties(world.Object, chunkPosition, blockList);

            var task = new CollisionRemeshTask(props);
            task.Finish();

            Assert.AreEqual(24, task.Mesh.Vertices.Count);
            Assert.AreEqual(24, task.Mesh.Normals.Count);
            Assert.AreEqual(0, task.Mesh.UVs.Count);
        }

        [Test]
        public void IgnoreNeighborChunks()
        {
            var chunkSize = new GridSize(3);
            var chunkPosition = new ChunkPosition(0, 0, 0);
            var chunk = new Chunk(chunkSize, chunkPosition);

            var neighborPosition = new ChunkPosition(-1, 0, 0);
            var neighbor = new Chunk(chunkSize, neighborPosition);

            chunk.SetBlockID(new BlockPosition(0, 0, 0), 2);
            neighbor.SetBlockID(new BlockPosition(7, 0, 0), 2);

            var world = new Mock<IBlockContainerProvider>();
            world.Setup(w => w.GetContainer(chunkPosition, It.IsAny<bool>())).Returns(chunk);
            world.Setup(w => w.GetContainer(neighborPosition, It.IsAny<bool>())).Returns(neighbor);

            BlockTypeList blockList = new BlockTypeList();
            blockList.AddBlockType(new BlockBuilder(2)
                .Name("Grass")
                .Solid(true)
                .Visible(true)
                .Build());

            var props = new ChunkProperties(world.Object, chunkPosition, blockList);

            var task = new CollisionRemeshTask(props);
            task.Finish();

            Assert.AreEqual(24, task.Mesh.Vertices.Count);
            Assert.AreEqual(24, task.Mesh.Normals.Count);
            Assert.AreEqual(0, task.Mesh.UVs.Count);
        }
    }
}