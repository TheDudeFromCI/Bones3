using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Bones3;

namespace Tests
{
    public class RegionTest
    {
        private Region region;

        [SetUp]
        public void Setup()
        {
            region = new GameObject().AddComponent<Region>();
        }

        [TearDown]
        public void Teardown()
        {
            if (Application.isPlaying)
                Object.Destroy(region);
            else
                Object.DestroyImmediate(region);
        }

        [Test]
        public void GetChunk()
        {
            Chunk chunk1 = region.GetChunk(3, 4, 5, false);
            Assert.IsNull(chunk1);

            Chunk chunk2 = region.GetChunk(7, 8, 9, true);
            Assert.IsNotNull(chunk2);
            Assert.AreEqual(7, chunk2.X);
            Assert.AreEqual(8, chunk2.Y);
            Assert.AreEqual(9, chunk2.Z);
        }

        public void DestroyChunk()
        {
            region.GetChunk(0, 0, 0, true);
            region.DestroyChunk(0, 0, 0);

            Assert.IsNull(region.GetChunk(0, 0, 0, false));
        }

        public void ChunkCount()
        {
            Assert.AreEqual(0, region.ChunkCount);

            region.GetChunk(0, 1, 2, true);
            region.GetChunk(1, 1, 2, true);
            region.GetChunk(2, 1, 2, true);
            region.GetChunk(3, 1, 2, true);

            Assert.AreEqual(4, region.ChunkCount);

            region.DestroyChunk(1, 1, 2);
            region.DestroyChunk(2, 1, 2);

            Assert.AreEqual(2, region.ChunkCount);
        }
    }
}
