using NUnit.Framework;
using Bones3Rebuilt.Remeshing;
using Bones3Rebuilt.Remeshing.Voxel;
using Moq;

namespace Test
{
    public class CollisionRemeshTaskTest : VoxelTest
    {
        [Test]
        public void TwoCubes()
        {
            // Arrange
            ChunkProperties.SetBlock(Pos(0, 0, 0), NormalBlock);
            ChunkProperties.SetBlock(Pos(0, 1, 0), NormalBlock);

            // Act
            var task = new CollisionRemeshTask(ChunkProperties);
            var mesh = task.Finish();

            // Assert
            Assert.AreEqual(24, mesh.Vertices.Count);
            Assert.AreEqual(24, mesh.Normals.Count);
            Assert.AreEqual(0, mesh.UVs.Count);
            Assert.AreEqual(36, mesh.Triangles.Count);
        }

        [Test]
        public void IgnoreNeighborChunks()
        {
            // Arrange
            ChunkProperties.SetBlock(Pos(-1, 0, 0), NormalBlock);
            ChunkProperties.SetBlock(Pos(0, 0, 0), NormalBlock);

            // Act
            var task = new CollisionRemeshTask(ChunkProperties);
            var mesh = task.Finish();

            // Assert
            Assert.AreEqual(24, mesh.Vertices.Count);
            Assert.AreEqual(24, mesh.Normals.Count);
            Assert.AreEqual(0, mesh.UVs.Count);
            Assert.AreEqual(36, mesh.Triangles.Count);
        }
    }
}