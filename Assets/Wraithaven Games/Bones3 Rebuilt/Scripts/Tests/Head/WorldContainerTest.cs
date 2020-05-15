using Bones3Rebuilt;

using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace Test
{
    public class WorldContainerTest
    {
        public Mock<IBlockContainerProvider> World;
        public Mock<IRemeshHandler> RemeshHandler;
        public Mock<IBlockTypeList> BlockList;
        public Mock<IWorldDatabase> Database;
        public WorldContainer WorldContainer;

        [SetUp]
        public void Init()
        {
            World = new Mock<IBlockContainerProvider>();
            RemeshHandler = new Mock<IRemeshHandler>();
            BlockList = new Mock<IBlockTypeList>();
            Database = new Mock<IWorldDatabase>();

            World.Setup(w => w.ContainerSize).Returns(new GridSize(4));

            WorldContainer = new WorldContainer(World.Object,
                RemeshHandler.Object, BlockList.Object, Database.Object);
        }

        [Test]
        public void CreateWorldContainer()
        {
            Assert.AreEqual(World.Object, WorldContainer.BlockContainerProvider);
            Assert.AreEqual(RemeshHandler.Object, WorldContainer.RemeshHandler);
            Assert.AreEqual(BlockList.Object, WorldContainer.BlockList);
            Assert.AreEqual(Database.Object, WorldContainer.Database);
        }

        [Test]
        public void SetBlock()
        {
            var blockPosition = new BlockPosition(3, 6, 19);
            var blockType = new BlockBuilder(2).Build();

            var chunkPosition = new ChunkPosition(0, 0, 1);
            var chunk = new Mock<IBlockContainer>();
            World.Setup(w => w.GetContainer(chunkPosition, It.IsAny<bool>())).Returns(chunk.Object);
            RemeshHandler.Setup(r => r.RemeshChunk(It.IsAny<IChunkProperties>()))
                .Callback<IChunkProperties>(props => Assert.AreEqual(chunkPosition, props.ChunkPosition));

            WorldContainer.SetBlock(blockPosition, blockType);

            chunk.Verify(c => c.SetBlockID(new BlockPosition(3, 6, 3), 2));
            RemeshHandler.Verify(r => r.RemeshChunk(It.IsAny<IChunkProperties>()), Times.Once);
        }

        [Test]
        public void SetBlock_RemeshNeighborChunks()
        {
            var blockPosition = new BlockPosition(0, 15, 15);
            var blockType = new BlockBuilder(2).Build();

            var chunk = new Mock<IBlockContainer>();
            var chunkPosition1 = new ChunkPosition(0, 0, 0);
            var chunkPosition2 = new ChunkPosition(-1, 0, 0);
            var chunkPosition3 = new ChunkPosition(0, 1, 0);
            var chunkPosition4 = new ChunkPosition(0, 0, 1);
            World.Setup(w => w.GetContainer(chunkPosition1, It.IsAny<bool>())).Returns(chunk.Object);
            World.Setup(w => w.GetContainer(chunkPosition2, It.IsAny<bool>())).Returns(chunk.Object);
            World.Setup(w => w.GetContainer(chunkPosition3, It.IsAny<bool>())).Returns(chunk.Object);
            World.Setup(w => w.GetContainer(chunkPosition4, It.IsAny<bool>())).Returns(chunk.Object);

            WorldContainer.SetBlock(blockPosition, blockType);

            RemeshHandler.Verify(r => r.RemeshChunk(It.IsAny<IChunkProperties>()), Times.Exactly(4));
        }

        [Test]
        public void SetBlock_RemeshNeighborChunk_NeighborsDontExist()
        {
            var blockPosition = new BlockPosition(15, 0, 0);
            var blockType = new BlockBuilder(2).Build();

            var chunkPosition = new ChunkPosition(0, 0, 0);
            var chunk = new Mock<IBlockContainer>();
            World.Setup(w => w.GetContainer(chunkPosition, It.IsAny<bool>())).Returns(chunk.Object);

            WorldContainer.SetBlock(blockPosition, blockType);

            RemeshHandler.Verify(r => r.RemeshChunk(It.IsAny<IChunkProperties>()), Times.Once);
        }

        [Test]
        public void GetBlock()
        {
            var blockPosition = new BlockPosition(-4, 12, 22);
            var localBlockPosition = new BlockPosition(-4 & 15, 12, 22 & 15);
            var blockType = new BlockBuilder(2).Build();

            var chunkPosition = new ChunkPosition(-1, 0, 1);
            var chunk = new Mock<IBlockContainer>();
            World.Setup(w => w.GetContainer(chunkPosition, It.IsAny<bool>())).Returns(chunk.Object);
            chunk.Setup(c => c.GetBlockID(localBlockPosition)).Returns(2);
            BlockList.Setup(b => b.GetBlockType(2)).Returns(blockType);

            Assert.AreEqual(blockType, WorldContainer.GetBlock(blockPosition, false));
        }

        [Test]
        public void SetBlocks()
        {
            World.Setup(w => w.ContainerSize).Returns(new GridSize(2));

            var editBatch = new Mock<IEditBatch>();
            editBatch.Setup(e => e.GetBlocks()).Returns(SimpleEditBatch());

            var chunkPosition = new ChunkPosition(0, 0, 0);
            var chunk = new Mock<IBlockContainer>();
            World.Setup(w => w.GetContainer(chunkPosition, It.IsAny<bool>())).Returns(chunk.Object);

            WorldContainer.SetBlocks(editBatch.Object);

            chunk.Verify(c => c.SetBlockID(new BlockPosition(1, 1, 1), 2));
            chunk.Verify(c => c.SetBlockID(new BlockPosition(2, 2, 2), 2));
            chunk.Verify(c => c.SetBlockID(new BlockPosition(3, 3, 3), 2));
            RemeshHandler.Verify(r => r.RemeshChunk(It.IsAny<IChunkProperties>()), Times.Once);
        }

        private IEnumerable<BlockPlacement> SimpleEditBatch()
        {
            for (int i = 1; i <= 3; i++)
                yield return new BlockPlacement
                {
                    Position = new BlockPosition(i, i, i),
                    BlockID = 2,
                };
        }

        [Test]
        public void SetBlock_SameBlock_NoRemesh()
        {
            var blockPosition = new BlockPosition(5, 5, 5);
            var blockType = new BlockBuilder(0).Build();

            var chunkPosition = new ChunkPosition(0, 0, 0);
            var chunk = new Mock<IBlockContainer>();
            World.Setup(w => w.GetContainer(chunkPosition, It.IsAny<bool>())).Returns(chunk.Object);

            WorldContainer.SetBlock(blockPosition, blockType);

            RemeshHandler.Verify(r => r.RemeshChunk(It.IsAny<IChunkProperties>()), Times.Never);
        }

        [Test]
        public void GetBlock_ChunkNotLoaded()
        {
            var blockPosition = new BlockPosition(5, 5, 5);
            var blockType = new BlockBuilder(0).Build();

            BlockList.Setup(b => b.GetBlockType(0)).Returns(blockType);

            Assert.AreEqual(blockType, WorldContainer.GetBlock(blockPosition, false));
        }
    }
}
