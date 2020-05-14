using NUnit.Framework;
using System.IO;

namespace Test
{
    public class DatabaseTestBase
    {
        public string TestFolder;

        [SetUp]
        public void SetupTestFolder()
        {
            TestFolder = Directory.CreateDirectory("test-data/WorldDatabaseTest").FullName;
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(TestFolder, true);
        }
    }
}
