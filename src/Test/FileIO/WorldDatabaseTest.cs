using Bones3Rebuilt;
using NUnit.Framework;
using System.IO;

namespace Test
{
    public class WorldDatabaseTest : DatabaseTestBase
    {
        [Test]
        public void NewWorldDatabase_CreatesDirectory()
        {
            new WorldDatabase(TestFolder + "/Saves/newworld01");
            Assert.IsTrue(Directory.Exists(TestFolder + "/Saves/newworld01"));
        }

        [Test]
        public void RegisterHandler()
        {
            var rootFolder = TestFolder + "/Saves/world15";
            var db = new WorldDatabase(rootFolder);

            var handler = new WorldPropertiesHandler();
            db.RegisterFileHandler(handler);

            var props = new WorldProperties
            {
                ChunkSize = new GridSize(2),
                WorldName = "New World",
            };

            var saveTask = db.SaveObject(props);
            saveTask.FinishTask();

            var loadTask = db.LoadObject<WorldProperties>();
            var loadData = loadTask.FinishTask();

            Assert.AreEqual(props, loadData.Data);
        }

        [Test]
        public void HandlerNotFound_ThrowsError()
        {
            var rootFolder = TestFolder + "/Saves/world15";
            var db = new WorldDatabase(rootFolder);

            var props = new WorldProperties
            {
                ChunkSize = new GridSize(2),
                WorldName = "New World",
            };

            Assert.Throws<System.ArgumentException>(() => db.SaveObject(props));
            Assert.Throws<System.ArgumentException>(() => db.LoadObject(props));
        }
    }
}
