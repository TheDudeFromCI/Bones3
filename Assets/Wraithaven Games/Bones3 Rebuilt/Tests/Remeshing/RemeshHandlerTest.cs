using NUnit.Framework;
using Bones3Rebuilt.Remeshing;
using Moq;
using System.Collections.Generic;

namespace Test
{
    public class RemeshHandlerTest
    {
        [Test]
        public void Add_Distributor()
        {
            var handler = new RemeshHandler();
            var distributor = new Mock<IRemeshDistributor>();
            var chunkProperties = new Mock<ChunkProperties>();

            handler.AddDistributor(distributor.Object);
            handler.RemeshChunk(chunkProperties.Object);
            distributor.Verify(dis => dis.CreateTasks(chunkProperties.Object, It.IsAny<RemeshTaskStack>()), Times.Once);
        }

        [Test]
        public void FinishTasks()
        {
            var handler = new RemeshHandler();
            var distributor = new Mock<IRemeshDistributor>();
            var chunkProperties = new Mock<ChunkProperties>();

            var task0 = new Mock<IRemeshTask>();
            var task1 = new Mock<IRemeshTask>();
            var task2 = new Mock<IRemeshTask>();

            distributor.Setup(dis => dis.CreateTasks(chunkProperties.Object, It.IsAny<RemeshTaskStack>()))
                .Callback<ChunkProperties, RemeshTaskStack>((properties, taskStack) =>
                 {
                     taskStack.AddTask(task0.Object);
                     taskStack.AddTask(task1.Object);
                     taskStack.AddTask(task2.Object);
                 });

            handler.AddDistributor(distributor.Object);
            handler.RemeshChunk(chunkProperties.Object);

            List<RemeshTaskStack> taskStacks = new List<RemeshTaskStack>();
            handler.FinishTasks(taskStacks);

            task0.Verify(t => t.Finish(), Times.Once);
            task1.Verify(t => t.Finish(), Times.Once);
            task2.Verify(t => t.Finish(), Times.Once);
            Assert.AreEqual(1, taskStacks.Count);
        }
    }
}