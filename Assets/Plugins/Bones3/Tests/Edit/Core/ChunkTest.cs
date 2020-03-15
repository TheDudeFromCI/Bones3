using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Bones3;

namespace Tests
{
    public class ChunkTest
    {
        private Chunk chunk;

        [SetUp]
        public void Setup()
        {
            chunk = new GameObject().AddComponent<Chunk>();
        }

        [TearDown]
        public void Teardown()
        {
            if (Application.isPlaying)
                Object.Destroy(chunk);
            else
                Object.DestroyImmediate(chunk);
        }

        [Test]
        public void SetBlock()
        {
            ushort block = 14;

            chunk.SetBlock(3, 4, 8, block);

            Assert.AreEqual(block, chunk.GetBlock(3, 4, 8));
        }

        [Test]
        public void SetBlock_OutsideOfRange()
        {
            Assert.Throws<Bones3Exception>(() => chunk.SetBlock(-1, 2, 2, 17));
        }

        [Test]
        public void BlockCount()
        {
            Assert.AreEqual(0, chunk.BlockCount);

            chunk.SetBlock(1, 2, 3, 1);
            Assert.AreEqual(1, chunk.BlockCount);

            chunk.SetBlock(1, 5, 0, 17);
            Assert.AreEqual(2, chunk.BlockCount);

            chunk.SetBlock(1, 5, 0, 4);
            Assert.AreEqual(2, chunk.BlockCount);

            chunk.SetBlock(1, 2, 3, 0);
            Assert.AreEqual(1, chunk.BlockCount);
        }
    }
}
