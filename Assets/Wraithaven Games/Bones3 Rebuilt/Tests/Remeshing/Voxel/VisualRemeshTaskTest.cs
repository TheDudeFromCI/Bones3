using Bones3Rebuilt.Remeshing.Voxel;
using Bones3Rebuilt.Remeshing;

using NUnit.Framework;

namespace Test
{
    public class VisualRemeshTaskTest : VoxelTest
    {
        [Test]
        public void TwoCubes()
        {
            // Arrange
            ChunkProperties.SetBlock(Pos(0, 0, 0), NormalBlock);
            ChunkProperties.SetBlock(Pos(0, 1, 0), NormalBlock);

            // Act
            var task = new VisualRemeshTask(ChunkProperties, 0);
            var mesh = task.Finish();

            // Assert
            Assert.AreEqual(24, mesh.Vertices.Count);
            Assert.AreEqual(24, mesh.Normals.Count);
            Assert.AreEqual(24, mesh.UVs.Count);
            Assert.AreEqual(36, mesh.Triangles.Count);
        }

        [Test]
        public void RespectNeighborChunks()
        {
            // Arrange
            ChunkProperties.SetBlock(Pos(-1, 0, 0), NormalBlock);
            ChunkProperties.SetBlock(Pos(0, 0, 0), NormalBlock);

            // Act
            var task = new VisualRemeshTask(ChunkProperties, 0);
            var mesh = task.Finish();

            // Assert
            Assert.AreEqual(20, mesh.Vertices.Count);
            Assert.AreEqual(20, mesh.Normals.Count);
            Assert.AreEqual(20, mesh.UVs.Count);
            Assert.AreEqual(30, mesh.Triangles.Count);
        }

        [Test]
        public void BlockFace_DifferentAtlas_IgnoreQuads()
        {
            // Arrange
            ChunkProperties.SetBlock(Pos(0, 0, 0), NormalBlock);
            ChunkProperties.SetBlock(Pos(1, 1, 1), DifferentMaterialBlock);

            // Act
            var task = new VisualRemeshTask(ChunkProperties, 0);
            var mesh = task.Finish();

            // Assert
            Assert.AreEqual(24, mesh.Vertices.Count);
            Assert.AreEqual(24, mesh.Normals.Count);
            Assert.AreEqual(24, mesh.UVs.Count);
            Assert.AreEqual(36, mesh.Triangles.Count);
        }
    }
}
