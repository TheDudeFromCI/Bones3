using NUnit.Framework;
using Bones3Rebuilt;
using Moq;

namespace Test
{
    public class VoxelChunkMesherTest
    {
        public class VoxelChunkMesherStubTest : VoxelChunkMesher
        {
            public VoxelChunkMesherStubTest(IChunkProperties properties, bool wieldVertices, bool enableUVs) :
                base(properties, wieldVertices, enableUVs)
            { }

            public override bool CanPlaceQuad(IChunkProperties properties, BlockPosition pos, int side) =>
                properties.GetBlock(pos).IsVisible && !properties.GetNextBlock(pos, side).IsVisible;
        }

        [Test]
        public void MeshChunk_Empty()
        {
            var chunkSize = new GridSize(4);
            var chunkPosition = new ChunkPosition(0, 0, 0);
            var chunk = new Chunk(chunkSize, chunkPosition);

            var world = new Mock<IBlockContainerProvider>();
            world.Setup(w => w.GetContainer(chunkPosition, It.IsAny<bool>())).Returns(chunk);

            var blockList = new BlockTypeList();

            var properties = new ChunkProperties(world.Object, chunkPosition, blockList);

            var mesher = new VoxelChunkMesherStubTest(properties, false, true);
            mesher.Finish();

            Assert.AreEqual(0, mesher.Mesh.Vertices.Count);
        }

        [Test]
        public void MeshChunk_OnlyAir()
        {
            var chunkSize = new GridSize(4);
            var chunkPosition = new ChunkPosition(0, 0, 0);
            var chunk = new Chunk(chunkSize, chunkPosition);

            chunk.SetBlockID(default, BlockType.AIR);

            var world = new Mock<IBlockContainerProvider>();
            world.Setup(w => w.GetContainer(chunkPosition, It.IsAny<bool>())).Returns(chunk);

            var blockList = new BlockTypeList();

            var properties = new ChunkProperties(world.Object, chunkPosition, blockList);

            var mesher = new VoxelChunkMesherStubTest(properties, false, true);
            mesher.Finish();

            Assert.AreEqual(0, mesher.Mesh.Vertices.Count);
        }

        [Test]
        public void MeshChunk_PlaneOfGrass()
        {
            var chunkSize = new GridSize(4);
            var chunkPosition = new ChunkPosition(0, 0, 0);
            var chunk = new Chunk(chunkSize, chunkPosition);

            for (int x = 0; x < 16; x++)
                for (int z = 0; z < 16; z++)
                    chunk.SetBlockID(new BlockPosition(x, 0, z), 2);

            var world = new Mock<IBlockContainerProvider>();
            world.Setup(w => w.GetContainer(chunkPosition, It.IsAny<bool>())).Returns(chunk);
            world.Setup(w => w.ContainerSize).Returns(chunkSize);

            var blockList = new BlockTypeList();
            blockList.AddBlockType(new BlockBuilder(blockList.NextBlockID)
                .Name("Grass")
                .Build());

            var properties = new ChunkProperties(world.Object, chunkPosition, blockList);

            var mesher = new VoxelChunkMesherStubTest(properties, false, true);
            mesher.Finish();

            Assert.AreEqual(24, mesher.Mesh.Vertices.Count);
            Assert.AreEqual(24, mesher.Mesh.UVs.Count);
        }

        [Test]
        public void MeshChunk_WieldVertices()
        {
            var chunkSize = new GridSize(4);
            var chunkPosition = new ChunkPosition(0, 0, 0);
            var chunk = new Chunk(chunkSize, chunkPosition);

            for (int x = 0; x < 16; x++)
                for (int z = 0; z < 16; z++)
                    chunk.SetBlockID(new BlockPosition(x, 0, z), 2);

            for (int x = 0; x < 8; x++)
                chunk.SetBlockID(new BlockPosition(x, 0, 0), 1);

            var world = new Mock<IBlockContainerProvider>();
            world.Setup(w => w.GetContainer(chunkPosition, It.IsAny<bool>())).Returns(chunk);
            world.Setup(w => w.ContainerSize).Returns(chunkSize);

            var blockList = new BlockTypeList();
            blockList.AddBlockType(new BlockBuilder(blockList.NextBlockID)
                .Name("Grass")
                .Build());

            var properties = new ChunkProperties(world.Object, chunkPosition, blockList);

            var mesher = new VoxelChunkMesherStubTest(properties, true, false);
            mesher.Finish();

            Assert.AreEqual(38, mesher.Mesh.Vertices.Count);
            Assert.AreEqual(0, mesher.Mesh.UVs.Count);
        }
    }
}