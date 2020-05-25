using Bones3Rebuilt;
using Bones3Rebuilt.Database;
using NUnit.Framework;
using System.IO;
using Moq;

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

            var handler = new Mock<IFileHandler<string>>();
            db.RegisterFileHandler(handler.Object);

            db.SaveObject("Hello World!");
            handler.Verify(h => h.Save(rootFolder, "Hello World!"));

            db.LoadObject<string>();
            handler.Verify(h => h.Load(rootFolder, null));
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
