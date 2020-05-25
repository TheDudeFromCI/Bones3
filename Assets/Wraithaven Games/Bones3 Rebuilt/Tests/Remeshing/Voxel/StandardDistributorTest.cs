using Bones3Rebuilt.Remeshing.Voxel;
using Bones3Rebuilt.Remeshing;
using Bones3Rebuilt;

using Moq;

using NUnit.Framework;
using System.Collections.Generic;

namespace Test
{
    public class StandardDistributorTest : VoxelTest
    {
        [Test]
        public void ChunkAllAir_GeneratesNoTasks()
        {
            // Arrange
            var remeshHandler = new RemeshHandler();
            remeshHandler.AddDistributor(new StandardDistributor());

            // Act
            remeshHandler.RemeshChunk(ChunkProperties);
            List<RemeshTaskStack> tasks = new List<RemeshTaskStack>();
            remeshHandler.FinishTasks(tasks);

            // Assert
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual(0, tasks[0].TaskCount);
        }

        [Test]
        public void SomeVisible_NoSolid()
        {
            // Arrange
            ChunkProperties.SetBlock(Pos(1, 1, 1), NoCollisionBlock);
            ChunkProperties.SetBlock(Pos(2, 1, 2), NoCollisionBlock);
            ChunkProperties.SetBlock(Pos(3, 1, 3), NoCollisionBlock);

            var remeshHandler = new RemeshHandler();
            remeshHandler.AddDistributor(new StandardDistributor());

            // Act
            remeshHandler.RemeshChunk(ChunkProperties);
            List<RemeshTaskStack> tasks = new List<RemeshTaskStack>();
            remeshHandler.FinishTasks(tasks);

            // Assert
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual(1, tasks[0].TaskCount);
            Assert.IsInstanceOf<VisualRemeshTask>(tasks[0].GetTask(0));
        }

        [Test]
        public void SomeSolid_NoVisible()
        {
            // Arrange
            ChunkProperties.SetBlock(Pos(1, 1, 5), InvisibleBlock);
            ChunkProperties.SetBlock(Pos(2, 2, 4), InvisibleBlock);
            ChunkProperties.SetBlock(Pos(3, 3, 3), InvisibleBlock);

            var remeshHandler = new RemeshHandler();
            remeshHandler.AddDistributor(new StandardDistributor());

            // Act
            remeshHandler.RemeshChunk(ChunkProperties);
            List<RemeshTaskStack> tasks = new List<RemeshTaskStack>();
            remeshHandler.FinishTasks(tasks);

            // Assert
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual(1, tasks[0].TaskCount);
            Assert.IsInstanceOf<CollisionRemeshTask>(tasks[0].GetTask(0));
        }

        [Test]
        public void StandardChunk()
        {
            // Arrange
            ChunkProperties.SetBlock(Pos(4, 4, 5), NormalBlock);
            ChunkProperties.SetBlock(Pos(4, 5, 5), NormalBlock);
            ChunkProperties.SetBlock(Pos(5, 5, 5), NormalBlock);

            var remeshHandler = new RemeshHandler();
            remeshHandler.AddDistributor(new StandardDistributor());

            // Act
            remeshHandler.RemeshChunk(ChunkProperties);
            List<RemeshTaskStack> tasks = new List<RemeshTaskStack>();
            remeshHandler.FinishTasks(tasks);

            // Assert
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual(2, tasks[0].TaskCount);
            Assert.IsInstanceOf<VisualRemeshTask>(tasks[0].GetTask(0));
            Assert.IsInstanceOf<CollisionRemeshTask>(tasks[0].GetTask(1));
        }

        [Test]
        public void TwoInputsMaterials_ThreeOutputTasks()
        {
            // Arrange
            ChunkProperties.SetBlock(Pos(1, 1, 1), NormalBlock);
            ChunkProperties.SetBlock(Pos(2, 2, 2), NormalBlock);
            ChunkProperties.SetBlock(Pos(3, 3, 3), NormalBlock);
            ChunkProperties.SetBlock(Pos(4, 4, 4), NormalBlock);
            ChunkProperties.SetBlock(Pos(5, 5, 5), DifferentMaterialBlock);
            ChunkProperties.SetBlock(Pos(6, 6, 6), DifferentMaterialBlock);
            ChunkProperties.SetBlock(Pos(7, 7, 7), DifferentMaterialBlock);

            var remeshHandler = new RemeshHandler();
            remeshHandler.AddDistributor(new StandardDistributor());

            // Act
            remeshHandler.RemeshChunk(ChunkProperties);
            List<RemeshTaskStack> tasks = new List<RemeshTaskStack>();
            remeshHandler.FinishTasks(tasks);

            // Assert
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual(3, tasks[0].TaskCount);
            Assert.IsInstanceOf<VisualRemeshTask>(tasks[0].GetTask(0));
            Assert.IsInstanceOf<VisualRemeshTask>(tasks[0].GetTask(1));
            Assert.IsInstanceOf<CollisionRemeshTask>(tasks[0].GetTask(2));

            Assert.AreEqual(0, (tasks[0].GetTask(0) as VisualRemeshTask).MaterialID);
            Assert.AreEqual(1, (tasks[0].GetTask(1) as VisualRemeshTask).MaterialID);
        }
    }
}
