using NUnit.Framework;
using Bones3Rebuilt;
using System.IO;

namespace Test
{
    public class WorldPropertiesHandlerTest : DatabaseTestBase
    {
        [Test]
        public void WorldProperties_SaveAndLoad()
        {
            var rootFolder = TestFolder + "/Saves/newworld01";
            Directory.CreateDirectory(rootFolder);

            var props = new WorldProperties
            {
                ChunkSize = new GridSize(2),
                WorldName = "New World",
            };

            var handler = new WorldPropertiesHandler();
            var saveTask = handler.Save(rootFolder, props);
            var saveResults = saveTask.FinishTask();
            Assert.IsNull(saveResults.Error);
            Assert.IsTrue(saveResults.SuccessfullySaved);

            var loadTask = handler.Load(rootFolder);
            var loadResults = loadTask.FinishTask();
            Assert.IsNull(saveResults.Error);
            Assert.IsTrue(loadResults.SuccessfullyLoaded);
            Assert.AreEqual(props, loadResults.Data);
        }

        [Test]
        public void LoadWorldProperties_DoesNotExist()
        {
            var rootFolder = TestFolder + "/Saves/world";
            Directory.CreateDirectory(rootFolder);

            var handler = new WorldPropertiesHandler();

            var loadTask = handler.Load(rootFolder);
            var loadResults = loadTask.FinishTask();

            Assert.IsInstanceOf(typeof(FileNotFoundException), loadResults.Error);
            Assert.IsFalse(loadResults.SuccessfullyLoaded);
            Assert.IsNull(loadResults.Data);
        }

        [Test]
        public void LoadWorldProperties_UnknownFileVersion()
        {
            var rootFolder = TestFolder + "/Saves/world";
            Directory.CreateDirectory(rootFolder);

            File.WriteAllBytes(rootFolder + "/properties.dat", new byte[] { 0x02, 0x03, 0xAF, 0x00 });

            var handler = new WorldPropertiesHandler();
            var loadTask = handler.Load(rootFolder);
            var loadResults = loadTask.FinishTask();

            Assert.IsInstanceOf(typeof(UnknownFileVersionException), loadResults.Error);
            Assert.IsFalse(loadResults.SuccessfullyLoaded);
            Assert.IsNull(loadResults.Data);
        }

        [Test]
        public void SaveWorldProperties_DirectoryDoesNotExist()
        {
            var rootFolder = TestFolder + "/NotReal/world";

            var props = new WorldProperties
            {
                ChunkSize = new GridSize(2),
                WorldName = "New World",
            };

            var handler = new WorldPropertiesHandler();
            var saveTask = handler.Save(rootFolder, props);
            var saveResults = saveTask.FinishTask();

            Assert.IsInstanceOf(typeof(IOException), saveResults.Error);
            Assert.IsFalse(saveResults.SuccessfullySaved);
        }
    }
}