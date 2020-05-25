using NUnit.Framework;
using Bones3Rebuilt.Database.ChunkData;
using Bones3Rebuilt;

namespace Test
{
    public class ChunkDataHandlerTest : DatabaseTestBase
    {
        public ChunkData ChunkData;
        public ChunkDataHandler Handler;

        [SetUp]
        public void Init()
        {
            var size = new GridSize(4);
            ChunkData = new ChunkData(size);
            ChunkData.Blocks[1] = 1;
            ChunkData.Blocks[2] = 2;
            ChunkData.Blocks[3] = 3;

            TestFolder += "/ChunkDataTest";
            System.IO.Directory.CreateDirectory(TestFolder);

            Handler = new ChunkDataHandler();
        }

        [Test]
        public void Save()
        {
            ChunkData.Position = new ChunkPosition(0, 0, 1);

            var task = Handler.Save(TestFolder, ChunkData);
            var results = task.FinishTask();

            Assert.IsNull(results.Error);
            Assert.IsTrue(results.SuccessfullySaved);
        }

        [Test]
        public void Load()
        {
            ChunkData.Position = new ChunkPosition(0, 0, 2);
            Handler.Save(TestFolder, ChunkData).FinishTask();

            var newChunk = new ChunkData(new GridSize(4));
            newChunk.Position = new ChunkPosition(0, 0, 2);

            var task = Handler.Load(TestFolder, newChunk);
            var results = task.FinishTask();

            Assert.IsNull(results.Error);
            Assert.IsTrue(results.SuccessfullyLoaded);
            Assert.AreEqual(ChunkData.Blocks, newChunk.Blocks);
        }
    }
}