using Bones3Rebuilt;

using Moq;

using NUnit.Framework;

namespace Test
{
    public class VisualRemeshTaskTest
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
            world.Setup(w => w.ContainerSize).Returns(chunkSize);

            BlockTypeList blockList = new BlockTypeList();
            blockList.AddBlockType(new BlockBuilder(2)
                .Name("Grass")
                .Solid(true)
                .Visible(true)
                .Build());

            var props = new ChunkProperties(world.Object, chunkPosition, blockList);

            var task = new VisualRemeshTask(props, 0);
            task.Finish();

            Assert.AreEqual(24, task.Mesh.Vertices.Count);
            Assert.AreEqual(24, task.Mesh.Normals.Count);
            Assert.AreEqual(24, task.Mesh.UVs.Count);
        }

        [Test]
        public void RespectNeighborChunks()
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
            world.Setup(w => w.ContainerSize).Returns(chunkSize);

            BlockTypeList blockList = new BlockTypeList();
            blockList.AddBlockType(new BlockBuilder(2)
                .Name("Grass")
                .Solid(true)
                .Visible(true)
                .Build());

            var props = new ChunkProperties(world.Object, chunkPosition, blockList);

            var task = new VisualRemeshTask(props, 0);
            task.Finish();

            Assert.AreEqual(20, task.Mesh.Vertices.Count);
            Assert.AreEqual(20, task.Mesh.Normals.Count);
            Assert.AreEqual(20, task.Mesh.UVs.Count);
        }

        [Test]
        public void BlockFace_DifferentAtlas_IgnoreQuads()
        {
            var chunkSize = new GridSize(3);
            var chunkPosition = new ChunkPosition(0, 0, 0);
            var chunk = new Chunk(chunkSize, chunkPosition);

            chunk.SetBlockID(new BlockPosition(0, 0, 0), 2);

            var world = new Mock<IBlockContainerProvider>();
            world.Setup(w => w.GetContainer(chunkPosition, It.IsAny<bool>())).Returns(chunk);
            world.Setup(w => w.ContainerSize).Returns(chunkSize);

            BlockTypeList blockList = new BlockTypeList();
            blockList.AddBlockType(new BlockBuilder(2)
                .Name("Grass")
                .Solid(true)
                .Visible(true)
                .TextureAtlas(0, 1)
                .TextureAtlas(1, 1)
                .Build());

            var props = new ChunkProperties(world.Object, chunkPosition, blockList);

            var task = new VisualRemeshTask(props, 0);
            task.Finish();

            Assert.AreEqual(16, task.Mesh.Vertices.Count);
        }
    }
}
