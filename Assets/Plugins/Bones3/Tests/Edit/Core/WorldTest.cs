using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Bones3;

namespace Tests
{
    public class WorldTest
    {
        private World world;

        [SetUp]
        public void Setup()
        {
            world = new GameObject().AddComponent<World>();
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(world);
        }

        [Test]
        public void GetChunk()
        {
            Chunk chunk = world.GetChunk(-104, 1005, 2349, true);
            Assert.IsNotNull(chunk);
            Assert.AreEqual(-104, chunk.X);
            Assert.AreEqual(1005, chunk.Y);
            Assert.AreEqual(2349, chunk.Z);
        }

        [Test]
        public void GetChunk_DontCreate()
        {
            Chunk chunk = world.GetChunk(-104, 1005, 2349, false);
            Assert.IsNull(chunk);
        }

        [Test]
        public void ParentOfRegion()
        {
            world.GetChunk(200, 200, 200, true);
            Assert.AreEqual(1, world.transform.childCount);

            Region region = world.GetComponentInChildren<Region>();
            Assert.AreEqual(12, region.X);
            Assert.AreEqual(12, region.Y);
            Assert.AreEqual(12, region.Z);
        }

        [Test]
        public void RegionNamedCorrectly()
        {
            world.GetChunk(20, 30, 40, true);
            Region region = world.GetComponentInChildren<Region>();
            Assert.AreEqual("Region (1, 1, 2)", region.gameObject.name);
        }
    }
}