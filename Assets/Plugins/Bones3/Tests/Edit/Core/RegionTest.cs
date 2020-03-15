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
        }
    }
}
