using NUnit.Framework;
using Bones3Rebuilt;
using Moq;

namespace Test
{
    public class ChunkPropertiesTest
    {
        [Test]
        public void AnalyzeChunk()
        {
            var gridSize = new GridSize(4);
            var chunkPosition = new ChunkPosition(0, 0, 0);
            var chunk = new Chunk(gridSize, chunkPosition);

            chunk.SetBlockID(new BlockPosition(1, 1, 1), 1);
            chunk.SetBlockID(new BlockPosition(2, 2, 2), 2);
            chunk.SetBlockID(new BlockPosition(3, 3, 3), 3);

            var world = new Mock<IBlockContainerProvider>();
            world.Setup(w => w.GetContainer(chunkPosition, It.IsAny<bool>())).Returns(chunk);
            world.Setup(w => w.ContainerSize).Returns(gridSize);

            var blockList = new BlockTypeList();
            blockList.AddBlockType(new BlockBuilder(2).Build());
            blockList.AddBlockType(new BlockBuilder(3).Build());

            var props = new ChunkProperties(world.Object, chunkPosition, blockList);

            Assert.AreEqual(blockList.GetBlockType(0), props.GetBlock(new BlockPosition(0, 0, 0)));
            Assert.AreEqual(blockList.GetBlockType(1), props.GetBlock(new BlockPosition(1, 1, 1)));
            Assert.AreEqual(blockList.GetBlockType(2), props.GetBlock(new BlockPosition(2, 2, 2)));
            Assert.AreEqual(blockList.GetBlockType(3), props.GetBlock(new BlockPosition(3, 3, 3)));
        }

        [Test]
        public void AnalyzeChunkNeighbors()
        {
            var gridSize = new GridSize(4);
            var chunkPosition = new ChunkPosition(0, 0, 0);
            var chunk = new Chunk(gridSize, chunkPosition);

            var neighborPosition = new ChunkPosition(0, 1, 0);
            var neighbor = new Chunk(gridSize, neighborPosition);
            neighbor.SetBlockID(new BlockPosition(0, 0, 0), 1);

            var world = new Mock<IBlockContainerProvider>();
            world.Setup(w => w.GetContainer(chunkPosition, It.IsAny<bool>())).Returns(chunk);
            world.Setup(w => w.GetContainer(neighborPosition, It.IsAny<bool>())).Returns(neighbor);
            world.Setup(w => w.ContainerSize).Returns(gridSize);

            var blockList = new BlockTypeList();

            var props = new ChunkProperties(world.Object, chunkPosition, blockList);

            Assert.AreEqual(blockList.GetBlockType(1), props.GetNextBlock(new BlockPosition(0, 15, 0), 2));
            Assert.AreEqual(blockList.GetBlockType(0), props.GetNextBlock(new BlockPosition(1, 15, 1), 2));
        }

        [Test]
        public void GetNeighbor()
        {
            var gridSize = new GridSize(4);
            var chunkPosition = new ChunkPosition(0, 0, 0);
            var chunk = new Chunk(gridSize, chunkPosition);

            chunk.SetBlockID(new BlockPosition(5, 5, 5), 1);

            var world = new Mock<IBlockContainerProvider>();
            world.Setup(w => w.GetContainer(chunkPosition, It.IsAny<bool>())).Returns(chunk);
            world.Setup(w => w.ContainerSize).Returns(gridSize);

            var blockList = new BlockTypeList();

            var props = new ChunkProperties(world.Object, chunkPosition, blockList);

            Assert.AreEqual(blockList.GetBlockType(1), props.GetNextBlock(new BlockPosition(5, 4, 5), 2));
            Assert.AreEqual(blockList.GetBlockType(0), props.GetNextBlock(new BlockPosition(5, 5, 5), 2));
        }
    }
}
