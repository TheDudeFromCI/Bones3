using NUnit.Framework;
using Bones3Rebuilt.Remeshing;
using Bones3Rebuilt;
using Moq;

namespace Test
{
    public class ChunkPropertiesTest
    {
        [Test]
        public void SetBlock_GetBlock()
        {
            // Arrange
            var blockType1 = new Mock<IMeshBlockDetails>();
            var blockType2 = new Mock<IMeshBlockDetails>();
            var blockType3 = new Mock<IMeshBlockDetails>();
            var blockPos1 = new BlockPosition(1, 1, 4);
            var blockPos2 = new BlockPosition(2, 2, 3);
            var blockPos3 = new BlockPosition(3, 3, 2);
            var blockPos4 = new BlockPosition(4, 4, 1);

            var chunkSize = new GridSize(3);
            var chunkPos = new ChunkPosition(1, 3, 6);
            var props = new ChunkProperties();

            // Act
            props.Reset(chunkPos, chunkSize);
            props.SetBlock(blockPos1, blockType1.Object);
            props.SetBlock(blockPos2, blockType2.Object);
            props.SetBlock(blockPos3, blockType3.Object);

            // Assert
            Assert.AreEqual(blockType1.Object, props.GetBlock(blockPos1));
            Assert.AreEqual(blockType2.Object, props.GetBlock(blockPos2));
            Assert.AreEqual(blockType3.Object, props.GetBlock(blockPos3));
            Assert.AreEqual(null, props.GetBlock(blockPos4));
        }

        [Test]
        public void SetBlock_ResetChunk_GetBlock_ReturnsNull()
        {
            // Arrange
            var blockType = new Mock<IMeshBlockDetails>();
            var blockPos = new BlockPosition(1, 1, 4);

            var chunkSize = new GridSize(3);
            var chunkPos1 = new ChunkPosition(1, 3, 6);
            var chunkPos2 = new ChunkPosition(17, -123, 12999);
            var props = new ChunkProperties();

            // Act
            props.Reset(chunkPos1, chunkSize);
            props.SetBlock(blockPos, blockType.Object);
            props.Reset(chunkPos2, chunkSize);

            // Assert
            Assert.AreEqual(null, props.GetBlock(blockPos));
        }
    }
}
