using NUnit.Framework;
using Bones3Rebuilt;
using Moq;

namespace Test
{
    public class RemeshHandlerTest
    {
        [Test]
        public void AddRemove_Distributor()
        {
            var handler = new RemeshHandler();
            var distributor = new Mock<IRemeshDistributor>();
            var chunkProperties = new Mock<IChunkProperties>();

            handler.AddDistributor(distributor.Object);
            handler.RemeshChunk(chunkProperties.Object);
            distributor.Verify(dis => dis.CreateTasks(chunkProperties.Object, It.IsAny<RemeshTaskStack>()), Times.Once);

            handler.RemoveDistributor(distributor.Object);
            handler.RemeshChunk(chunkProperties.Object);
            distributor.Verify(dis => dis.CreateTasks(chunkProperties.Object, It.IsAny<RemeshTaskStack>()), Times.Once);
        }

        [Test]
        public void FinishTasks()
        {
            var handler = new RemeshHandler();
            var distributor = new Mock<IRemeshDistributor>();
            var chunkProperties = new Mock<IChunkProperties>();

            var task0 = new Mock<IRemeshTask>();
            var task1 = new Mock<IRemeshTask>();
            var task2 = new Mock<IRemeshTask>();

            distributor.Setup(dis => dis.CreateTasks(chunkProperties.Object, It.IsAny<RemeshTaskStack>()))
                .Callback<IChunkProperties, RemeshTaskStack>((properties, taskStack) =>
                 {
                     taskStack.AddVisualTask(task0.Object);
                     taskStack.AddVisualTask(task1.Object);
                     taskStack.AddCollisionTask(task2.Object);
                 });

            handler.AddDistributor(distributor.Object);
            handler.RemeshChunk(chunkProperties.Object);

            handler.FinishTasks();
            handler.FinishTasks();

            task0.Verify(t => t.Finish(), Times.Once);
            task1.Verify(t => t.Finish(), Times.Once);
            task2.Verify(t => t.Finish(), Times.Once);
        }

        [Test]
        public void ThrowsRemeshEvent()
        {
            var handler = new RemeshHandler();
            var distributor = new Mock<IRemeshDistributor>();
            var chunkProperties = new Mock<IChunkProperties>();

            var task0 = new Mock<IRemeshTask>();
            var task1 = new Mock<IRemeshTask>();
            var task2 = new Mock<IRemeshTask>();

            task0.Setup(t => t.Mesh).Returns(new ProcMesh());
            task1.Setup(t => t.Mesh).Returns(new ProcMesh());
            task2.Setup(t => t.Mesh).Returns(new ProcMesh());

            distributor.Setup(dis => dis.CreateTasks(chunkProperties.Object, It.IsAny<RemeshTaskStack>()))
                .Callback<IChunkProperties, RemeshTaskStack>((properties, taskStack) =>
                 {
                     taskStack.AddVisualTask(task0.Object);
                     taskStack.AddVisualTask(task1.Object);
                     taskStack.AddCollisionTask(task2.Object);
                 });

            handler.AddDistributor(distributor.Object);
            handler.RemeshChunk(chunkProperties.Object);

            int calls = 0;
            handler.OnRemeshFinish += ev =>
            {
                calls++;

                Assert.AreEqual(2, ev.Report.VisualMesh.TotalLayers);
                Assert.AreEqual(1, ev.Report.CollisionMesh.TotalLayers);
            };

            handler.FinishTasks();

            Assert.AreEqual(1, calls);
        }
    }
}